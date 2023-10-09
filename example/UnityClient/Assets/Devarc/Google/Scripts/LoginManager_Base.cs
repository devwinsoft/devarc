using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace Devarc
{
    public abstract class LoginManager_Base : MonoBehaviour
    {
        public event CallbackSignIn OnSignIn;
        public event CallbackSignOut OnSignOut;

        public LoginType loginType => mPrefsLoginType.Value;

        protected enum STATE
        {
            INIT,
            NEED_SIGNIN_COMPLETE,
            COMPLETED,
        };
        protected STATE mState = STATE.INIT;

        protected EnumPrefs<LoginType> mPrefsLoginType = new EnumPrefs<LoginType>("login_type", LoginType.NONE);
        protected StringPrefs mPrefsAccountID = new StringPrefs("user_id", "");
        protected StringPrefs mPrefsSecret = new StringPrefs("secret", "");

        public virtual void clear()
        {
            mPrefsLoginType.Value = LoginType.NONE;
            mPrefsAccountID.Value = string.Empty;
            mPrefsSecret.Value = string.Empty;
        }

        protected bool hasError(UnityWebRequest request)
        {
            if (request == null) return true;
            if (request.error != null) return true;
            try
            {
                var result = JsonUtility.FromJson<CommonResult>(request.downloadHandler.text);
                if (result == null) return true;
                return result.errorCode != ErrorType.SUCCESS;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                return true;
            }
        }

        protected void notifySignIn(LoginType loginType, bool firstInit)
        {
            if (loginType != LoginType.NONE)
            {
                mState = STATE.COMPLETED;
            }
            else
            {
                mState = STATE.INIT;
            }
            mPrefsLoginType.Value = loginType;
            OnSignIn?.Invoke(loginType, firstInit);
        }

        protected void notifySignOut(bool success)
        {
            clear();
            OnSignOut?.Invoke(success);
        }


        public void Custom_Register(string account_id, string passwd)
        {
            StopAllCoroutines();
            StartCoroutine(custom_register(account_id, passwd));
        }

        IEnumerator custom_register(string account_id, string passwd)
        {
            var info = DEV_Settings.Instance.loginData.custom;
            var url = $"{info.base_uri}/register?account_id={account_id}&passwd={EncryptUtil.Encrypt_Base64(passwd)}";
            var request = UnityWebRequest.Get(url);
            request.certificateHandler = new CertificateHandler_AcceptAll();

            yield return request.SendWebRequest();
            request.certificateHandler.Dispose();

            if (hasError(request))
            {
                notifySignIn(LoginType.NONE, true);
                yield break;
            }

            mPrefsAccountID.Value = account_id;
            mPrefsSecret.Value = request.downloadHandler.text;
            notifySignIn(LoginType.CUSTOM, true);
        }


        public void Custom_SignIn(string account_id, string passwd)
        {
            StopAllCoroutines();
            StartCoroutine(custom_signin(account_id, passwd));
        }

        IEnumerator custom_signin(string account_id, string passwd)
        {
            var info = DEV_Settings.Instance.loginData.custom;
            var url = $"{info.base_uri}/signin?account_id={account_id}&passwd={EncryptUtil.Encrypt_Base64(passwd)}";
            var request = UnityWebRequest.Get(url);
            request.certificateHandler = new CertificateHandler_AcceptAll();

            yield return request.SendWebRequest();
            request.certificateHandler.Dispose();

            if (hasError(request))
            {
                notifySignIn(LoginType.NONE, true);
                yield break;
            }

            mPrefsLoginType.Value = LoginType.CUSTOM;
            mPrefsAccountID.Value = account_id;
            var packet = JsonUtility.FromJson<CustomSigninResult>(request.downloadHandler.text);
            mPrefsSecret.Value = packet.secret;

            yield return logIn(true);
        }

        public void Custom_SignOut()
        {
            if (mPrefsLoginType ==  LoginType.CUSTOM)
            {
                clear();
                notifySignOut(true);
            }
        }


        public void LogIn(bool firstInit)
        {
            if (loginType != LoginType.NONE)
            {
                StopAllCoroutines();
                StartCoroutine(logIn(firstInit));
            }
        }

        protected IEnumerator logIn(bool firstInit)
        {
            var info = DEV_Settings.Instance.loginData;
            var account_id = mPrefsAccountID.Value;
            var secret = mPrefsSecret.Value;

            if (string.IsNullOrEmpty(account_id))
            {
                notifySignIn(LoginType.NONE, firstInit);
                yield break;
            }

            var url = $"{info.login_uri}?account_id={account_id}&secret={secret}";
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (hasError(request))
            {
                notifySignIn(LoginType.NONE, firstInit);
                yield break;
            }

            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                notifySignIn(LoginType.NONE, firstInit);
                yield break;
            }

            notifySignIn(mPrefsLoginType.Value, firstInit);
        }
    }
}
