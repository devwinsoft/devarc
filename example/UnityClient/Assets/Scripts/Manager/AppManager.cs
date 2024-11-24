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
        SoundManager.Create();
        TableManager.Create();
        UIManager.Create("UIManager");

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


        // Initialize DownloadManager.
        DownloadManager.Instance.AddToPatchList("effect");
        DownloadManager.Instance.AddToPatchList("sound");

        DownloadManager.Instance.OnError += () =>
        {
            Debug.Log($"Download failed.");
        };
    }


    T create<T>(Transform root) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(typeof(T).Name);
        obj.transform.parent = root;
        T compo = obj.AddComponent<T>();
        return compo;
    }

}

