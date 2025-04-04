// Copyright (c) 2021 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
//
using System.Threading;
using UnityEngine;

namespace Devarc
{
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

        public int SoundSEQ => mSoundSEQ;
        int mSoundSEQ;

        public int GroupID => mGroupID;
        int mGroupID;

        public string SoundID => mSoundID;
        string mSoundID;

        public AudioSource mAudio;

        bool mMuted;
        float mVolumn;
        float mWaitTime;
        float mFadingTime; // fade-in / fade-out time
        float mElapsedTime = 0f;

        public void Init(int _soundSEQ, string _soundID, int _group, AudioClip _clip, float _volumn, bool _loop, bool _muted, float _waitTime, float _fadingTime)
        {
            mSoundSEQ = _soundSEQ;
            mSoundID = _soundID;
            mGroupID = _group;
            mMuted = _muted;
            mVolumn = _volumn;
            mWaitTime = _waitTime;
            mFadingTime = _fadingTime;
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
                mAudio.volume = mMuted ? 0f : mVolumn;
                mAudio.Play();
            }
        }

        public void FadeOut(float _fadeOutTime)
        {
            State = SOUND_PLAY_STATE.FADE_OUT;
            mFadingTime = _fadeOutTime;
            mAudio.volume = mMuted ? 0f : mVolumn;
        }

        public void Mute(bool muted)
        {
            mMuted = muted;
            mAudio.volume = mMuted ? 0f : mVolumn;
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
                        mAudio.volume = mMuted ? 0f : mVolumn;
                    }
                    else
                    {
                        mAudio.volume = mVolumn * Mathf.Min(1f, mElapsedTime / mFadingTime);
                    }
                    break;
                case SOUND_PLAY_STATE.FADE_OUT:
                    if (mFadingTime < mElapsedTime)
                        return true;
                    if (mMuted)
                        mAudio.volume = 0f;
                    else
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
}