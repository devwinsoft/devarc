using System.Collections;
using UnityEngine;
using MessagePack.Resolvers;
using TMPro;
using Devarc;
using System.Collections.Generic;


public class TestNetworkScene : BaseScene
{
    public UIDebugLog debugLog;
    public TMP_Dropdown authServerAddress;
    public TMP_Dropdown socketServerAddress;
    public TMP_InputField inputID;
    public TMP_InputField inputPW;

    List<string> authServerURL = new List<string>
    { "https://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com:3000"
    , "https://localhost:3000"
    };

#if UNITY_ANDROID || UNITY_IOS
    List<string> socketServerURL = new List<string>
    { "ws://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com:4001"
    , "ws://localhost:4001"
    };
#else
    List<string> socketServerURL = new List<string>
    { "wss://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com:4000"
    , "wss://localhost:4000"
    };
#endif

    protected override void onAwake()
    {
        if (!AppManager.IsCreated())
        {
            AppManager.Create("AppManager");
        }
        Debug.Log("TestNetworkScene::onAwake");
    }


    public override IEnumerator OnEnterScene()
    {
        yield return null;
       
        Debug.Log("TestNetworkScene::OnEnterScene");
        authServerAddress.ClearOptions();
        authServerAddress.AddOptions(authServerURL);
        socketServerAddress.ClearOptions();
        socketServerAddress.AddOptions(socketServerURL);

        OnChangeNetwork();
    }


    public override void OnLeaveScene()
    {
    }


    public void OnChangeNetwork()
    {
        AppManager.authNetwork.Init(authServerAddress.captionText.text, "msgpack", "packet", StaticCompositeResolver.Instance);

        C2Auth.RequestSession request = new C2Auth.RequestSession();
        AppManager.authNetwork.Post<C2Auth.RequestSession, Auth2C.NotifySession>(request, (response) =>
        {
            switch (response.errorCode)
            {
                case ErrorType.SUCCESS:
                    AppManager.Instance.sessionID = response.sessionID;
                    AppManager.Instance.secret = response.secret;
                    break;
                default:
                    AppManager.Instance.sessionID = string.Empty;
                    AppManager.Instance.secret = 0;
                    break;
            }
            Debug.Log(JsonUtility.ToJson(response));
        });
    }


    public void OnClick_Google_Signin()
    {
        LoginManager.Instance.SignIn((success) =>
        {
            Debug.Log($"Google SignIn: success={success}");
        });
    }

    public void OnClick_Google_Signout()
    {
        LoginManager.Instance.SignOut();
    }


    public void OnClick_RequestSignin()
    {
        var request = new C2Auth.RequestSignin();
        request.accountID = inputID.text;
        request.password = EncryptUtil.Encrypt_Base64(inputPW.text);

        AppManager.authNetwork.Post<C2Auth.RequestSignin, Auth2C.NotifySignin>(request, (response) =>
        {
            switch (response.errorCode)
            {
                case ErrorType.SUCCESS:
                    AppManager.Instance.sessionID = response.sessionID;
                    AppManager.Instance.secret = response.secret;
                    break;
                default:
                    break;
            }
            Debug.Log(JsonUtility.ToJson(response));
        });
    }


    public void OnClick_RequestLogin()
    {
        var request = new C2Auth.RequestLogin();
        request.accountID = inputID.text;
        request.password = EncryptUtil.Encrypt_Base64(inputPW.text);

        AppManager.authNetwork.Post<C2Auth.RequestLogin, Auth2C.NotifyLogin>(request, (response) =>
        {
            switch (response.errorCode)
            {
                case ErrorType.SUCCESS:
                    AppManager.Instance.sessionID = response.sessionID;
                    AppManager.Instance.secret = response.secret;
                    break;
                default:
                    break;
            }
            Debug.Log(JsonUtility.ToJson(response));
        });
    }


    public void OnClick_RequestLoginout()
    {
        var request = new C2Auth.RequestLogout();

        AppManager.authNetwork.Post<C2Auth.RequestLogout, Auth2C.NotifyLogout>(request, (response) =>
        {
            switch (response.errorCode)
            {
                case ErrorType.SUCCESS:
                    AppManager.Instance.sessionID = string.Empty;
                    AppManager.Instance.secret = 0;
                    break;
                default:
                    break;
            }
            Debug.Log(JsonUtility.ToJson(response));
        });
    }



    public void OnClick_ConnectLogin()
    {
        if (AppManager.gameNetwork.IsConnected)
        {
            Debug.LogWarning("Already connected.");
        }
        else
        {
            AppManager.gameNetwork.Connect(socketServerAddress.captionText.text);
        }
    }


    public void OnClick_Test1()
    {
        AppManager.gameNetwork.DisConnect();
    }


    public void OnClick_GotoAssetTest()
    {
        SceneTransManager.Instance.LoadScene("TestAssetScene");
    }
}
