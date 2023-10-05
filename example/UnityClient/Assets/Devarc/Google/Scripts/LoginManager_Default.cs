using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Devarc
{
    [Serializable]
    public class GoogleUserToken
    {
        public string access_token;
        public int expires_in;
        public string id_token;
        public string refresh_token;
        public string scope;
        public string token_type;
    }

    [Serializable]
    public class GoogleUserInfo
    {
        public string sub; // Id;
        public string name;
        public string given_name;
        public string family_name;
        public string picture;
        public string email;
        public bool email_verified;
        public string locale;
    }


    public abstract class LoginManager_Default : LoginManager_Base, ILoginManager
    {
        protected abstract void signin_open();

        protected string state = string.Empty;
        protected string code_verifier = string.Empty;
        protected string code_challenge = string.Empty;

        const char Base64Character62 = '+';
        const char Base64Character63 = '/';
        const string Base64DoublePadCharacter = "==";
        const char Base64PadCharacter = '=';
        const char Base64UrlCharacter62 = '-';
        const char Base64UrlCharacter63 = '_';


        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                var info = DEV_Settings.Instance.googleWebData;
                switch (mState)
                {
                    case STATE.NEED_SIGNIN_COMPLETE:
                        StartCoroutine(signin_complete(info.redirect_uri, null));
                        break;
                    case STATE.COMPLETED:
                        LogIn(false);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void clear()
        {
            base.clear();

            state = string.Empty;
            code_challenge = string.Empty;
            code_verifier = string.Empty;
        }

        public void SignIn()
        {
            StopAllCoroutines();

            mState = STATE.NEED_SIGNIN_COMPLETE;
            signin_open();
        }

        protected IEnumerator signin_complete(string redirect_uri, string code =  null)
        {
            var info = DEV_Settings.Instance.googleWebData;

            // Get code
            if (string.IsNullOrEmpty(code))
            {
                var url = $"{info.code_uri}?state={state}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                code = request.downloadHandler.text;
                if (hasError(request))
                {
                    notifySignIn(false, true);
                    yield break;
                }
            }

            // Pednding or sign-in window is closed.
            if (string.IsNullOrEmpty(code))
            {
                // Skip error notification.
                yield break;
            }

            // Get access_token.
            {
                var url = $"{info.signin_uri}?code={code}&code_verifier={code_verifier}&redirect_uri={redirect_uri}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                if (hasError(request))
                {
                    notifySignIn(false, true);
                    yield break;
                }

                mPrefsAccessToken.Value = request.downloadHandler.text;
            }

            // Get account_id.
            {
                var request = UnityWebRequest.Get(DEV_Settings.UserinfoURL);
                request.SetRequestHeader("Authorization", $"Bearer {mPrefsAccessToken.Value}");
                yield return request.SendWebRequest();

                if (hasError(request))
                {
                    notifySignIn(false, true);
                    yield break;
                }

                var userInfo = JsonUtility.FromJson<GoogleUserInfo>(request.downloadHandler.text);
                mPrefsAccountID.Value = userInfo.email;
            }

            Debug.Log(mPrefsAccountID.Value);
            notifySignIn(true, true);
        }


        public void SignOut()
        {
            mState = STATE.INIT;
            StopAllCoroutines();
            StartCoroutine(signout());
        }

        protected IEnumerator signout()
        {
            if (string.IsNullOrEmpty(mPrefsAccessToken.Value))
            {
                notifySignOut(false);
                yield break;
            }

            var request = UnityWebRequest.PostWwwForm($"{DEV_Settings.RevocationURI}?token={mPrefsAccessToken.Value}", "");
            yield return request.SendWebRequest();

            notifySignOut(true);
        }


        protected static string CreateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

            string value = Convert.ToBase64String(hash, 0, hash.Length);
            value = value.Split(Base64PadCharacter)[0];                     // Remove trailing padding i.e. = or ==
            value = value.Replace(Base64Character62, Base64UrlCharacter62); // Replace + with -
            value = value.Replace(Base64Character63, Base64UrlCharacter63); // Replace / with _
            return value;
        }
    }
}
