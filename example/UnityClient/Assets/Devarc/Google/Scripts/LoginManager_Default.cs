using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Devarc
{
    public abstract class LoginManager_Default : MonoBehaviour, ILoginManager
    {
        protected abstract void OpenURL();

        StringPrefs mPrefsAccountID = new StringPrefs("user_id", "");
        StringPrefs mPrefsAccessToken = new StringPrefs("access_token", "");

        protected bool mBusy1 = false;
        protected bool mBusy2 = false;
        protected string state = string.Empty;
        protected string code_verifier = string.Empty;
        protected string code_challenge = string.Empty;

        System.Action<bool> mCallback = null;

        const char Base64Character62 = '+';
        const char Base64Character63 = '/';
        const string Base64DoublePadCharacter = "==";
        const char Base64PadCharacter = '=';
        const char Base64UrlCharacter62 = '-';
        const char Base64UrlCharacter63 = '_';


        public void Clear()
        {
            state = string.Empty;
            code_challenge = string.Empty;
            code_verifier = string.Empty;

            mPrefsAccountID.Value = string.Empty;
            mPrefsAccessToken.Value = string.Empty;
        }


        public void SignIn(System.Action<bool> callback)
        {
            if (mBusy1 == false)
            {
                mBusy1 = true;
                mCallback = callback;
                StartCoroutine(open_redirect());
            }
        }

        protected IEnumerator open_redirect()
        {
            var info = DEV_Settings.Instance.googleWebData;
            var account_id = mPrefsAccountID.Value;
            var access_token = mPrefsAccessToken.Value;

            if (!string.IsNullOrEmpty(account_id) && !string.IsNullOrEmpty(access_token))
            {
                var url = $"{info.login_uri}?account_id={account_id}&access_token={access_token}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                if (HasError(request))
                {
                    mBusy1 = false;
                    mCallback?.Invoke(false);
                    yield break;
                }

                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    mBusy1 = false;
                    mCallback?.Invoke(true);
                    yield break;
                }
            }

            OpenURL();
        }


        protected IEnumerator signin_2(string redirect_uri, string code =  null)
        {
            var info = DEV_Settings.Instance.googleWebData;

            // Get code
            if (string.IsNullOrEmpty(code))
            {
                var url = $"{info.code_uri}?state={state}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                code = request.downloadHandler.text;
                if (HasError(request))
                {
                    mBusy1 = false;
                    mBusy2 = false;
                    mCallback?.Invoke(false);
                    yield break;
                }
            }

            // Code expired
            if (string.IsNullOrEmpty(code))
            {
                mBusy1 = false;
                mBusy2 = false;
                mCallback?.Invoke(false);
                yield break;
            }

            // Get access_token.
            {
                var url = $"{info.signin_uri}?code={code}&code_verifier={code_verifier}&redirect_uri={redirect_uri}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                if (HasError(request))
                {
                    mBusy1 = false;
                    mBusy2 = false;
                    mCallback?.Invoke(false);
                    yield break;
                }

                mPrefsAccessToken.Value = request.downloadHandler.text;
            }

            // Get account_id.
            {
                var request = UnityWebRequest.Get(DEV_Settings.UserinfoURL);
                request.SetRequestHeader("Authorization", $"Bearer {mPrefsAccessToken.Value}");
                yield return request.SendWebRequest();

                if (HasError(request))
                {
                    mBusy1 = false;
                    mBusy2 = false;
                    mCallback?.Invoke(false);
                    yield break;
                }

                var userInfo = JsonUtility.FromJson<GoogleUserInfo>(request.downloadHandler.text);
                mPrefsAccountID.Value = userInfo.email;
            }

            Debug.Log(mPrefsAccountID.Value);

            mBusy1 = false;
            mBusy2 = false;
            mCallback?.Invoke(true);
        }


        public void SignOut()
        {
            if (mBusy1 == false)
            {
                mBusy1 = true;
                StartCoroutine(signout());
            }
        }

        protected IEnumerator signout()
        {
            if (string.IsNullOrEmpty(mPrefsAccessToken.Value))
            {
                mBusy1 = false;
                yield break;
            }

            var request = UnityWebRequest.PostWwwForm($"{DEV_Settings.RevocationURI}?token={mPrefsAccessToken.Value}", "");
            yield return request.SendWebRequest();

            mBusy1 = false;
            Clear();
            Debug.Log($"SignOut: success={request.result == UnityWebRequest.Result.Success}");
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

        protected static bool HasError(UnityWebRequest request)
        {
            if (request == null) return true;
            if (request.error != null) return true;
            return false;
        }

    }
}
