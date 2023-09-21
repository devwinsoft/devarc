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

    public CString sessionID;
    public CInt secret;


    protected override void onAwake()
    {
        // Init debugging.
        Debugging.OnAssert += (condition, message) => { Debug.Assert(condition, message); };
        Debugging.OnLog += (message) => { Debug.Log(message); };
        Debugging.OnLogWarning += (message) => { Debug.LogWarning(message); };
        Debugging.OnLogError += (message) => { Debug.LogError(message); };
        Debugging.OnLogException += (ex) => { Debug.LogException(ex); };

        // Init network.
        StaticCompositeResolver.Instance.Register(
            MessagePack.Resolvers.GeneratedResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance,
            MessagePack.Unity.UnityResolver.Instance
        );

        mAuthNetwork = create<AuthNetwork>(transform);
        mAuthNetwork.InitProtocol("msgpack", "packet", StaticCompositeResolver.Instance);

        mGameNetwork = create<GameNetwork>(transform);
        mGameNetwork.InitProtocol("Game", StaticCompositeResolver.Instance);


        // Init download manager.
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
            StartCoroutine(loadRemoteBundles());
        };

        DownloadManager.Instance.OnError += () =>
        {
            Debug.Log($"Download failed.");
        };
    }


    IEnumerator loadRemoteBundles()
    {
        EffectManager.Instance.Clear();
        yield return EffectManager.Instance.LoadBundle("effect");
        yield return SoundManager.Instance.LoadBundle("sound");
        //yield return SoundManager.Instance.LoadBundleSounds("voice", lang);
    }


    T create<T>(Transform root) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(typeof(T).Name);
        obj.transform.parent = root;
        T compo = obj.AddComponent<T>();
        return compo;
    }
}
