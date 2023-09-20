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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum ANIM_PLAY_COUNT
{
    Loop,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten
}

[System.Serializable]
public class AnimData
{
    public AnimationClip Clip;
    public float Speed = 1f;
    public ANIM_PLAY_COUNT Repeat;
    public float FadeTime;
}

[System.Serializable]
public class SimpleAnimList
{
    public AnimData[] list;
    public bool IsValid()
    {
        if (list == null || list.Length == 0)
            return false;
        return true;
    }
}


public class SimpleAnimator : MonoBehaviour
{
    public event System.Action OnAnimComplete;

    const float Epsilon = 0.001f;

    public float PlaySpeed
    {
        get { return mPlaySpeed; }
        set
        {
            mPlaySpeed = value;
            if (mPlayDatas != null && mPlayDatas.Length > 0)
            {
                animator.speed = mPlaySpeed * mPlayDatas[mPlayIndex].Speed;
            }
        }
    }
    float mPlaySpeed = 1f;

    public bool IsPlaying
    {
        get
        {
            if (mPlayDatas == null)
                return false;
            if (mPlayIndex < mPlayDatas.Length)
                return true;
            return false;
        }
    }


    public Animator animator
    {
        get
        {
            if (mAnimator == null)
                mAnimator = GetComponent<Animator>();
            if (mAnimator == null)
                Debug.LogError($"Animator is not attached: gameObject={gameObject.name}");
            return mAnimator;
        }
    }
    Animator mAnimator;

    public bool IsCompleted
    {
        get
        {
            if (mCompleted == false)
                return false;

            if (mPlayDatas != null && mPlayIndex < mPlayDatas.Length)
            {
                AnimData playData = mPlayDatas[mPlayIndex];
                if (playData.Repeat == ANIM_PLAY_COUNT.Loop)
                    return false;
            }
            return true;
        }
    }
    bool mCompleted = false;
    bool mPaused = false;

    System.Action mCallback = null;
    AnimData[] mPlayDatas;
    string mCurrentAnimationName = string.Empty;
    int mPlayIndex;
    int mRepeatIndex;

    public void Clear()
    {
        mPaused = false;
        mPlaySpeed = 1f;
        mCurrentAnimationName = string.Empty;
        mCallback = null;
    }

    private void Awake()
    {
        mCompleted = true;
        if (animator.runtimeAnimatorController != null)
        {
            foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
            {
                bool skip = false;
                foreach (var temp in ac.events)
                {
                    if (temp.functionName == "onAnimationComplete")
                        skip = true;
                }
                if (skip)
                    continue;
                AnimationEvent evt = new AnimationEvent();
                evt.functionName = "onAnimationComplete";
                evt.stringParameter = ac.name;
                evt.time = ac.length;
                ac.AddEvent(evt);
            }
        }
    }

    void onAnimationComplete(string _name)
    {
        if (string.Equals(mCurrentAnimationName, _name))
        {
            mCompleted = true;
        }
    }

    public void Pause(bool _value)
    {
        mPaused = _value;
        if (_value)
        {
            animator.speed = Epsilon;
        }
        else
        {
            if (mPlayDatas != null && mPlayIndex < mPlayDatas.Length)
            {
                animator.speed = mPlaySpeed * mPlayDatas[mPlayIndex].Speed;
            }
            else
            {
                animator.speed = mPlaySpeed;
            }
        }
    }

    public void Stop()
    {
        if (mCallback != null)
        {
            mCallback.Invoke();
            mCallback = null;
        }
        animator.speed = 0;
        mCompleted = true;
        mPaused = false;
    }

    public void PlayAnimation(SimpleAnimList _data)
    {
        PlayAnimation(_data.list, mPlaySpeed, null);
    }

    public void PlayAnimation(AnimData[] _playDatas)
    {
        PlayAnimation(_playDatas, mPlaySpeed, null);
    }

    public void PlayAnimation(AnimData[] _playDatas, float _playSpeed)
    {
        PlayAnimation(_playDatas, _playSpeed, 0, null);
    }

    public void PlayAnimation(AnimData[] _playDatas, System.Action _callback)
    {
        PlayAnimation(_playDatas, mPlaySpeed, 0, _callback);
    }

    public void PlayAnimation(AnimData[] _playDatas, float _playSpeed, System.Action _callback)
    {
        PlayAnimation(_playDatas, _playSpeed, 0, _callback);
    }

    public void PlayAnimation(AnimData[] _playDatas, float _playSpeed, int _playIndex, System.Action _callback)
    {
        if (_playDatas == null || _playDatas.Length == 0)
        {
            //Debug.LogErrorFormat("[AnimController] Animation play data is empty.");
            if (mCallback != null)
            {
                mCallback.Invoke();
            }
            return;
        }

        if (mCallback != null)
        {
            var func = mCallback;
            mCallback = null;
            func.Invoke();
        }

        mPlayDatas = _playDatas;
        mCallback = _callback;
        mPlaySpeed = _playSpeed;
        playAnimation(_playIndex, 0);
    }

    private bool playAnimation(int _playIndex, int _repeatIndex)
    {
        if (mPlayDatas == null || _playIndex >= mPlayDatas.Length)
        {
            mPlayIndex = 0;
            mRepeatIndex = 0;
            return false;
        }

        mCompleted = false;
        mPlayIndex = _playIndex;
        mRepeatIndex = _repeatIndex;
        AnimData _playData = mPlayDatas[_playIndex];
        if (_playData.Clip == null)
        {
            Debug.LogErrorFormat("[SimpleAnimator] Animation clip is null: index={0}", _playIndex);
            return false;
        }
        if (string.Equals(mCurrentAnimationName, _playData.Clip))
        {
            animator.Rebind();
        }
        mCurrentAnimationName = _playData.Clip.name;

        if (mPaused)
            animator.speed = Epsilon;
        else
            animator.speed = mPlaySpeed * _playData.Speed;

        if (_playData.FadeTime <= 0f)
        {
            animator.Play(_playData.Clip.name, 0, 0f);
            for (int i = 1; i < animator.layerCount; i++)
            {
                animator.Play(string.Format("{0} ({1})", _playData.Clip.name, animator.GetLayerName(i)), i, 0f);
            }
        }
        else
        {
            animator.CrossFade(_playData.Clip.name, _playData.FadeTime, 0, 0f);
            for (int i = 1; i < animator.layerCount; i++)
            {
                animator.CrossFade(string.Format("{0} ({1})", _playData.Clip.name, animator.GetLayerName(i)), _playData.FadeTime, i, 0f);
            }
        }
        return true;
    }

    void LateUpdate()
    {
        float _deltaTime = Time.deltaTime;

        if (mPlayDatas == null || mPlayIndex >= mPlayDatas.Length)
        {
            return;
        }

        AnimData playData = mPlayDatas[mPlayIndex];
        if (mCompleted == false)
        {
            // wait
        }
        else if (playData.Repeat == ANIM_PLAY_COUNT.Loop)
        {
            //OnAnimComplete?.Invoke();
        }
        else if (mRepeatIndex + 1 < (int)playData.Repeat)
        {
            playAnimation(mPlayIndex, mRepeatIndex + 1);
        }
        else if (mPlayIndex < mPlayDatas.Length - 1)
        {
            playAnimation(mPlayIndex + 1, 0);
        }
        else
        {
            animator.speed = 0f;
            if (mCallback != null)
            {
                mCallback.Invoke();
                mCallback = null;
            }
            OnAnimComplete?.Invoke();
        }
    }


    public static float GetAnimationLength(AnimData[] _playDatas)
    {
        if (_playDatas == null || _playDatas.Length == 0)
        {
            return 0f;
        }
        float value = 0f;
        for (int i = 0; i < _playDatas.Length; i++)
        {
            var temp = _playDatas[i];
            if (temp.Clip == null)
            {
                return 0f;
            }
            if (temp.Repeat == ANIM_PLAY_COUNT.Loop)
            {
                value += temp.Clip.length / temp.Speed;
            }
            else if (temp.Speed > 0)
            {
                value += (int)temp.Repeat * temp.Clip.length / temp.Speed;
            }
            else
            {
                value += (int)temp.Repeat * temp.Clip.length;
            }
        }
        return value;
    }

}


public abstract class SimpleAnimator<EVT> : SimpleAnimator where EVT : System.Enum
{
    public delegate void CALLBACK_ANIM_EVENT(EVT _eventType);
    public void onAnimEvent(EVT _eventType)
    {
        OnAnimEvent?.Invoke(_eventType);
    }

    public event System.Action<EVT> OnAnimEvent;
}