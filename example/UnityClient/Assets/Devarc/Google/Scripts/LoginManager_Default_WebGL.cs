using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Devarc
{
    public class LoginManager_Default_WebGL : LoginManager_Default
    {
        protected override void signin_open()
        {
            clear();

            var info = DEV_Settings.Instance.loginData.google;
            state = Guid.NewGuid().ToString();
            code_verifier = Guid.NewGuid().ToString();
            code_challenge = CreateCodeChallenge(code_verifier);

            var url = $"{GoogleAuthURI}?response_type=code&access_type=offline&scope={Uri.EscapeDataString(string.Join(" ", mScopes))}&redirect_uri={Uri.EscapeDataString(info.base_uri + "/redirect")}&client_id={info.client_id}&state={state}&code_challenge={code_challenge}&code_challenge_method=S256";
            Application.OpenURL(url);
        }
    }
}

