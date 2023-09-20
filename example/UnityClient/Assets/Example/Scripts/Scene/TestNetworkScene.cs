using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
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
        Debug.Log("TestNetworkScene::OnEnterScene");

        AppManager.authNetwork.InitConnection(domains.captionText.text, 3000);



        yield return null;
    }


    public override void OnLeaveScene()
    {
    }


    public void OnClick_RequestLogin()
    {
        var request = new C2Auth.RequestLogin();
        request.accountID = inputID.text;
        request.password = EncryptUtil.Encrypt_Base64(inputPW.text);

        AppManager.authNetwork.InitConnection(domains.captionText.text, 3000);
        AppManager.authNetwork.RequestLogin(request, (response, errorType, errorMsg) =>
        {
            switch (errorType)
            {
                case UnityWebRequest.Result.Success:
                    AppManager.Instance.sessionID = response.sessionID;
                    AppManager.Instance.secret = response.secret;
                    Debug.Log(JsonUtility.ToJson(response));
                    break;
                default:
                    Debug.LogError(errorMsg);
                    break;
            }
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
