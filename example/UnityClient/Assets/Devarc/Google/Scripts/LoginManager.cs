using UnityEngine;

namespace Devarc
{
    public class LoginManager : MonoBehaviour
    {
        public static ILoginManager Instance => mInstance;
        static ILoginManager mInstance;

        public static ILoginManager Create()
        {
            GameObject obj = new GameObject("LoginManager");
#if UNITY_ANDROID || UNITY_IOS
            mInstance = obj.AddComponent<LoginManager_Mobile>();
#else
            mInstance = obj.AddComponent<LoginManager_WebGL>();
#endif
            return mInstance;
        }
    }

}