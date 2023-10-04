using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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

    public class LoginManager_WebGL : MonoSingleton<LoginManager_WebGL>, ILoginManager
    {
        const string AuthorizationURI = "https://accounts.google.com/o/oauth2/v2/auth";
        const string UserinfoURL = "https://www.googleapis.com/oauth2/v2/userinfo";
        const string RevocationURI = "https://oauth2.googleapis.com/revoke";

        StringPrefs mPrefsAccountID = new StringPrefs("user_id", "");
        StringPrefs mPrefsAccessToken = new StringPrefs("access_token", "");

        bool mBusy1 = false;
        bool mBusy2 = false;
        System.Action<bool> mCallback = null;
        string state = string.Empty;
        string code_verifier = string.Empty;
        string code_challenge = string.Empty;

        private const char Base64Character62 = '+';
        private const char Base64Character63 = '/';
        private const string Base64DoublePadCharacter = "==";
        private const char Base64PadCharacter = '=';
        private const char Base64UrlCharacter62 = '-';
        private const char Base64UrlCharacter63 = '_';


        void Clear()
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
                StartCoroutine(signin_1());
            }
        }

        IEnumerator signin_1()
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

            {
                Clear();
                state = Guid.NewGuid().ToString();
                code_verifier = Guid.NewGuid().ToString();
                code_challenge = CreateCodeChallenge(code_verifier);

                var url = $"{AuthorizationURI}?response_type=code&scope={Uri.EscapeDataString(string.Join(" ", info.scopes))}&redirect_uri={Uri.EscapeDataString(info.redirect_uri)}&client_id={info.client_id}&state={state}&code_challenge={code_challenge}&code_challenge_method=S256";
                Application.OpenURL(url);
            }
        }


        private void OnApplicationFocus(bool focus)
        {
            if (focus && mBusy1 && mBusy2 == false)
            {
                mBusy2 = true;
                StartCoroutine(signin_2());
            }
        }

        IEnumerator signin_2()
        {
            var info = DEV_Settings.Instance.googleWebData;
            string code = string.Empty;

            // Get code
            {
                var url = $"{info.code_uri}?state={state}";
                var request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                code = request.downloadHandler.text;
                if (HasError(request) || string.IsNullOrEmpty(code))
                {
                    mBusy1 = false;
                    mBusy2 = false;
                    mCallback?.Invoke(false);
                    yield break;
                }
            }

            // Get access_token.
            {
                var url = $"{info.signin_uri}?code={code}&code_verifier={code_verifier}";
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
                var request = UnityWebRequest.Get(UserinfoURL);
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
                StartCoroutine(Instance.signout());
            }
        }

        IEnumerator signout()
        {
            if (string.IsNullOrEmpty(mPrefsAccessToken.Value))
            {
                mBusy1 = false;
                yield break;
            }

            var request = UnityWebRequest.PostWwwForm($"{RevocationURI}?token={mPrefsAccessToken.Value}", "");
            yield return request.SendWebRequest();

            mBusy1 = false;
            Clear();
            Debug.Log($"SignOut: success={request.result == UnityWebRequest.Result.Success}");
        }


        static string CreateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

            string value = Convert.ToBase64String(hash, 0, hash.Length);
            value = value.Split(Base64PadCharacter)[0];                     // Remove trailing padding i.e. = or ==
            value = value.Replace(Base64Character62, Base64UrlCharacter62); // Replace + with -
            value = value.Replace(Base64Character63, Base64UrlCharacter63); // Replace / with _
            return value;
        }

        static bool HasError(UnityWebRequest request)
        {
            if (request == null) return true;
            if (request.error != null) return true;
            return false;
        }

        static int GetAvailPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        //public void AddHttpListenerCallback()
        //{
        //    var httpListener = new System.Net.HttpListener();
        //    httpListener.Prefixes.Add(redirect_uri + "/");
        //    httpListener.Start();

        //    var context = System.Threading.SynchronizationContext.Current;
        //    var asyncResult = httpListener.BeginGetContext(result => context.Send(HandleHttpListenerCallback, result), httpListener);

        //    if (!Application.runInBackground)
        //    {
        //        Debug.LogWarning("HttpListener is blocking the main thread. To avoid this, set [Run In Background] from [Player Settings].");
        //        if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(30)))
        //        {
        //            Debug.LogWarning("No response received.");
        //        }
        //    }
        //}


        //private void HandleHttpListenerCallback(object arg)
        //{
        //    var result = (IAsyncResult)arg;
        //    var httpListener = (System.Net.HttpListener)result.AsyncState;
        //    var context = httpListener.EndGetContext(result);

        //    // Send an HTTP response to the browser to notify the user to close the browser.
        //    var response = context.Response;
        //    var buffer = Encoding.UTF8.GetBytes(Resources.Load<TextAsset>("StandaloneTemplate").text.Replace("{0}", Application.productName));

        //    response.ContentEncoding = Encoding.UTF8;
        //    response.ContentType = "text/html; charset=utf-8";
        //    response.ContentLength64 = buffer.Length;

        //    var output = response.OutputStream;
        //    output.Write(buffer, 0, buffer.Length);
        //    output.Close();
        //    httpListener.Close();

        //    var parameters = new NameValueCollection();
        //    foreach (Match match in Regex.Matches(context.Request.Url.AbsoluteUri, @"(?<key>\w+)=(?<value>[^&#]+)"))
        //    {
        //        parameters.Add(match.Groups["key"].Value, Uri.UnescapeDataString(match.Groups["value"].Value));
        //    }

        //    var error = parameters.Get("error");
        //    if (error != null)
        //    {
        //        //_callbackU?.Invoke(false, error, null);
        //        return;
        //    }

        //    var code = parameters.Get("code");
        //    StartCoroutine(signin_2(code));
        //}

    }
}

