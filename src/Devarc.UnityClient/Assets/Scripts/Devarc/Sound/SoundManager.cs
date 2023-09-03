using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;
using UnityEngine.AI;

public enum SOUND_CHANNEL
{
    BGM,
    EFFECT,
    UI,
    MAX,
}

[System.Serializable]
public class SoundData
{
    public SOUND_ID soundID;
    public float waitTime;
    public float fadeTime;
}

public class SoundManager : SingletonManager<SoundManager>
{
    Dictionary<string, List<SOUND>> mSoundDatas = new Dictionary<string, List<SOUND>>();
    SoundChannel[] mChannels = new SoundChannel[(int)SOUND_CHANNEL.MAX];
    Dictionary<string, AudioClip> mClips = new Dictionary<string, AudioClip>();

    protected override void onAwake()
    {
        createChannel(SOUND_CHANNEL.BGM, 2);
        createChannel(SOUND_CHANNEL.EFFECT, 10);
        createChannel(SOUND_CHANNEL.UI, 10);
    }

    protected override void onDestroy()
    {
    }


    IEnumerator LoadAsync()
    {
        yield return AssetManager.Instance.LoadAudioClip("sound");

        AssetManager.LoadAssetAtPath<AudioClip>("");

        foreach (var data in Table.SOUND.List)
        {
            List<SOUND> list;
            if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
            {
                list = new List<SOUND>();
                mSoundDatas.Add(data.sound_id, list);
            }
            list.Add(data);
        }
    }

    public bool IsPlaying(SOUND_CHANNEL channel)
    {
        return mChannels[(int)channel].IsPlaying;
    }

    public int PlaySound(SOUND_CHANNEL channel, string soundID)
    {
        return PlaySound(channel, soundID, 0, 0f);
    }

    public int PlaySound(SOUND_CHANNEL channel, string soundID, int groupID, float fadeIn)
    {
        var soundData = Table.SOUND.Get(soundID);
        if (soundData == null)
        {
            Debug.LogError($"[SoundManager] Cannot find sound_id: {soundID}");
            return 0;
        }

        AudioClip clip = getClip(soundData.path, soundData.bundle);
        if (clip == null)
        {
            return 0;
        }
        var chennel = mChannels[(int)channel];
        return chennel.Play(groupID, clip, soundData.volume, soundData.loop, 0f, fadeIn);
    }


    public void FadeOut(SOUND_CHANNEL channel, float _fadeOutTime)
    {
        mChannels[(int)channel].FadeOutAll(_fadeOutTime);
    }

    public void FadeOutGroup(SOUND_CHANNEL channel, int groupID, float _fadeOutTime)
    {
        mChannels[(int)channel].FadeOutGroup(groupID, _fadeOutTime);
    }

    public void Stop(SOUND_CHANNEL channel, int _soundSEQ)
    {
        mChannels[(int)channel].Stop(_soundSEQ);
    }

    public void StopGroup(SOUND_CHANNEL channel, int _soundSEQ)
    {
        mChannels[(int)channel].StopGroup(_soundSEQ);
    }


    void createChannel(SOUND_CHANNEL _channel, int _pool)
    {
        GameObject obj = new GameObject(_channel.ToString());
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        SoundChannel compo = obj.AddComponent<SoundChannel>();
        compo.Init(_channel, _pool);
        mChannels[(int)_channel] = compo;
    }


    AudioClip getClip(string path, bool bundle)
    {
        AudioClip clip = null;
        if (bundle)
        {
            clip = AssetManager.LoadAssetAtPath<AudioClip>(path);
        }
        else
        {
        }

        if (mClips.TryGetValue(path, out clip) == false)
        {
            clip = Resources.Load<AudioClip>(path);
            if (clip != null)
            {
                mClips.Add(path, clip);
            }
        }
        return clip;
    }

}
