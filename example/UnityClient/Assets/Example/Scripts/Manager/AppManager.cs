using System.Collections;
using UnityEngine;
using MessagePack.Resolvers;
using Devarc;

public class AppManager : MonoSingleton<AppManager>
{
    public static AuthNetwork authNetwork => Instance.mAuthNetwork;
    AuthNetwork mAuthNetwork;

    public static GameNetwork gameNetwork => Instance.mGameNetwork;
    GameNetwork mGameNetwork;


    public CString Token;
    public CString Error;

    public CString sessionID;
    public CInt secret;


    protected override void onAwake()
    {
        // Create managers...
        AssetManager.Create();
        DownloadManager.Create();
        EffectManager.Create();
        LoginManager.Create();
        SoundManager.Create();
        TableManager.Create();

        // Initialize TableManager,
        TableManager.Instance.OnError += (errorType, args) =>
        {
            Debug.Log(errorType);
        };

        // Initialize Debugging.
        Debugging.OnAssert += (condition, message) => { Debug.Assert(condition, message); };
        Debugging.OnLog += (message) => { Debug.Log(message); };
        Debugging.OnLogWarning += (message) => { Debug.LogWarning(message); };
        Debugging.OnLogError += (message) => { Debug.LogError(message); };
        Debugging.OnLogException += (ex) => { Debug.LogException(ex); };

        // Initialize network.
        mAuthNetwork = create<AuthNetwork>(transform);
        mAuthNetwork.OnError += (errorType, errorMsg) => { Debug.LogError(errorMsg); };

        mGameNetwork = create<GameNetwork>(transform);
        mGameNetwork.InitProtocol(StaticCompositeResolver.Instance);


        // Init LoginManager
        LoginManager.Instance.OnSignIn += (loginType, firstInit) =>
        {
            Debug.Log($"Google SignIn: LoginType={loginType}, firstInit={firstInit}");
        };
        LoginManager.Instance.OnSignOut += (success) =>
        {
            Debug.Log($"SignOut: success={success}");
        };


        // Initialize DownloadManager.
        DownloadManager.Instance.AddToPatchList("effect");
        DownloadManager.Instance.AddToPatchList("sound");


        DownloadManager.Instance.OnPatch += (info) =>
        {
            Debug.LogFormat("Start to download contents: {0:N0} kb", (float)(info.totalSize / 1000f));
            DownloadManager.Instance.BeginDownload();
        };

        DownloadManager.Instance.OnProgress += (progress) =>
        {
        };

        DownloadManager.Instance.OnResult += () =>
        {
            Debug.Log($"Download completed.");
            StartCoroutine(LoadBundles());
        };

        DownloadManager.Instance.OnError += () =>
        {
            Debug.Log($"Download failed.");
        };
    }



    public IEnumerator LoadBundles()
    {
#if UNITY_EDITOR
        yield return TableManager.Instance.LoadBundleTable("table-json", TableFormatType.JSON);
        yield return TableManager.Instance.LoadBundleString("lstring-json", TableFormatType.JSON, SystemLanguage.English);
#else
        yield return TableManager.Instance.LoadBundleTable("table-bin", TableFormatType.BIN);
        yield return TableManager.Instance.LoadBundleString("lstring-bin", TableFormatType.BIN, SystemLanguage.English);
#endif

        yield return EffectManager.Instance.LoadBundle("effect");
        yield return SoundManager.Instance.LoadBundle("sound");
        //yield return SoundManager.Instance.LoadBundleSounds("voice", SystemLanguage.English);

        foreach (var data in Table.CHARACTER.List)
        {
            Debug.Log(JsonUtility.ToJson(data));
        }

        foreach (var data in Table.SKILL.List)
        {
            Debug.Log(JsonUtility.ToJson(data));
        }

        foreach (var data in Table.SOUND_BUNDLE.List)
        {
            Debug.Log(JsonUtility.ToJson(data));
        }
    }


    public void UnloadBundles()
    {
#if UNITY_EDITOR
        TableManager.Instance.UnloadBundleTable("table-json");
        TableManager.Instance.UnloadBundleString("lstring-json");
#else
        TableManager.Instance.UnloadBundleTable("table-bin");
        TableManager.Instance.UnloadBundleString("lstring-bin");
#endif

        EffectManager.Instance.UnloadBundle("effect");
        SoundManager.Instance.UnloadBundle("sound");
    }


    T create<T>(Transform root) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(typeof(T).Name);
        obj.transform.parent = root;
        T compo = obj.AddComponent<T>();
        return compo;
    }

}

