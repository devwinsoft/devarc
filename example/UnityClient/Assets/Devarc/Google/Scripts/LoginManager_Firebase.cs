#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Google;
using Firebase.Auth;
using Firebase.Extensions;

namespace Devarc
{
    public class LoginManager_Firebase : MonoSingleton<LoginManager_Firebase>, ILoginManager
    {
        public static Firebase.FirebaseApp app;
        public static FirebaseAuth auth;
        public static FirebaseUser user;
        GoogleSignInConfiguration configuration;

        System.Action<bool> mCallback = null;


        static bool HasError(UnityWebRequest request)
        {
            if (request == null) return true;
            if (request.error != null) return true;
            return false;
        }

        protected override void onAwake()
        {
            configuration = new GoogleSignInConfiguration
            {
                RequestEmail = true,
                RequestIdToken = true,
                WebClientId = "880902336083-rsba61ob5nmm0vbpbbbcgqkb9ccb1ga9.apps.googleusercontent.com"
            };

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    app = Firebase.FirebaseApp.DefaultInstance;
                    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                    auth.StateChanged += AuthStateChanged;
                    AuthStateChanged(this, null);
                }
                else
                {
                    Debug.LogError(string.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }

        protected override void onDestroy()
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }


        public void SignIn(System.Action<bool> callback)
        {
            mCallback = callback;
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("IsFaulted");
                    using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                            Debug.Log("Got Error: " + error.Status + " " + error.Message);
                        }
                        else
                        {
                            Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                        }
                    }
                }
                else if (task.IsCanceled)
                {
                    Debug.Log("IsCanceled");
                }
                else
                {
                    Debug.Log(task.Result.IdToken);

                    var user = task.Result;
                    auth.CurrentUser.TokenAsync(true).ContinueWithOnMainThread(task =>
                    {
                    });
                }
            });
        }


        public void SignOut()
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }


        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != user)
            {
                bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                    && auth.CurrentUser.IsValid();
                if (!signedIn && user != null)
                {
                    Debug.Log("Signed out " + user.UserId);
                }
                user = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log("Signed in " + user.UserId);
                }
            }
        }




        //void SignInWithGoogle(bool linkWithCurrentAnonUser)
        //{
        //    Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        //    TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        //    signIn.ContinueWith(task =>
        //    {
        //        mDispatcher.Invoke((args) =>
        //        {
        //            Debug.Log("signIn.ContinueWith");
        //        });
        //        if (task.IsCanceled)
        //        {
        //            signInCompleted.SetCanceled();
        //        }
        //        else if (task.IsFaulted)
        //        {
        //            signInCompleted.SetException(task.Exception);
        //        }
        //        else
        //        {
        //            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
        //            if (linkWithCurrentAnonUser)
        //            {
        //                AppManager.auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(HandleLoginResult);
        //            }
        //            else
        //            {
        //                SignInWithCredential(credential);
        //            }
        //        }
        //    });
        //}

        //void HandleLoginResult(Task<AuthResult> task)
        //{
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("SignInWithCredentialAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception.InnerException.Message);
        //        return;
        //    }
        //    else
        //    {
        //        AuthResult authResult = task.Result;
        //        Debug.Log($"User signed in successfully: {authResult.User.DisplayName} ({authResult.User.UserId})");
        //        mCallback?.Invoke(true);
        //    }
        //}

        //void SignInWithCredential(Credential credential)
        //{
        //    Debug.Log("SignInWithCredential");
        //    AppManager.auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        //    {
        //        if (task.IsCanceled)
        //        {
        //            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
        //            return;
        //        }
        //        if (task.IsFaulted)
        //        {
        //            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
        //            return;
        //        }
        //        Firebase.Auth.AuthResult result = task.Result;
        //        Debug.LogFormat("User signed in successfully: {0} ({1})",
        //            result.User.DisplayName, result.User.UserId);
        //    });
        //}
    }
}
#endif