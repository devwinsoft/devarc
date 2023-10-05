using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Devarc
{
    public abstract class LoginManager_Base : MonoBehaviour
    {
        public event CallbackSignIn OnSignIn;
        public event CallbackSignOut OnSignOut;
        protected enum STATE
        {
            INIT,
            NEED_SIGNIN_COMPLETE,
            COMPLETED,
        };
        protected STATE mState = STATE.INIT;

        protected StringPrefs mPrefsAccountID = new StringPrefs("user_id", "");
        protected StringPrefs mPrefsAccessToken = new StringPrefs("access_token", "");

        public virtual void clear()
        {
            mPrefsAccountID.Value = string.Empty;
            mPrefsAccessToken.Value = string.Empty;
        }

        protected bool hasError(UnityWebRequest request)
        {
            if (request == null) return true;
            if (request.error != null) return true;
            return false;
        }

        protected void notifySignIn(bool success, bool firstInit)
        {
            if (success)
            {
                mState = STATE.COMPLETED;
            }
            else
            {
                mState = STATE.INIT;
            }
            OnSignIn?.Invoke(success, firstInit);
        }

        protected void notifySignOut(bool success)
        {
            clear();
            OnSignOut?.Invoke(success);
        }


        public void LogIn(bool firstInit)
        {
            StopAllCoroutines();
            StartCoroutine(login(firstInit));
        }

        protected IEnumerator login(bool firstInit)
        {
            var info = DEV_Settings.Instance.googleWebData;
            var account_id = mPrefsAccountID.Value;
            var access_token = mPrefsAccessToken.Value;

            if (string.IsNullOrEmpty(account_id) || string.IsNullOrEmpty(access_token))
            {
                notifySignIn(false, firstInit);
                yield break;
            }

            var url = $"{info.login_uri}?account_id={account_id}&access_token={access_token}";
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (hasError(request))
            {
                notifySignIn(false, firstInit);
                yield break;
            }

            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                notifySignIn(false, firstInit);
                yield break;
            }

            notifySignIn(true, firstInit);
        }
    }
}
