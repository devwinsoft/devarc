using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Devarc;


public class TestNetworkScene : BaseScene
{
    public UIDebugLog debugLog;
    public TMP_Dropdown domains;
    public TMP_InputField inputID;
    public TMP_InputField inputPW;

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

        OnChangeNetwork();
    }


    public override void OnLeaveScene()
    {
    }


    public void OnChangeNetwork()
    {
        AppManager.authNetwork.InitConnection(domains.captionText.text, 3000);

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
                    break;
            }
            Debug.Log(JsonUtility.ToJson(response));
        });
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
            AppManager.gameNetwork.Connect(domains.captionText.text, 4000);
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
