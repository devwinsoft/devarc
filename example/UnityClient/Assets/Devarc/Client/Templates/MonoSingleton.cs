//
// Copyright (c) 2021 Kim, Hyoung Joon
// License: Apache License, Version 2.0
//
using UnityEngine;

namespace Devarc
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    return Create();
                }
                return mInstance;
            }
        }

        public static bool IsCreated() => mInstance != null;

        static T mInstance;
        static bool mInitialized = false;

        public static T Create()
        {
            if (IsCreated())
            {
                return mInstance;
            }
            GameObject obj = new GameObject(typeof(T).Name);
            mInstance = obj.AddComponent<T>();
            return mInstance;
        }

        public static T Create(string resourcePath)
        {
            if (IsCreated())
            {
                return mInstance;
            }

            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                Debug.LogError($"[{typeof(T).Name}::Create] Cannot find prefab: resourcePath={resourcePath}");
                return null;
            }
            var obj = Instantiate(prefab);
            var compo = obj.GetComponent<T>();
            return compo;
        }

        protected virtual void onAwake() { }
        protected virtual void onStart() { }
        protected virtual void onUpdate() { }
        protected virtual void onLateUpdate() { }
        protected virtual void onDestroy() { }


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            mInstance = this as T;
            if(mInitialized == false)
            {
                onAwake();
                mInitialized = true;
            }
        }

        private void OnDestroy()
        {
            onDestroy();
            if (mInstance == this)
            {
                mInstance = null;
                mInitialized = false;
            }
        }

        private void Start()
        {
            onStart();
        }

        private void Update()
        {
            onUpdate();
        }

        private void LateUpdate()
        {
            onLateUpdate();
        }
    }

}