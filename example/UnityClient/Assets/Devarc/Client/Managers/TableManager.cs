using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using Devarc;
using System;

public enum TableErrorType
{
    SUCCESS,
    CANNOT_FIND_LOAD_FUNCTION,
    CANNOT_FIND_UNLOAD_FUNCTION,
    CANNOT_LOAD_BUNDLE,
    CANNOT_LOAD_RESOURCE,
    CANNOT_UNLOAD_BUNDLE,
    CANNOT_UNLOAD_RESOURCE,
    DATA,
}

public enum TableFormatType
{
    BIN,
    JSON,
}

public partial class TableManager : MonoSingleton<TableManager>
{
    public delegate void CallbackLoadBin(byte[] data, MessagePackSerializerOptions options);
    public delegate void CallbackLoadJson(TextAsset textAsset);
    public delegate void CallbackSave(TextAsset textAsset, bool isBundle, SystemLanguage lang);
    public delegate void CallbackUnload();
    public delegate void CallbackError(TableErrorType errorType, params object[] args);

    public event CallbackError OnError;

    static Dictionary<string, CallbackLoadBin> mLoadTableBinCallbacks = new Dictionary<string, CallbackLoadBin>();
    static Dictionary<string, CallbackLoadJson> mLoadTableJsonCallbacks = new Dictionary<string, CallbackLoadJson>();
    static Dictionary<string, CallbackSave> mSaveTableCallbacks = new Dictionary<string, CallbackSave>();
    static Dictionary<string, CallbackUnload> mUnloadTableCallbacks = new Dictionary<string, CallbackUnload>();

    static Dictionary<string, CallbackLoadBin> mLoadStringBinCallbacks = new Dictionary<string, CallbackLoadBin>();
    static Dictionary<string, CallbackLoadJson> mLoadStringJsonCallbacks = new Dictionary<string, CallbackLoadJson>();
    static Dictionary<string, CallbackSave> mSaveStringCallbacks = new Dictionary<string, CallbackSave>();
    static Dictionary<string, CallbackUnload> mUnloadStringCallbacks = new Dictionary<string, CallbackUnload>();

    protected override void onAwake()
    {
        Table.Initailize();
#if UNITY_EDITOR
        StaticCompositeResolver.Instance.Register(
            StandardResolver.Instance,
            MessagePack.Unity.UnityResolver.Instance
        );
#else
        StaticCompositeResolver.Instance.Register(
            GeneratedResolver.Instance,
            StandardResolver.Instance,
            MessagePack.Unity.UnityResolver.Instance
        );
#endif
    }

    public static string GetClassName(string fileName)
    {
        return Path.GetFileName(fileName.Split('@')[0]).ToLower();
    }

    public static void RegisterLoadTableBin(string tableName, CallbackLoadBin callback)
    {
        mLoadTableBinCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterLoadTableJson(string tableName, CallbackLoadJson callback)
    {
        mLoadTableJsonCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterSaveTable(string tableName, CallbackSave callback)
    {
        mSaveTableCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterUnloadTable(string tableName, CallbackUnload callback)
    {
        mUnloadTableCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterLoadStringBin(string tableName, CallbackLoadBin callback)
    {
        mLoadStringBinCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterLoadStringJson(string tableName, CallbackLoadJson callback)
    {
        mLoadStringJsonCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterSaveString(string tableName, CallbackSave callback)
    {
        mSaveStringCallbacks.Add(GetClassName(tableName), callback);
    }

    public static void RegisterUnloadString(string tableName, CallbackUnload callback)
    {
        mUnloadStringCallbacks.Add(GetClassName(tableName), callback);
    }


    static void invokeLoadTableBin(TextAsset textAsset, MessagePackSerializerOptions options)
    {
        CallbackLoadBin callback = null;
        if (mLoadTableBinCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            var data = Convert.FromBase64String(textAsset.text);
            callback.Invoke(data, options);
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    static void invokeLoadTableJson(TextAsset textAsset)
    {
        CallbackLoadJson callback = null;
        if (mLoadTableJsonCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset);
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    static void invokeSaveTable(TextAsset textAsset, bool isBundle)
    {
        CallbackSave callback = null;
        if (mSaveTableCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset, isBundle, SystemLanguage.Unknown);
        }
    }

    static void invokeUnloadTable(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadTableCallbacks.TryGetValue(GetClassName(tableName), out callback))
        {
            callback.Invoke();
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_UNLOAD_FUNCTION, tableName);
        }
    }

    static void invokeLoadStringBin(TextAsset textAsset, MessagePackSerializerOptions options)
    {
        CallbackLoadBin callback = null;
        if (mLoadStringBinCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            var data = Convert.FromBase64String(textAsset.text);
            callback.Invoke(data, options);
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    static void invokeLoadStringJson(TextAsset textAsset)
    {
        CallbackLoadJson callback = null;
        if (mLoadStringJsonCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset);
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_LOAD_FUNCTION, textAsset);
        }
    }

    static void invokeSaveString(TextAsset textAsset, bool isBundle, SystemLanguage lang)
    {
        CallbackSave callback = null;
        if (mSaveStringCallbacks.TryGetValue(GetClassName(textAsset.name), out callback))
        {
            callback.Invoke(textAsset, isBundle, lang);
        }
    }

    static void invokeUnloadString(string tableName)
    {
        CallbackUnload callback = null;
        if (mUnloadStringCallbacks.TryGetValue(GetClassName(tableName), out callback))
        {
            callback.Invoke();
        }
        else if (IsCreated())
        {
            Instance.OnError?.Invoke(TableErrorType.CANNOT_FIND_UNLOAD_FUNCTION, tableName);
        }
    }

    public void LoadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableJsonDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var textAsset in list)
        {
            isEmpty = false;
            invokeLoadTableJson(textAsset);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_RESOURCE, resourceDir, SystemLanguage.Unknown);
        }
    }

    public void UnloadResourceTable()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.tableJsonDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadTable(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_RESOURCE, resourceDir);
        }
    }


    public void LoadResourceString(SystemLanguage lang)
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringJsonDirectory;
        var list = AssetManager.Instance.LoadResourceAssets<TextAsset>(resourceDir, lang);
        bool isEmpty = true;
        foreach (var textAsset in list)
        {
            isEmpty = false;
            invokeLoadStringJson(textAsset);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_LOAD_RESOURCE, resourceDir, lang);
        }
    }

    public void UnloadResourceString()
    {
        string resourceDir = DEV_Settings.Instance.defaultDirectory.stringJsonDirectory;
        var removeList = AssetManager.Instance.UnloadResourceAssets<TextAsset>(resourceDir);
        bool isEmpty = true;
        foreach (var tableName in removeList)
        {
            isEmpty = false;
            invokeUnloadString(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_RESOURCE, resourceDir);
        }
    }



    public IEnumerator LoadBundleTable(string addressKey, TableFormatType formatType)
    {
        var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>(addressKey);
        yield return handle;
        if (handle.IsValid())
        {
            try
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
                        foreach (var textAsset in handle.Result)
                        {
                            invokeLoadTableBin(textAsset, options);
                        }
                        break;
                    default:
                        foreach (var textAsset in handle.Result)
                        {
                            invokeLoadTableJson(textAsset);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(TableErrorType.DATA, addressKey);
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
            invokeUnloadTable(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_BUNDLE, addressKey);
        }
    }



    public IEnumerator LoadBundleString(string addressKey, TableFormatType formatType, SystemLanguage lang)
    {
        var handle = AssetManager.Instance.LoadBundleAssets<TextAsset>(addressKey, lang);
        yield return handle;
        if (handle.IsValid())
        {
            try
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
                        foreach (var textAsset in handle.Result)
                        {
                            invokeLoadStringBin(textAsset, options);
                        }
                        break;
                    default:
                        foreach (var textAsset in handle.Result)
                        {
                            invokeLoadStringJson(textAsset);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(TableErrorType.DATA, addressKey);
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
            invokeUnloadString(tableName);
        }
        if (isEmpty)
        {
            OnError?.Invoke(TableErrorType.CANNOT_UNLOAD_BUNDLE, addressKey);
        }
    }

#if UNITY_EDITOR
    public static void SaveTable()
    {
        var assem = GetPlayerAssembly();
        var types = assem.GetTypes();
        var textAssets = AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetTablePath(true, TableFormatType.JSON));
        foreach (var textAsset in textAssets)
        {
            invokeSaveTable(textAsset, true);
        }

        textAssets = AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetTablePath(false, TableFormatType.JSON));
        foreach (var textAsset in textAssets)
        {
            invokeSaveTable(textAsset, false);
        }

        SystemLanguage[] languages = new SystemLanguage[]
        { SystemLanguage.Korean
        , SystemLanguage.English
        , SystemLanguage.Japanese
        , SystemLanguage.Chinese
        };
        foreach (var lang in languages)
        {
            textAssets = AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetStringPath(lang, true, TableFormatType.JSON));
            foreach (var textAsset in textAssets)
            {
                invokeSaveString(textAsset, true, lang);
            }

            textAssets = AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetStringPath(lang, false, TableFormatType.JSON));
            foreach (var textAsset in textAssets)
            {
                invokeSaveString(textAsset, false, lang);
            }
        }
    }

    public static Assembly GetPlayerAssembly()
    {
        foreach (var assem in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assem.GetName().Name.EndsWith("-CSharp"))
            {
                return assem;
            }
        }
        return null;
    }
#endif
}

