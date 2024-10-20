using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;

namespace Devarc
{
    //[Serializable]
    //public class GoogleUserToken
    //{
    //    public string access_token;
    //    public int expires_in;
    //    public string id_token;
    //    public string refresh_token;
    //    public string scope;
    //    public string token_type;
    //}

    //[Serializable]
    //public class GoogleUserInfo
    //{
    //    public string sub; // Id;
    //    public string name;
    //    public string given_name;
    //    public string family_name;
    //    public string picture;
    //    public string email;
    //    public bool email_verified;
    //    public string locale;
    //}

    //[Serializable]
    //public class SigninResult
    //{
    //    public string account_id;
    //    public string access_token;
    //    public string refresh_token;
    //    public int expires_in;
    //    public string secret;
    //}

    public abstract class LoginManager_Default : LoginManager_Base, ILoginManager
    {
        public const string GoogleAuthURI = "https://accounts.google.com/o/oauth2/v2/auth";
        public const string GoogleTokenURL = "https://oauth2.googleapis.com/token";
        public const string GoogleUserinfoURL = "https://www.googleapis.com/oauth2/v2/userinfo";
        public const string GoogleRevokeURI = "https://oauth2.googleapis.com/revoke";


        protected abstract void google_signin_open();

        protected StringPrefs mPrefsAccessToken = new StringPrefs("access_token", "");
        protected StringPrefs mPrefsRefreshToken = new StringPrefs("refresh_token", "");

        protected string state = string.Empty;
        protected string code_verifier = string.Empty;
        protected string code_challenge = string.Empty;
        protected string[] mScopes = new string[]
        {
            "openid",
            "email",
            "profile"
        };

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
                var info = DEV_Settings.Instance.loginData.google;
                switch (mState)
                {
                    case STATE.NEED_SIGNIN_COMPLETE:
                        StartCoroutine(google_signin_complete($"{info.base_uri}/redirect", null));
                        break;
                    case STATE.COMPLETED:
                        //LogIn(false);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void clear()
        {
            base.clear();

            mPrefsAccessToken.Value = string.Empty;

            state = string.Empty;
            code_challenge = string.Empty;
            code_verifier = string.Empty;
        }

        public void Google_SignIn()
        {
            StopAllCoroutines();

            mState = STATE.NEED_SIGNIN_COMPLETE;
            clear();

            state = Guid.NewGuid().ToString();
            code_verifier = Guid.NewGuid().ToString();
            code_challenge = CreateCodeChallenge(code_verifier);

            google_signin_open();
        }

        protected IEnumerator google_signin_complete(string redirect_uri, string code =  null)
        {
            var info = DEV_Settings.Instance.loginData.google;

            // Get code
            if (string.IsNullOrEmpty(code))
            {
                var url = $"{info.base_uri}/code?state={state}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();
                if (hasError(request))
                {
                    notifySignIn(LoginType.NONE, true);
                    yield break;
                }

                state = string.Empty;
                var result = JsonUtility.FromJson<GoogleCodeResult>(request.downloadHandler.text);
                code = result.code;

                // Pednding or sign-in window is closed.
                if (string.IsNullOrEmpty(code))
                {
                    // Skip error notification.
                    yield break;
                }
            }


            //Get secrets.
            {
                var url = $"{info.base_uri}/signin?code={code}&code_verifier={code_verifier}&redirect_uri={redirect_uri}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                if (hasError(request))
                {
                    notifySignIn(LoginType.NONE, true);
                    yield break;
                }

                var result = JsonUtility.FromJson<GoogleSigninResult>(request.downloadHandler.text);
                if (result == null || string.IsNullOrEmpty(result.secret))
                {
                    notifySignIn(LoginType.NONE, true);
                    yield break;
                }

                mPrefsAccountID.Value = result.account_id;
                mPrefsAccessToken.Value = result.access_token;
                if (!string.IsNullOrEmpty(result.refresh_token))
                {
                    mPrefsRefreshToken.Value = result.refresh_token;
                }
                mPrefsSecret.Value = result.secret;
            }

            Debug.Log($"account_id:{mPrefsAccountID.Value}");
            Debug.Log($"access_token:{mPrefsAccessToken.Value}");
            Debug.Log($"refresh_token:{mPrefsRefreshToken.Value}");
            notifySignIn(LoginType.GOOGLE, true);
        }


        public void Google_SignOut()
        {
            mState = STATE.INIT;
            StopAllCoroutines();
            StartCoroutine(google_signout());
        }

        protected IEnumerator google_signout()
        {
            if (mPrefsLoginType.Value == LoginType.NONE)
            {
                notifySignOut(false);
                yield break;
            }

            string url;
            if (!string.IsNullOrEmpty(mPrefsRefreshToken.Value))
            {
                url = $"{GoogleRevokeURI}?token={mPrefsRefreshToken.Value}";
            }
            else
            {
                url = $"{GoogleRevokeURI}?token={mPrefsAccessToken.Value}";
            }
            var request = UnityWebRequest.PostWwwForm(url, "");
            yield return request.SendWebRequest();

            if (hasError(request))
            {
                notifySignIn(LoginType.NONE, false);
                yield break;
            }

            mPrefsRefreshToken.Value = string.Empty;
            notifySignOut(true);
        }


        public void Google_RefreshToken()
        {
            mState = STATE.INIT;
            StopAllCoroutines();
            StartCoroutine(google_refresh_token());
        }


        protected IEnumerator google_refresh_token()
        {
            var info = DEV_Settings.Instance.loginData.google;
            var url = $"{info.base_uri}/refresh?refresh_token={mPrefsRefreshToken.Value}";
            var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();
            if (hasError(request))
            {
                yield break;
            }

            state = string.Empty;
            var result = JsonUtility.FromJson<GoogleRefreshResult>(request.downloadHandler.text);
            mPrefsAccessToken.Value = result.access_token;
            mPrefsRefreshToken.Value = result.refresh_token;

            Debug.Log($"access_token:{mPrefsAccessToken.Value}");
            Debug.Log($"refresh_token:{mPrefsRefreshToken.Value}");
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
