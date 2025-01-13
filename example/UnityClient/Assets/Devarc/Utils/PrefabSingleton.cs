using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class PrefabSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => mInstance;
        static T mInstance;
        protected Transform mTransform;

        public static bool IsCreated() => mInstance != null;

        public static T CreateFromResource(string resourcePath)
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

        public static T CreateFromBundle(string fileName)
        {
            if (IsCreated())
            {
                return mInstance;
            }

            var prefab = AssetManager.Instance.GetAsset<GameObject>(fileName);
            if (prefab == null)
            {
                Debug.LogError($"[{typeof(T).Name}::Create] Cannot find prefab: fileName={fileName}");
                return null;
            }
            var obj = Instantiate(prefab);
            var compo = obj.GetComponent<T>();
            return compo;
        }

        protected virtual void onAwake() { }
        protected virtual void onDestroy() { }


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            mInstance = this as T;
            mTransform = transform;
            onAwake();
        }

        private void OnDestroy()
        {
            onDestroy();
            if (mInstance == this)
            {
                mInstance = null;
            }
        }
    }
}

