using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Devarc;


public class TestNetworkScene : BaseScene
{
    public TMP_Dropdown domains;
    public TMP_InputField inputID;
    public TMP_InputField inputPW;
    public ScrollRect scrollRect;
    public TextMeshProUGUI logText;

    StringBuilder mStrBuilder = new StringBuilder();
    List<string> mLogMessages = new List<string>();


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

        Application.logMessageReceived += LogCallback;
        logText.text = string.Empty;
        logText.OnPreRenderText += (info) =>
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        };

        yield return null;
    }


    public override void OnLeaveScene()
    {
        Application.logMessageReceived -= LogCallback;
    }


    void LogCallback(string log, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                mLogMessages.Add($"<color=red>[{DateTime.Now.ToString("HH:mm:ss")}] {log}</color>");
                break;
            case LogType.Warning:
                mLogMessages.Add($"<color=yellow>[{DateTime.Now.ToString("HH:mm:ss")}] {log}</color>");
                break;
            default:
                mLogMessages.Add($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}");
                break;
        }

        if (mLogMessages.Count > 50)
        {
            mLogMessages.RemoveAt(0);
        }

        mStrBuilder.Clear();
        foreach (string msg in mLogMessages)
        {
            mStrBuilder.AppendLine(msg);
        }
        logText.text = mStrBuilder.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(logText.GetComponent<RectTransform>());
    }



    public void OnClick_Clear()
    {
        mStrBuilder.Clear();
        mLogMessages.Clear();
        logText.text = string.Empty;
        LayoutRebuilder.ForceRebuildLayoutImmediate(logText.GetComponent<RectTransform>());
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


    public void OnClick_Test2()
    {
        SceneTransManager.Instance.LoadScene("TestAssetScene");
    }

}
