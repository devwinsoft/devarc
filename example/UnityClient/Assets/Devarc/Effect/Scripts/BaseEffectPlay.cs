using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseEffectPlay : MonoBehaviour
    {
        public event System.Action<BaseEffectPlay> OnRemove;

        protected abstract void onPause();
        protected abstract void onResume();
        protected abstract void onPlay();
        protected abstract void onStop();

        public virtual void onAwake() { }
        public abstract void onLateUpdate();

        protected enum STATE
        {
            Init,
            Playing,
            Paused,
            Stopped,
            Removed,
        }

        Vector3 mInitScale = Vector3.one;
        protected STATE mState = STATE.Init;

        public virtual void Clear()
        {
            OnRemove = null;
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

        public void Init()
        {
            mState = STATE.Playing;
            onPlay();
        }

        public void Play()
        {
            if (mState == STATE.Playing)
                return;
            mState = STATE.Playing;
            onPlay();
        }

        public void Pause()
        {
            if (mState == STATE.Paused)
                return;
            mState = STATE.Paused;
            onPause();
        }

        public void Resume()
        {
            if (mState != STATE.Paused)
                return;
            mState = STATE.Playing;
            onResume();
        }

        public void Stop()
        {
            switch (mState)
            {
                case STATE.Stopped:
                case STATE.Removed:
                    return;
                default:
                    break;
            }
            mState = STATE.Stopped;
            onStop();
        }

        public void Remove()
        {
            if (mState == STATE.Removed)
                return;
            mState = STATE.Removed;
            OnRemove?.Invoke(this);
            EffectManager.Instance.Remove(this);
        }

        private void Awake()
        {
            mInitScale = transform.localScale;
            onAwake();
        }

        private void LateUpdate()
        {
            switch (mState)
            {
                case STATE.Paused:
                case STATE.Removed:
                    return;
                default:
                    break;
            }
            onLateUpdate();
        }
    }

}

