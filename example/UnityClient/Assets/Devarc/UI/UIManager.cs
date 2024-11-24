using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class UIManager : PrefabSingleton<UIManager>
    {
        static List<UILayout> msLayouts = new List<UILayout>();

        Dictionary<Type, List<UIPanel>> mPanels = new Dictionary<Type, List<UIPanel>>();
        bool mLockCursor = false;
        bool mInitialized = false;

        public void Init()
        {
            if (mInitialized)
                return;
            mInitialized = true;

            foreach (var obj in msLayouts)
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


        public UILayout CreateLayout(string asset_name)
        {
            UILayout obj = AssetManager.Instance.CreateObject<UILayout>(asset_name, mTransform);
            if (mInitialized)
            {
                obj.Init();
            }
            msLayouts.Add(obj);
            return obj;
        }

        public void RegisterLayout(UILayout obj)
        {
            if (mInitialized)
            {
                obj.Init();
            }
            msLayouts.Add(obj);
        }

        public void RemoveLayout(UILayout obj)
        {
            obj.Clear();
            msLayouts.Remove(obj);
            Destroy(obj.gameObject);
        }

        public UIFrame CreateFrame<T>(string asset_name, Transform attachTr)
        {
            var prefab = AssetManager.Instance.GetAsset<GameObject>(asset_name);
            if (prefab == null) return null;

            var prefabFrame = prefab.GetComponent<UIFrame>();
            if (prefabFrame == null) return null;

            var prefabTr = prefab.GetComponent<RectTransform>();
            if (prefabTr == null) return null;

            var obj = Instantiate(prefab, attachTr);
            var frame = obj.GetComponent<UIFrame>();
            var rectTransform = frame.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = prefabTr.anchoredPosition;
            rectTransform.localScale = Vector3.one;

            if (mInitialized)
            {
                frame.Init();
            }
            return frame;
        }

        public void RemoveFrame(UIFrame frame)
        {
            frame.Clear();
            GameObject.Destroy(frame.gameObject);
        }

        public T Pop<T>(string asset_name) where T : UIPanel
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
            return obj;
        }

        public void Push<T>(T obj) where T : UIPanel
        {
            var type = typeof(T);
            Push(type, obj);
        }

        public void Push(Type type, UIPanel obj)
        {
            List<UIPanel> list = null;
            if (mPanels.TryGetValue(type, out list) == false)
            {
                list = new List<UIPanel>(32);
                mPanels.Add(type, list);
            }
            obj.Clear();
            list.Add(obj);
        }
    }
}