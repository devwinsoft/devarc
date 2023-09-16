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
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class ExampleManager : MonoBehaviour
{
    public CHARACTER_ID charID;
    public SKILL_ID skillID;
    public SOUND_ID soundID;
    public EFFECT_ID effectID;

    public AuthNetwork authNetwork;
    public GameNetwork gameNetwork;

    public TMP_Dropdown domains;
    public TMP_InputField inputID;
    public TMP_InputField inputPW;
    public ScrollRect scrollRect;
    public TextMeshProUGUI logText;

    string sessionID = string.Empty;
    int secret = 0;

    StringBuilder mStrBuilder = new StringBuilder();
    List<string> mLogMessages = new List<string>();

    string gameAddress => $"ws://{domains.captionText.text}:4000/Game";


    IEnumerator Start()
    {
        initDebugging();
        initNetwork();
        yield return loadAssets();
    }


    void initDebugging()
    {
        Debugging.OnAssert += (condition, message) => { Debug.Assert(condition, message); };
        Debugging.OnLog += (message) => { Debug.Log(message); };
        Debugging.OnLogWarning += (message) => { Debug.LogWarning(message); };
        Debugging.OnLogError += (message) => { Debug.LogError(message); };
        Debugging.OnLogException += (ex) => { Debug.LogException(ex); };

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

        logText.text = string.Empty;
        logText.OnPreRenderText += (info) =>
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        };
    }



    void initNetwork()
    {
        StaticCompositeResolver.Instance.Register(
            MessagePack.Resolvers.GeneratedResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance,
            MessagePack.Unity.UnityResolver.Instance
        );

        authNetwork.Init(domains.captionText.text, 3000, "msgpack", "packet", StaticCompositeResolver.Instance);

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
    }


    IEnumerator loadAssets()
    {
        // Load Resources...
        AssetManager.Instance.LoadAssets_Resource<TextAsset>("Tables");
        SoundManager.Instance.LoadResourceSounds("SOUND@builtin");

        // Load Local Bundle Tables...
        yield return AssetManager.Instance.LoadAssets_Bundle<TextAsset>("table");
        Table.CHARACTER.LoadFromFile("CHARACTER");
        Table.SKILL.LoadFromFile("SKILL");

        // Load Sound Table & AudioClips...
        yield return SoundManager.Instance.LoadBundleSounds("SOUND", "sound");
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

        authNetwork.Domain = domains.captionText.text;
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
        {
            Debug.LogWarning("Already connected.");
        }
        else
        {
            gameNetwork.Connect(gameAddress);
        }
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


    public void OnClick_Test1()
    {
        gameNetwork.DisConnect();
    }


    IEnumerator download()
    {
        long totalSize = 0;
        Dictionary<string, long> patchList = null;
        yield return DownloadManager.Instance.GetPatchList((_size, _list) =>
        {
            totalSize = _size;
            patchList = _list;
        });

        Debug.LogFormat("Start to download contents: {0:N0} kb", (float)totalSize / 1000f);
        bool success = false;
        yield return DownloadManager.Instance.Download(patchList, (result, process) =>
        {
            success = (result == AsyncOperationStatus.Succeeded);
        });

        Debug.Log($"Download completed: success={success}");
        if (success)
        {
            yield return AssetManager.Instance.LoadPrefabs_Bundle("effect");
        }
    }


    public void OnClick_Test2()
    {
        SoundManager.Instance.PlaySound(CHANNEL.EFFECT, soundID);
    }

}
