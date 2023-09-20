using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseEffect : MonoBehaviour, ISimpleObject
    {
        public void OnPoolEvent_Pop()
        {
        }

        public void OnPoolEvent_Push()
        {
            Clear();
        }

        protected abstract void onPause();
        protected abstract void onResume();
        protected abstract void onPlay();
        protected abstract void onStop();

        public virtual void onAwake() { }
        public abstract void onLateUpdate();


        Vector3 mInitScale = Vector3.one;
        float mWaitTime = -1f;
        bool mPlaying = false;

        public virtual void Clear()
        {
            transform.localScale = mInitScale;
        }

        public virtual void SetSortingOrder(int _order)
        {
        }

        public void SetDirection(bool _reversed)
        {
            if (_reversed)
            {
                transform.localScale = new Vector3(-mInitScale.x, mInitScale.y, 1);
            }
            else
            {
                transform.localScale = mInitScale;
            }
        }

        public void Play(float _waitTime)
        {
            mWaitTime = _waitTime;
            if (mWaitTime <= 0f)
            {
                mPlaying = true;
                onPlay();
            }
        }

        public void Pause()
        {
            mPlaying = false;
            onPause();
        }

        public void Resume()
        {
            mPlaying = true;
            onResume();
        }

        public void Stop()
        {
            mPlaying = false;
            onStop();
        }

        public void Complete()
        {
            EffectManager.Instance.Remove(this);
        }

        private void Awake()
        {
            mInitScale = transform.localScale;
            onAwake();
        }

        private void LateUpdate()
        {
            if (mWaitTime > 0f)
            {
                mWaitTime -= Time.deltaTime;
                if (mWaitTime <= 0f)
                {
                    mPlaying = true;
                    onPlay();
                }
            }
            if (mPlaying == false)
            {
                return;
            }
            onLateUpdate();
        }
    }

}

