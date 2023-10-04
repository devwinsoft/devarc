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

    public class LoginManager_Default_WebGL : LoginManager_Default
    {
        protected override void OpenURL()
        {
            Clear();

            var info = DEV_Settings.Instance.googleWebData;
            state = Guid.NewGuid().ToString();
            code_verifier = Guid.NewGuid().ToString();
            code_challenge = CreateCodeChallenge(code_verifier);

            var url = $"{DEV_Settings.AuthorizationURI}?response_type=code&scope={Uri.EscapeDataString(string.Join(" ", info.scopes))}&redirect_uri={Uri.EscapeDataString(info.redirect_uri)}&client_id={info.client_id}&state={state}&code_challenge={code_challenge}&code_challenge_method=S256";
            Application.OpenURL(url);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && mBusy1 && mBusy2 == false)
            {
                mBusy2 = true;
                var info = DEV_Settings.Instance.googleWebData;
                StartCoroutine(signin_2(info.redirect_uri, null));
            }
        }
    }
}

