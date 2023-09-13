using Devarc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundChannel : MonoBehaviour
{
    CHANNEL mChannel;
    int mNextSEQ = 1;
    List<SoundPlay> mPool = new List<SoundPlay>();
    List<SoundPlay> mPlayList = new List<SoundPlay>();
    Dictionary<int, SoundPlay> mPlayDict = new Dictionary<int, SoundPlay>();

    public bool IsPlaying
    {
        get
        {
            foreach (var play in mPlayList)
            {
                switch (play.State)
                {
                    case SoundPlay.SOUND_PLAY_STATE.PLAY:
                    case SoundPlay.SOUND_PLAY_STATE.WAIT:
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }
    }

    public void Clear()
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay data = mPlayList[i];
            data.mAudio.Stop();
            Destroy(data.gameObject);
        }
        mPlayList.Clear();
        mPlayDict.Clear();
    }

    public void Init(CHANNEL _channel, int _playCount)
    {
        mChannel = _channel;
        mNextSEQ = (int)mChannel * 1000;

        for (int i = 0; i < _playCount; i++)
        {
            GameObject obj = new GameObject("SoundPlay");
            SoundPlay compo = obj.AddComponent<SoundPlay>();
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            compo.mAudio = obj.AddComponent<AudioSource>();
            compo.mAudio.playOnAwake = false;
            compo.mAudio.rolloffMode = AudioRolloffMode.Linear;
            mPool.Add(compo);
        }
    }


    public int Play(int groupID, AudioClip clip, float volumn, bool loop, float wait, float fadeIn)
    {
        if (groupID != 0)
        {
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay playData = mPlayList[i];
                if (playData.GroupID == groupID
                    && playData.mAudio.clip != null
                    && playData.mAudio.isPlaying
                    && string.Equals(playData.mAudio.clip.name, clip.name))
                {
                    stop(playData);
                    break;
                }
            }
        }

        SoundPlay obj = null;
        if (mPool.Count > 0)
        {
            obj = mPool[0];
            mPool.RemoveAt(0);
        }
        else
        {
            obj = mPlayList[0];
            stop(obj);
            if (mPool.Count > 0)
            {
                obj = mPool[0];
                mPool.RemoveAt(0);
            }
        }
        obj.Init(generateSoundSEQ(), mChannel, groupID, clip, volumn, loop, wait, fadeIn);
        mPlayList.Add(obj);
        mPlayDict.Add(obj.SoundSEQ, obj);
        return obj.SoundSEQ;
    }

    public void FadeOutAll(float fadeOut)
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (fadeOut <= 0f)
            {
                stop(sound);
            }
            else
            {
                sound.FadeOut(fadeOut);
            }
        }
    }

    public void FadeOutGroup(int groupID, float _fadeOut)
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (sound.GroupID != groupID)
                continue;
            if (_fadeOut <= 0f)
            {
                stop(sound);
            }
            else
            {
                sound.FadeOut(_fadeOut);
            }
        }
    }


    public void Stop(int _soundSEQ)
    {
        SoundPlay obj;
        if (mPlayDict.TryGetValue(_soundSEQ, out obj))
        {
            stop(obj);
        }
    }


    public void StopGroup(int groupID)
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (sound.GroupID != groupID)
                continue;
            stop(sound);
        }
    }


    private void Update()
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (sound.Tick(Time.unscaledDeltaTime))
            {
                stop(sound);
            }
        }
    }

    int generateSoundSEQ()
    {
        do
        {
            mNextSEQ++;
            if (mNextSEQ >= 10000)
            {
                mNextSEQ = (int)mChannel * 1000;
            }
        } while (mPlayDict.ContainsKey(mNextSEQ));
        return mNextSEQ;
    }

    void stop(SoundPlay _play)
    {
        int soundSEQ = _play.SoundSEQ;
        _play.Stop();

        mPlayList.Remove(_play);
        mPlayDict.Remove(soundSEQ);
        mPool.Add(_play);
    }
}
