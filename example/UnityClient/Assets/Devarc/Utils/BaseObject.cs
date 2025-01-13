using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseObject : MonoBehaviour
    {
        bool mIsInitAwake = false;
        protected List<BaseController> mControllers = new List<BaseController>();

        protected abstract void onInitAwake();
        protected abstract void onPlayStart();

        public virtual bool InitAwake()
        {
            if (mIsInitAwake)
                return false;
            mIsInitAwake = true;
            onInitAwake();
            return true;
        }

        public virtual void PlayStart()
        {
            onPlayStart();
        }

        void Awake()
        {
            InitAwake();
        }

        protected T register<T>() where T : BaseController
        {
            T controller = gameObject.SafeGetComponent<T>();
            mControllers.Add(controller);
            controller.InitAwake(this);
            return controller;
        }

        protected void register<T>(T controller) where T : BaseController
        {
            mControllers.Add(controller);
            controller.InitAwake(this);
        }
    }

    public abstract class BaseObject<ABILITY> : BaseObject
    {
        public abstract ABILITY Ability { get; }

        protected abstract void onInitLoad(ABILITY ability);

        public virtual void Load(ABILITY ability)
        {
            onInitLoad(ability);
            foreach (var controller in mControllers)
            {
                controller.InitLoad();
            }
        }
    }
}
