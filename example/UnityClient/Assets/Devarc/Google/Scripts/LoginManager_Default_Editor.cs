using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Devarc
{
    public class LoginManager_Default_Editor : LoginManager_Default
    {
        HttpListener mListener = null;

        protected override void signin_open()
        {
            clear();

            var info = DEV_Settings.Instance.loginData.google;
            state = Guid.NewGuid().ToString();
            code_verifier = Guid.NewGuid().ToString();
            code_challenge = CreateCodeChallenge(code_verifier);

            var url = $"{GoogleAuthURI}?response_type=code&access_type=offline&scope={Uri.EscapeDataString(string.Join(" ", mScopes))}&redirect_uri={Uri.EscapeDataString(info.loopback_uri)}&client_id={info.client_id}&state={state}&code_challenge={code_challenge}&code_challenge_method=S256";
            Application.OpenURL(url);
            AddHttpListener();
        }


        //static int GetAvailPort()
        //{
        //    var listener = new TcpListener(IPAddress.Loopback, 0);
        //    listener.Start();
        //    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        //    listener.Stop();
        //    return port;
        //}

        void AddHttpListener()
        {
            if (mListener != null)
                return;

            var info = DEV_Settings.Instance.loginData.google;
            mListener = new System.Net.HttpListener();
            mListener.Prefixes.Add(info.loopback_uri + "/");
            mListener.Start();

            var context = System.Threading.SynchronizationContext.Current;
            var asyncResult = mListener.BeginGetContext(result => context.Send(HandleHttpListener, result), mListener);

            if (!Application.runInBackground)
            {
                Debug.LogWarning("HttpListener is blocking the main thread. To avoid this, set [Run In Background] from [Player Settings].");
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    Debug.LogWarning("No response received.");
                }
            }
        }


        void HandleHttpListener(object arg)
        {
            var info = DEV_Settings.Instance.loginData.google;
            var result = (IAsyncResult)arg;
            //var httpListener = (System.Net.HttpListener)result.AsyncState;
            var context = mListener.EndGetContext(result);

            // Send an HTTP response to the browser to notify the user to close the browser.
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(Resources.Load<TextAsset>("StandaloneTemplate").text.Replace("{0}", Application.productName));

            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            mListener.Close();
            mListener = null;

            var parameters = new NameValueCollection();
            foreach (Match match in Regex.Matches(context.Request.Url.AbsoluteUri, @"(?<key>\w+)=(?<value>[^&#]+)"))
            {
                parameters.Add(match.Groups["key"].Value, Uri.UnescapeDataString(match.Groups["value"].Value));
            }

            var error = parameters.Get("error");
            if (error != null)
            {
                notifySignIn(LoginType.GOOGLE, true);
                return;
            }

            var code = parameters.Get("code");
            StartCoroutine(signin_complete(info.loopback_uri, code));
        }
    }

}
