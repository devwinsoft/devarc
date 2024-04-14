using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseEffectPlay : MonoBehaviour, ISimpleObject
    {
        public event System.Action<BaseEffectPlay> OnRemove;

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
        bool mPlaying = false;
        bool mRemoved = false;

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
            mPlaying = true;
            mRemoved = false;
            onPlay();
        }

        public void Play()
        {
            if (mRemoved)
                return;
            mPlaying = true;
            onPlay();
        }

        public void Pause()
        {
            if (mRemoved)
                return;
            mPlaying = false;
            onPause();
        }

        public void Resume()
        {
            if (mRemoved)
                return;
            mPlaying = true;
            onResume();
        }

        public void Stop()
        {
            if (mRemoved)
                return;
            mPlaying = false;
            onStop();
        }

        public void Remove()
        {
            if (mRemoved)
                return;
            mRemoved = true;
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
            if (mPlaying == false)
            {
                return;
            }
            onLateUpdate();
        }
    }

}

