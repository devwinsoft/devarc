using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Devarc
{
    public class UIManager : PrefabSingleton<UIManager>
    {
        static List<UICanvas> mLayouts = new List<UICanvas>();

        public Camera uiCamera => mCamera;
        Camera mCamera = null;
        RectTransform mRectTransform;
        Dictionary<Type, List<UIPanel>> mPanels = new Dictionary<Type, List<UIPanel>>();
        bool mLockCursor = false;
        bool mInitialized = false;

        protected override void onAwake()
        {
            base.onAwake();
            mCamera = GetComponentInChildren<Camera>();
            mRectTransform = gameObject.SafeGetComponent<RectTransform>();
        }

        public void Init()
        {
            if (mInitialized)
                return;
            mInitialized = true;

            foreach (var obj in mLayouts)
            {
                obj.Init();
            }
        }

        public void ShowCursor(bool value)
        {
            if (mLockCursor == true)
            {
                return;
            }

            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        public UICanvas CreateCanvas(string asset_name)
        {
            UICanvas obj = AssetManager.Instance.CreateObject<UICanvas>(asset_name, mTransform);
            if (mInitialized)
            {
                obj.Init();
            }
            mLayouts.Add(obj);
            return obj;
        }

        public void RegisterCanvas(UICanvas obj)
        {
            if (mInitialized)
            {
                obj.Init();
            }
            mLayouts.Add(obj);
        }

        public void Remove(UICanvas obj)
        {
            obj.Clear();
            mLayouts.Remove(obj);
            Destroy(obj.gameObject);
        }

        public T CreatePanel<T>(string asset_name) where T : UIPanel
        {
            T obj = null;
            List<UIPanel> list = null;
            if (mPanels.TryGetValue(typeof(T), out list) && list.Count > 0)
            {
                obj = list[0] as T;
                list.RemoveAt(0);
            }
            else
            {
                obj = AssetManager.Instance.CreateObject<T>(asset_name, mTransform);
            }
            obj.transform.localScale = Vector3.one;
            return obj;
        }

        public void Remove<T>(T obj) where T : UIPanel
        {
            var type = typeof(T);
            Remove(type, obj);
        }

        public void Remove(Type type, UIPanel obj)
        {
            List<UIPanel> list = null;
            if (mPanels.TryGetValue(type, out list) == false)
            {
                list = new List<UIPanel>(32);
                mPanels.Add(type, list);
            }
            obj.Clear();
            list.Add(obj);
            obj.rectTransform.SetParent(transform);
            obj.gameObject.SetActive(false);
        }
    }
}