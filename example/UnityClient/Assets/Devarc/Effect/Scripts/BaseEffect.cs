using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseEffect : MonoBehaviour, ISimpleObject
    {
        public event System.Action<BaseEffect> OnRemove;

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

        public void Play()
        {
            mPlaying = true;
            onPlay();
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

        public void Remove()
        {
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

