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

    List<string> authServerURLs = new List<string>
    { "https://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com:3000"
    , "https://localhost:3000"
    };

#if UNITY_ANDROID || UNITY_IOS
    List<string> socketServerURLs = new List<string>
    { "ws://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com:4001"
    , "ws://localhost:4001"
    };
#else
    List<string> socketServerURLs = new List<string>
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
        authServerAddress.AddOptions(authServerURLs);

        socketServerAddress.ClearOptions();
        socketServerAddress.AddOptions(socketServerURLs);

        AppManager.authNetwork.Init(authServerAddress.captionText.text, "msgpack", "packet", StaticCompositeResolver.Instance);
    }


    public override void OnLeaveScene()
    {
    }


    public void OnClick_Google_Signin()
    {
    }

    public void OnClick_Google_Signout()
    {
    }


    public void OnClick_CustomRegister()
    {
    }


    public void OnClick_CustomSignIn()
    {
    }


    public void OnClick_CustomSignOut()
    {
        //var request = new C2Auth.RequestLogout();
        //AppManager.authNetwork.Post<C2Auth.RequestLogout, Auth2C.NotifyLogout>(request, (response) =>
        //{
        //    switch (response.errorCode)
        //    {
        //        case ErrorType.SUCCESS:
        //            AppManager.Instance.sessionID = string.Empty;
        //            AppManager.Instance.secret = 0;
        //            break;
        //        default:
        //            break;
        //    }
        //    Debug.Log(JsonUtility.ToJson(response));
        //});
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
