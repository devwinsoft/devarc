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
    public class LoginManager_Firebase : LoginManager_Base, ILoginManager
    {
        public static Firebase.FirebaseApp app;
        public static FirebaseAuth auth;
        public static FirebaseUser user;

        GoogleSignInConfiguration configuration;

        void Awake()
        {
            configuration = new GoogleSignInConfiguration
            {
                RequestEmail = true,
                RequestAuthCode = true,
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


        void OnDestroy()
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }


        public void SignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    notifySignIn(false, true);
                }
                else
                {
                    var user = task.Result;
                    StartCoroutine(signin_complete(user.Email, user.AuthCode));
                }
            });
        }

        IEnumerator signin_complete(string account_id, string access_token)
        {
            var info = DEV_Settings.Instance.googleWebData;
            mPrefsAccountID.Value = account_id;
            mPrefsAccessToken.Value = access_token;

            if (string.IsNullOrEmpty(account_id) || string.IsNullOrEmpty(access_token))
            {
                notifySignIn(false, true);
                yield break;
            }

            var url = $"{info.signin_uri}?account_id={Uri.EscapeDataString(account_id)}&access_token={access_token}";
            var request = UnityWebRequest.Get(url);
            request.certificateHandler = new CertificateHandler_AcceptAll();

            yield return request.SendWebRequest();
            request.certificateHandler.Dispose();

            if (hasError(request))
            {
                notifySignIn(false, true);
                yield break;
            }

            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                notifySignIn(false, true);
                yield break;
            }
            notifySignIn(true, true);
        }


        public void SignOut()
        {
            if (string.IsNullOrEmpty(mPrefsAccessToken.Value))
            {
                notifySignOut(false);
            }
            else
            {
                GoogleSignIn.DefaultInstance.SignOut();
                notifySignOut(true);
            }
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