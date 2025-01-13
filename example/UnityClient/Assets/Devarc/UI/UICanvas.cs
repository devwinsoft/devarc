using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Devarc
{
    public abstract class UICanvas : MonoBehaviour
    {
        public abstract void Clear();
        protected abstract void onInit();
        protected abstract void onAwake();
        protected abstract void onDestroy();

        public Canvas canvas => mCanvas;
        Canvas mCanvas;

        bool mInitialized = false;
        List<UIFrame> mFrames = new List<UIFrame>();

        public void Init()
        {
            if (mInitialized) return;
            mInitialized = true;

            var list = GetComponentsInChildren<UIFrame>(true);
            foreach (var frame in list)
            {
                mFrames.Add(frame);
                frame.Init(this);
            }
            onInit();
        }

        public UIFrame CreateFrame<T>(string asset_name)
        {
            var prefab = AssetManager.Instance.GetAsset<GameObject>(asset_name);
            return CreateFrame<T>(prefab);
        }

        public UIFrame CreateFrame<T>(GameObject prefab)
        {
            UIFrame prefabFrame = prefab?.GetComponent<UIFrame>();
            if (prefabFrame == null) return null;

            var prefabTr = prefab.GetComponent<RectTransform>();
            if (prefabTr == null) return null;

            var obj = Instantiate(prefab, transform);
            var frame = obj.GetComponent<UIFrame>();
            var rectTransform = frame.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = prefabTr.anchoredPosition;
            rectTransform.localScale = Vector3.one;

            if (mFrames.Contains(frame) == false)
            {
                mFrames.Add(frame);
            }
            if (mInitialized)
            {
                frame.Init(this);
            }
            return frame;
        }

        public void RemoveFrame(UIFrame frame)
        {
            frame.Clear();
            GameObject.Destroy(frame.gameObject);
        }


        private void Awake()
        {
            mCanvas = GetComponent<Canvas>();
            onAwake();
        }

        private void Start()
        {
            if (UIManager.IsCreated())
            {
                UIManager.Instance.RegisterCanvas(this);
            }
        }

        private void OnDestroy()
        {
            onDestroy();
            if (UIManager.IsCreated())
            {
                UIManager.Instance.Remove(this);
            }
        }
    }


    public abstract class UICanvas<T> : UICanvas where T : UICanvas
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

        public static T Create(string assetName)
        {
            if (IsCreated())
            {
                return mInstance;
            }

            var prefab = AssetManager.Instance.GetAsset<GameObject>(assetName);
            if (prefab == null)
            {
                Debug.LogError($"[{typeof(T).Name}::Create] Cannot find prefab: fileName={assetName}");
                return null;
            }
            var obj = Instantiate(prefab);
            var compo = obj.GetComponent<T>();
            return compo;
        }

        public static void Delete()
        {
            if (mInstance != null)
            {
                GameObject.Destroy(mInstance.gameObject);
            }
        }

        protected override void onAwake()
        {
            mInstance = this as T;
            mTransform = transform;
        }

        protected override void onDestroy()
        {
            if (mInstance == this)
            {
                mInstance = null;
            }
        }
    }
}
