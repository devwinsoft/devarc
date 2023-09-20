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


    public void LoadResources(SystemLanguage lang)
    {
        AssetManager.Instance.LoadResourceAssets<TextAsset>("Tables");
        AssetManager.Instance.LoadResourceAssets<TextAsset>("LStrings", lang);

        SoundManager.Instance.LoadResourceSounds();
    }


    public void UnloadResources()
    {
        AssetManager.Instance.UnloadResourceAssets<TextAsset>("Tables");
        AssetManager.Instance.UnloadResourceAssets<TextAsset>("LStrings");

        SoundManager.Instance.UnloadResourceSounds();
    }


    public IEnumerator LoadBundles(SystemLanguage lang)
    {
        {
            var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>("table");
            yield return handle;
            if (handle.IsValid())
            {
                Table.CHARACTER.LoadFromFile("CHARACTER");
                Table.SKILL.LoadFromFile("SKILL");
                Table.SOUND_BUNDLE.LoadFromFile("SOUND_BUNDLE");
            }
        }

        {
            var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>("lstring", lang);
            yield return handle;
            if (handle.IsValid())
            {
                Table.LString.LoadFromFile("LString");
            }
        }

        yield return SoundManager.Instance.LoadBundleSounds("sound");
    }


    public void UnloadBundles()
    {
        AssetManager.Instance.UnloadBundleAssets("table");
        Table.CHARACTER.Clear();
        Table.SKILL.Clear();
        Table.SOUND_BUNDLE.Clear();
        Table.SOUND_RESOURCE.Clear();

        AssetManager.Instance.UnloadBundleAssets("lstring");
        Table.LString.Clear();

        AssetManager.Instance.UnloadBundleAssets("effect");
        SoundManager.Instance.UnloadBundleSounds("sound");
    }
}
