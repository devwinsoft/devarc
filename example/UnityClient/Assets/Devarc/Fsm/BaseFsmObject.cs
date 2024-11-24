using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseFsmObject<OWNER, DATA> : MonoBehaviour
        where OWNER : BaseObject
        where DATA : BaseFsmData
    {
        protected virtual void onInitAwake() { }
        protected virtual void onInitLoad() { }
        protected abstract void onEnter(DATA transData);
        protected abstract void onExit();

        public virtual void Clear()
        {
        }

        protected OWNER mOwner = null;

        public void InitAwake(OWNER unit)
        {
            mOwner = unit;
            onInitAwake();
        }

        public void InitLoad()
        {
            onInitLoad();
        }

        public virtual void Enter(DATA transData)
        {
            onEnter(transData);
        }

        public virtual void Exit()
        {
            onExit();
        }
    }
}
