using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Devarc;

public enum CHANNEL
{
    BGM,
    EFFECT,
    UI,
    MAX,
}

public abstract class SoundData
{
    public abstract string sound_id { get; }
    public abstract string key { get; }
    public abstract string path { get; }
    public abstract float volume { get; }
    public abstract bool loop { get; }
    public abstract bool isBundle { get; }
}

public class BundleSoundData : SoundData
{
    public string addressKey;
    public SOUND_BUNDLE data;

    public override string sound_id => data.sound_id;
    public override string key => addressKey;
    public override string path => data.path;
    public override float volume => data.volume;
    public override bool loop => data.loop;
    public override bool isBundle => true;
}

public class ResourceSoundData : SoundData
{
    public SOUND_RESOURCE data;

    public override string sound_id => data.sound_id;
    public override string key => data.key;
    public override string path => data.path;
    public override float volume => data.volume;
    public override bool loop => data.loop;
    public override bool isBundle => false;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public SoundChannel[] Channels => mChannels;
    SoundChannel[] mChannels = new SoundChannel[(int)CHANNEL.MAX];

    Dictionary<string, List<SoundData>> mSoundDatas = new Dictionary<string, List<SoundData>>();

    protected override void onAwake()
    {
        createChannel(CHANNEL.BGM, 2);
        createChannel(CHANNEL.EFFECT, 10);
        createChannel(CHANNEL.UI, 10);
    }

    protected override void onDestroy()
    {
    }

    public void LoadResource(string key = null)
    {
        foreach (var data in Table.SOUND_RESOURCE.List)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(data.key))
            {
                // Load global sounds.
                register(data);
                AssetManager.Instance.LoadResourceAsset<AudioClip>(data.path);
            }
            else if (key == data.key)
            {
                // Load specific sounds.
                register(data);
                AssetManager.Instance.LoadResourceAsset<AudioClip>(data.path);
            }
        }
    }


    public void UnloadResource(string key = null)
    {
        foreach (var data in Table.SOUND_RESOURCE.List)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(data.key))
            {
                AssetManager.Instance.UnloadResourceAsset<AudioClip>(data.path);
            }
            else if (key == data.key)
            {
                AssetManager.Instance.UnloadResourceAsset<AudioClip>(data.path);
            }
        }
        unRegister(key, false);
    }


    public IEnumerator LoadBundle(string addressKey, SystemLanguage lang = SystemLanguage.Unknown)
    {
        foreach (var data in Table.SOUND_BUNDLE.List)
        {
            register(data, addressKey);
        }

        if (lang == SystemLanguage.Unknown)
        {
            var handle = AssetManager.Instance.LoadBundleAssets<AudioClip>(addressKey);
            yield return handle;
        }
        else
        {
            var handle = AssetManager.Instance.LoadBundleAssets<AudioClip>(addressKey, lang);
            yield return handle;
        }
    }


    public void UnloadBundle(string key)
    {
        AssetManager.Instance.UnloadBundleAssets(key);
        unRegister(key, true);
    }


    void register(SOUND_BUNDLE data, string addressKey)
    {
        List<SoundData> list;
        if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
        {
            list = new List<SoundData>();
            mSoundDatas.Add(data.sound_id, list);
        }
        var obj = new BundleSoundData();
        obj.addressKey = addressKey;
        obj.data = data;
        list.Add(obj);
    }

    void register(SOUND_RESOURCE data)
    {
        List<SoundData> list;
        if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
        {
            list = new List<SoundData>();
            mSoundDatas.Add(data.sound_id, list);
        }
        var obj = new ResourceSoundData();
        obj.data = data;
        list.Add(obj);
    }


    void unRegister(string addressKey, bool isBundle)
    {
        List<string> removeKeys = new List<string>();
        foreach (var temp in mSoundDatas)
        {
            var list = temp.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var obj = list[i];
                if (obj.isBundle != isBundle)
                    continue;
                if (obj.key != addressKey)
                    continue;

                list.RemoveAt(i);
            }
            if (list.Count == 0)
            {
                removeKeys.Add(temp.Key);
            }
        }
        foreach (var key in removeKeys)
        {
            mSoundDatas.Remove(key);
        }
    }


    public bool IsPlaying(CHANNEL channel)
    {
        return mChannels[(int)channel].IsPlaying;
    }

    public int PlaySound(CHANNEL channel, string soundID)
    {
        return PlaySound(channel, soundID, 0, 0f);
    }

    public int PlaySound(CHANNEL channel, string soundID, int groupID, float fadeIn)
    {
        List<SoundData> list = null;
        if (mSoundDatas.TryGetValue(soundID, out list) == false || list.Count == 0)
        {
            Debug.LogError($"[SoundManager] Cannot find sound_id: {soundID}");
            return 0;
        }

        var soundData = list[Random.Range(0, list.Count - 1)];
        var clip = AssetManager.Instance.GetAsset<AudioClip>(soundData.path);
        if (clip == null)
        {
            Debug.LogError($"[SoundManager] Cannot find audio clip: sound_id={soundID}, path={soundData.path}");
            return 0;
        }
        var chennel = mChannels[(int)channel];
        return chennel.Play(groupID, clip, soundData.volume, soundData.loop, 0f, fadeIn);
    }


    public void FadeOut(CHANNEL channel, float _fadeOutTime)
    {
        mChannels[(int)channel].FadeOutAll(_fadeOutTime);
    }

    public void FadeOutGroup(CHANNEL channel, int groupID, float _fadeOutTime)
    {
        mChannels[(int)channel].FadeOutGroup(groupID, _fadeOutTime);
    }

    public void Stop(CHANNEL channel, int _soundSEQ)
    {
        mChannels[(int)channel].Stop(_soundSEQ);
    }

    public void StopGroup(CHANNEL channel, int _soundSEQ)
    {
        mChannels[(int)channel].StopGroup(_soundSEQ);
    }


    void createChannel(CHANNEL _channel, int _pool)
    {
        GameObject obj = new GameObject(_channel.ToString());
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        SoundChannel compo = obj.AddComponent<SoundChannel>();
        compo.Init(_channel, _pool);
        mChannels[(int)_channel] = compo;
    }

}
