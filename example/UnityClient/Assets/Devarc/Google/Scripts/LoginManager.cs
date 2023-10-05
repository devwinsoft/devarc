using UnityEngine;

namespace Devarc
{
    public delegate void CallbackSignIn(bool success, bool firstInit);
    public delegate void CallbackSignOut(bool success);

    public interface ILoginManager
    {
        event CallbackSignIn OnSignIn;
        event CallbackSignOut OnSignOut;
        void SignIn();
        void SignOut();
        void LogIn(bool firstInit);
    }


    public class LoginManager : MonoBehaviour
    {
        public static ILoginManager Instance => mInstance;
        static ILoginManager mInstance;

        public static ILoginManager Create()
        {
            GameObject obj = new GameObject("LoginManager");
#if UNITY_EDITOR
            mInstance = obj.AddComponent<LoginManager_Default_Editor>();
#elif UNITY_ANDROID || UNITY_IOS
            mInstance = obj.AddComponent<LoginManager_Firebase>();
#else
            mInstance = obj.AddComponent<LoginManager_Default_WebGL>();
#endif
            return mInstance;
        }
    }

}