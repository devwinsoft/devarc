using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;

public enum CHANNEL
{
    BGM,
    EFFECT,
    UI,
    MAX,
}

public class SoundData
{
    public string addressKey;
    public string path;
    public float volume;
    public bool loop;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public SoundChannel[] Channels => mChannels;
    SoundChannel[] mChannels = new SoundChannel[(int)CHANNEL.MAX];

    Dictionary<string, List<SoundData>> mSoundDatas = new Dictionary<string, List<SoundData>>();
    Dictionary<string, AudioClip> mClips = new Dictionary<string, AudioClip>();

    protected override void onAwake()
    {
        createChannel(CHANNEL.BGM, 2);
        createChannel(CHANNEL.EFFECT, 10);
        createChannel(CHANNEL.UI, 10);
    }

    protected override void onDestroy()
    {
    }


    public IEnumerator LoadSounds_Bundle(string tableFileName, string addressKey, SystemLanguage lang = SystemLanguage.Unknown)
    {
        var textAsset = AssetManager.Instance.GetAsset<TextAsset>(tableFileName);
        if (textAsset != null)
        {
            Table.SOUND.LoadJson(textAsset.text, (data) =>
            {
                register(data, addressKey);
            });
        }

        // Load AudioClips...
        if (lang == SystemLanguage.Unknown)
            yield return AssetManager.Instance.LoadAssets_Bundle<AudioClip>(addressKey);
        else
            yield return AssetManager.Instance.LoadAssets_Bundle<AudioClip>(addressKey, lang);
    }


    public void UnloadSounds_Bundle(string addressKey)
    {
        AssetManager.Instance.UnloadAssets_Bundle(addressKey);
        unRegister(addressKey);
    }


    public void LoadSounds_Resource(string tableFileName)
    {
        var tableAsset = AssetManager.Instance.GetAsset<TextAsset>(tableFileName);
        if (tableAsset != null)
        {
            Table.SOUND.LoadJson(tableAsset.text, (data) =>
            {
                register(data, null);

                // Load AudioClips...
                AssetManager.Instance.LoadAsset_Resource<AudioClip>(data.path);
            });
        }
    }


    void register(SOUND data, string addressKey)
    {
        List<SoundData> list;
        if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
        {
            list = new List<SoundData>();
            mSoundDatas.Add(data.sound_id, list);
        }
        SoundData obj = new SoundData();
        obj.addressKey = addressKey;
        obj.path = data.path;
        obj.volume = data.volume;
        obj.loop = data.loop;
        list.Add(obj);
    }


    void unRegister(string addressKey)
    {
        foreach (var list in mSoundDatas.Values)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].addressKey == addressKey)
                    list.RemoveAt(i);
            }
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
