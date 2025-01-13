using System;
using UnityEngine;

namespace Devarc
{
    public abstract class SimpleFsmObject<STATE, OWNER>
        where STATE : struct, IConvertible
        where OWNER : MonoBehaviour
    {
        public abstract STATE State { get; }
        protected virtual void onInit() { }
        protected abstract void onEnter(object[] args);
        protected abstract void onExit();
        protected virtual void onTick(float deltaTime, float elapsedTime) { }

        public virtual void Clear()
        {
        }

        public OWNER Owner => mOwner;
        protected OWNER mOwner = null;
        float mElapseTime = 0f;

        public void Init(OWNER unit)
        {
            mOwner = unit;
            onInit();
        }

        public virtual void Enter(object[] args)
        {
            mElapseTime = 0f;
            onEnter(args);
        }

        public virtual void Exit()
        {
            onExit();
        }

        public void Tick()
        {
            var deltaTime = Time.deltaTime;
            mElapseTime += deltaTime;
            onTick(deltaTime, mElapseTime);
        }
    }
}
