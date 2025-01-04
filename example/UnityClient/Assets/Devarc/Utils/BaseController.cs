using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseController : MonoBehaviour
    {
        public abstract void Clear();
        public abstract void InitAwake(BaseObject obj);

        protected abstract void onInitAwake();
        protected abstract void onInitLoad();
        protected abstract void onPlayStart();
        protected virtual void onLateUpdate() { }

        private void LateUpdate()
        {
            onLateUpdate();
        }

        public void InitLoad()
        {
            onInitLoad();
        }

        public void PlayStart()
        {
            onPlayStart();
        }
    }

    public abstract class BaseController<T> : BaseController where T : BaseObject
    {
        protected T mOwner;

        public override void InitAwake(BaseObject obj)
        {
            mOwner = obj as T;
            onInitAwake();
        }
    }
}