using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Devarc;
using System.Text;
using MessagePack.Resolvers;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement;

public class ExampleManager : MonoBehaviour
{
    public CHARACTER_ID charID;

    public AuthNetwork authNetwork;
    public GameNetwork gameNetwork;

    public TMP_InputField authAddress;
    public TMP_InputField gameAddress;
    public TMP_InputField inputID;
    public TMP_InputField inputPW;
    public ScrollRect scrollRect;
    public TextMeshProUGUI logText;

    string sessionID = string.Empty;
    int secret = 0;

    StringBuilder mStrBuilder = new StringBuilder();
    List<string> mLogMessages = new List<string>();

    IEnumerator Start()
    {
        /*
         * Init Network
         * 
         */
        StaticCompositeResolver.Instance.Register(
            MessagePack.Resolvers.GeneratedResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance,
            MessagePack.Unity.UnityResolver.Instance
        );

        authNetwork.Init(authAddress.text, "packet", StaticCompositeResolver.Instance);
 
        gameNetwork.Init(StaticCompositeResolver.Instance);
        gameNetwork.OnOpen += onConnected;
        gameNetwork.OnClose += (sender, evt) =>
        {
            Debug.Log($"Diconnected: code={evt.Code}, reason={evt.Reason}");
        };
        gameNetwork.OnError += (sender, evt) =>
        {
            Debug.LogError(evt.Message);
            gameNetwork.DisConnect();
        };



        /*
         * Init UI
         * 
         */
        authAddress.text = "http://localhost:3000/msgpack";
        gameAddress.text = "ws://localhost:4000/Game";

        logText.text = string.Empty;
        Application.logMessageReceived += (log, stack, type) =>
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
        };

        logText.OnPreRenderText += (info) =>
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        };



        /*
         * Load Assets...
         * 
         */
        yield return AssetManager.Instance.LoadTextAssets("table");

        var textAsset = AssetManager.Instance.GetTextAsset("CHARACTER");
        Table.CHARACTER.LoadJson(textAsset.text);
    }


    public void OnClick_RequestLogin()
    {
        var request = new C2Auth.RequestLogin();
        request.accountID = inputID.text;
        request.password = EncryptUtil.Encrypt_Base64(inputPW.text);
        authNetwork.RequestLogin(request, (response, errorType, errorMsg) =>
        {
            switch (errorType)
            {
                case UnityWebRequest.Result.Success:
                    sessionID = response.sessionID;
                    secret = response.secret;
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
        if (gameNetwork.IsConnected)
            onConnected(null, null);
        else
            gameNetwork.Connect(gameAddress.text);
    }


    void onConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected...");
        C2Game.RequestLogin request = new C2Game.RequestLogin();
        request.sessionID = sessionID;
        request.secret = secret;
        gameNetwork.SendData(request);
        Debug.Log(JsonUtility.ToJson(request));
    }

}
