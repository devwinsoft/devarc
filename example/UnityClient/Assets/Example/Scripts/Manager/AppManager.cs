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
        if (!AppManager.IsCreated())
        {
            AppManager.Create("AppManager");
        }

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

        mAuthNetwork = Create<AuthNetwork>();
        mAuthNetwork.InitProtocol("msgpack", "packet", StaticCompositeResolver.Instance);

        mGameNetwork = Create<GameNetwork>();
        mGameNetwork.InitProtocol("Game", StaticCompositeResolver.Instance);
    }


    T Create<T>() where T : MonoBehaviour
    {
        GameObject obj = new GameObject(typeof(T).Name);
        T compo = obj.AddComponent<T>();
        return compo;
    }
}
