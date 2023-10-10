using UnityEngine;

namespace Devarc
{
    public enum LoginType
    {
        NONE,
        APPLE,
        GOOGLE,
        CUSTOM,
        GUEST,
    }
    public delegate void CallbackSignIn(LoginType loginType, bool firstInit);
    public delegate void CallbackSignOut(bool success);

    public interface ILoginManager
    {
        event CallbackSignIn OnSignIn;
        event CallbackSignOut OnSignOut;

        LoginType loginType { get; }

        void Custom_Register(string account_id, string passwd);
        void Custom_SignIn(string account_id, string passwd);
        void Custom_SignOut();

        void Google_SignIn();
        void Google_SignOut();
        void Google_RefreshToken();

        void LogIn(bool firstInit);
    }


    public static class LoginManager
    {
        public static ILoginManager Instance => mInstance;
        static ILoginManager mInstance;

        public static ILoginManager Create()
        {
            GameObject obj = new GameObject("LoginManager");
#if UNITY_EDITOR
            //mInstance = obj.AddComponent<LoginManager_Default_WebGL>();
            mInstance = obj.AddComponent<LoginManager_Default_Editor>();
#elif UNITY_ANDROID || UNITY_IOS
            mInstance = obj.AddComponent<LoginManager_Firebase>();
#else
            mInstance = obj.AddComponent<LoginManager_Default_WebGL>();
#endif
            GameObject.DontDestroyOnLoad(obj);
            return mInstance;
        }
    }

}