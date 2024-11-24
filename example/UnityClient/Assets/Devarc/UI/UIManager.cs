using System;
using System.Collections.Generic;
using UnityEngine;
using Devarc;

public class UIManager : MonoSingleton<UIManager>
{
    List<UIFrame> mFrames = new List<UIFrame>();
    Dictionary<Type, List<UIPanel>> mPanels = new Dictionary<Type, List<UIPanel>>();
    bool mLockCursor = false;
    bool mInitialized = false;

    public void Init()
    {
        if (mInitialized)
            return;
        mInitialized = true;
        foreach (var frame in mFrames)
        {
            frame.Init();
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


    public UIFrame CreateFrame(string asset_name)
    {
        UIFrame obj = AssetManager.Instance.CreateObject<UIFrame>(asset_name, mTransform);
        return obj;
    }

    public UIFrame CreateFrame<T>(string asset_name, Transform attachTr)
    {
        var prefab = AssetManager.Instance.GetAsset<GameObject>(asset_name);
        if (prefab == null) return null;

        var prefabTr = prefab.GetComponent<RectTransform>();
        if (prefabTr == null) return null;

        UIFrame frame = AssetManager.Instance.CreateObject<UIFrame>(asset_name, mTransform);
        var rectTransform = frame.GetComponent<RectTransform>();
        rectTransform.SetParent(attachTr);
        rectTransform.anchoredPosition = prefabTr.anchoredPosition;
        rectTransform.localScale = Vector3.one;

        mFrames.Add(frame);
        if (mInitialized)
        {
            frame.Init();
        }
        return frame;
    }

    public void RemoveFrame(UIFrame frame)
    {
        mFrames.Remove(frame);
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
