using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Devarc
{
    public class LoginManager_Default_WebGL : LoginManager_Default
    {
        protected override void google_signin_open()
        {
            var info = DEV_Settings.Instance.loginData.google;
            var url = $"{GoogleAuthURI}?response_type=code&access_type=offline&prompt=consent&scope={Uri.EscapeDataString(string.Join(" ", mScopes))}&redirect_uri={Uri.EscapeDataString(info.base_uri + "/redirect")}&client_id={info.client_id}&state={state}&code_challenge={code_challenge}&code_challenge_method=S256";
            Application.OpenURL(url);
        }
    }
}

