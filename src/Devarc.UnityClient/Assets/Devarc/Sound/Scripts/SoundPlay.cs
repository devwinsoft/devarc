using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    public enum SOUND_PLAY_STATE
    {
        STOP,
        WAIT,
        FADE_IN,
        PLAY,
        FADE_OUT,
    }

    public SOUND_PLAY_STATE State
    {
        get { return mState; }
        private set
        {
            mState = value;
            mElapsedTime = 0f;
        }
    }
    SOUND_PLAY_STATE mState = SOUND_PLAY_STATE.STOP;

    public CHANNEL Channel
    {
        get { return mChannel; }
    }
    CHANNEL mChannel;

    public int GroupID
    {
        get { return mGroupID; }
    }
    int mGroupID;

    public int SoundSEQ
    {
        get { return mSoundSEQ; }
    }
    int mSoundSEQ;

    public AudioSource mAudio;

    float mVolumn;
    float mWaitTime;
    float mFadingTime; // fade-in / fade-out time
    float mElapsedTime = 0f;

    public void Init(int _soundSEQ, CHANNEL _channel, int _ownerID, AudioClip _clip, float _volumn, bool _loop, float _waitTime, float _fadeInTime)
    {
        mSoundSEQ = _soundSEQ;
        mChannel = _channel;
        mGroupID = _ownerID;
        mVolumn = _volumn;
        mWaitTime = _waitTime;
        mFadingTime = _fadeInTime;
        mAudio.clip = _clip;
        mAudio.loop = _loop;
        mAudio.volume = _volumn;

        if (mWaitTime > 0)
        {
            State = SOUND_PLAY_STATE.WAIT;
        }
        else
        {
            play();
        }
    }

    void play()
    {
        if (mFadingTime > 0)
        {
            State = SOUND_PLAY_STATE.FADE_IN;
            mAudio.volume = 0f;
            mAudio.Play();
        }
        else
        {
            State = SOUND_PLAY_STATE.PLAY;
            mAudio.volume = mVolumn;
            mAudio.Play();
        }
    }


    public void FadeOut(float _fadeOutTime)
    {
        State = SOUND_PLAY_STATE.FADE_OUT;
        mFadingTime = _fadeOutTime;
        mAudio.volume = mVolumn;
    }

    public void Stop()
    {
        State = SOUND_PLAY_STATE.STOP;
        mSoundSEQ = 0;
        mAudio.Stop();
    }

    public bool Tick(float _deltaTime)
    {
        if (State == SOUND_PLAY_STATE.STOP)
            return true;

        mElapsedTime += _deltaTime;
        switch (State)
        {
            case SOUND_PLAY_STATE.WAIT:
                if (mWaitTime < mElapsedTime)
                {
                    play();
                }
                break;
            case SOUND_PLAY_STATE.FADE_IN:
                if (mFadingTime < mElapsedTime)
                {
                    State = SOUND_PLAY_STATE.PLAY;
                    mAudio.volume = mVolumn;
                }
                else
                {
                    mAudio.volume = mVolumn * Mathf.Min(1f, mElapsedTime / mFadingTime);
                }
                break;
            case SOUND_PLAY_STATE.FADE_OUT:
                if (mFadingTime < mElapsedTime)
                    return true;
                mAudio.volume = mVolumn * Mathf.Max(0f, 1f - mElapsedTime / mFadingTime);
                break;
            case SOUND_PLAY_STATE.PLAY:
                if (Application.isFocused)
                {
                    if (mAudio.isPlaying == false)
                    {
                        return true;
                    }
                }
                break;
            default:
                break;
        }
        return false;
    }
}

