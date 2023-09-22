using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Devarc;
using System.Linq;
using System.IO;

public enum TableErrorType
{
    SUCCESS,
    CANNOT_FIND_LOAD_FUNCTION,
    CANNOT_FIND_UNLOAD_FUNCTION,
    CANNOT_LOAD_BUNDLE,
    CANNOT_LOAD_RESOURCE,
    CANNOT_UNLOAD_BUNDLE,
    CANNOT_UNLOAD_RESOURCE,
}

public partial class TableManager : MonoSingleton<TableManager>
{
    public delegate void CallbackLoad(TextAsset textAsset);
    public delegate void CallbackUnload();
    public delegate void CallbackError(TableErrorType errorType, params object[] args);

    public event CallbackError OnError;

    Dictionary<string, CallbackLoad> mLoadTableCallbacks = new Dictionary<string, CallbackLoad>();
    Dictionary<string, CallbackUnload> mUnloadTableCallbacks = new Dictionary<string, CallbackUnload>();

    Dictionary<string, CallbackLoad> mLoadStringCallbacks = new Dictionary<string, CallbackLoad>();
    Dictionary<string, CallbackUnload> mUnloadStringCallbacks = new Dictionary<string, CallbackUnload>();

    string getFileName(string fileName)
    {
        return Path.GetFileName(fileName.Split('@')[0]).ToLower();
    }


    public void registerLoadTableCallback(string tableName, CallbackLoad callback)
    {
        mLoadTableCallbacks.Add(getFileName(tableName), callback);
    }

    public void registerUnloadTableCallback(string tableName, CallbackUnload callback)
    {
        mUnloadTableCallbacks.Add(getFileName(tableName), callback);
    }

    public void registerLoadStringCallback(string tableName, CallbackLoad callback)
    {
        mLoadStringCallbacks.Add(getFileName(tableName), callback);
    }

    public void registerUnloadStringCallback(string tableName, CallbackUnload callback)
    {
        mUnloadStringCallbacks.Add(getFileName(tableName), callback);
    }

    void invokeLoadTableCallback(TextAsset textAsset)
    {
        CallbackLoad callback = null;
        if (mLoadTableCallbacks.TryGetValue(getFileName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset);
        }
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    void invokeUnloadTableCallback(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadTableCallbacks.TryGetValue(getFileName(tableName), out callback))
        {
            callback.Invoke();
        }
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_FIND_UNLOAD_FUNCTION, tableName);
        }
    }

    void invokeLoadStringCallback(TextAsset textAsset)
    {
        CallbackLoad callback = null;
        if (mLoadStringCallbacks.TryGetValue(getFileName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset);
        }
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    void invokeUnloadStringCallback(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadStringCallbacks.TryGetValue(getFileName(tableName), out callback))
        {
            callback.Invoke();
        }
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_FIND_UNLOAD_FUNCTION, tableName);
        }
    }

    public void LoadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableSubDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var textAsset in list)
        {
            isEmpty = false;
            invokeLoadTableCallback(textAsset);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_RESOURCE, resourceDir, SystemLanguage.Unknown);
        }
    }

    public void UnloadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableSubDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadTableCallback(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_RESOURCE, resourceDir);
        }
    }


    public void LoadResourceString(SystemLanguage lang)
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringSubDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir, lang);
        bool isEmpty = true;
        foreach (var textAsset in list)
        {
            isEmpty = false;
            invokeLoadStringCallback(textAsset);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_RESOURCE, resourceDir, lang);
        }
    }

    public void UnloadResourceString()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringSubDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadStringCallback(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_RESOURCE, resourceDir);
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
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_BUNDLE, addressKey);
        }
    }

    public void UnloadBundleTable(string addressKey)
    {
        var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadTableCallback(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_BUNDLE, addressKey);
        }
    }



    public IEnumerator LoadBundleString(string addressKey, SystemLanguage lang)
    {
        var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>(addressKey, lang);
        yield return handle;
        if (handle.IsValid())
        {
            foreach (var textAsset in handle.Result)
            {
                invokeLoadStringCallback(textAsset);
            }
        }
        else
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_BUNDLE, addressKey);
        }
    }


    public void UnloadBundleString(string addressKey)
    {
        var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadStringCallback(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_BUNDLE, addressKey);
        }
    }
}

