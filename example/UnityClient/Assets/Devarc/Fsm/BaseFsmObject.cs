using System;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseFsmObject<STATE, OWNER> : MonoBehaviour
        where STATE : struct, IConvertible
        where OWNER : BaseObject
    {
        public abstract STATE State { get; }
        protected virtual void onInit() { }
        protected abstract void onEnter(object[] args);
        protected abstract void onExit(bool cancel);
        protected virtual void onTick() { }

        public virtual void Clear()
        {
        }

        public OWNER Owner => mOwner;
        protected OWNER mOwner = null;
        protected bool mIsEntered = false;

        public void Init(OWNER unit)
        {
            mOwner = unit;
            onInit();
        }

        public virtual void Enter(object[] args)
        {
            mIsEntered = true;
            onEnter(args);
        }

        public virtual void Exit(bool cancel)
        {
            mIsEntered = false;
            onExit(cancel);
        }

        public void Tick()
        {
            onTick();
        }
    }
}
