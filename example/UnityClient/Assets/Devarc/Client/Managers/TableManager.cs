using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


public partial class TableManager : MonoSingleton<TableManager>
{
    public delegate void CallbackLoad(TextAsset textAsset);
    public delegate void CallbackUnload();

    Dictionary<string, CallbackLoad> mLoadTableCallbacks = new Dictionary<string, CallbackLoad>();
    Dictionary<string, CallbackUnload> mUnloadTableCallbacks = new Dictionary<string, CallbackUnload>();

    Dictionary<string, CallbackLoad> mLoadStringCallbacks = new Dictionary<string, CallbackLoad>();
    Dictionary<string, CallbackUnload> mUnloadStringCallbacks = new Dictionary<string, CallbackUnload>();


    public void registerLoadTableCallback(string tableName, CallbackLoad callback)
    {
        mLoadTableCallbacks.Add(tableName.ToLower(), callback);
    }

    public void registerUnloadTableCallback(string tableName, CallbackUnload callback)
    {
        mUnloadTableCallbacks.Add(tableName.ToLower(), callback);
    }

    public void registerLoadStringCallback(string tableName, CallbackLoad callback)
    {
        mLoadStringCallbacks.Add(tableName.ToLower(), callback);
    }

    public void registerUnloadStringCallback(string tableName, CallbackUnload callback)
    {
        mUnloadStringCallbacks.Add(tableName.ToLower(), callback);
    }

    void invokeLoadTableCallback(TextAsset textAsset)
    {
        CallbackLoad callback = null;
        if (mLoadTableCallbacks.TryGetValue(textAsset.name.ToLower(), out callback))
        {
            callback.Invoke(textAsset);
        }
    }

    void invokeUnloadTableCallback(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadTableCallbacks.TryGetValue(tableName.ToLower(), out callback))
        {
            callback.Invoke();
        }
    }

    void invokeLoadStringCallback(TextAsset textAsset)
    {
        CallbackLoad callback = null;
        if (mLoadStringCallbacks.TryGetValue(textAsset.name.ToLower(), out callback))
        {
            callback.Invoke(textAsset);
        }
    }

    void invokeUnloadStringCallback(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadStringCallbacks.TryGetValue(tableName.ToLower(), out callback))
        {
            callback.Invoke();
        }
    }

    public void LoadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableSubDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir);
        foreach (var textAsset in list)
        {
            invokeLoadTableCallback(textAsset);
        }
    }

    public void UnloadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableSubDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        foreach (var tableName in removeList)
        {
            invokeUnloadTableCallback(tableName);
        }
    }


    public void LoadResourceString(SystemLanguage lang)
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringSubDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir, lang);
        foreach (var textAsset in list)
        {
            invokeLoadStringCallback(textAsset);
        }
    }

    public void UnloadResourceString()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringSubDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        foreach (var tableName in removeList)
        {
            invokeUnloadStringCallback(tableName);
        }
    }



    public IEnumerator LoadBundleTable(string addressKey)
    {
        var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>(addressKey);
        yield return handle;
        if (handle.IsValid())
        {
            foreach (var textAsset in handle.Result)
            {
                invokeLoadTableCallback(textAsset);
            }
        }
    }

    public void UnloadBundleTable(string addressKey)
    {
        var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
        foreach (var tableName in removeList)
        {
            invokeUnloadTableCallback(tableName);
        }
    }



    public IEnumerator LoadBundleString(string addressKey, SystemLanguage lang)
    {
        var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>("lstring", lang);
        yield return handle;
        if (handle.IsValid())
        {
            foreach (var textAsset in handle.Result)
            {
                invokeLoadStringCallback(textAsset);
            }
        }
    }


    public void UnloadBundleString(string addressKey)
    {
        var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
        foreach (var tableName in removeList)
        {
            invokeUnloadStringCallback(tableName);
        }
    }
}

