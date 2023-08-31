using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Devarc;

public abstract class WebClient : MonoBehaviour
{
    enum RequestType
    {
        Get,
        Post,
    }
    public delegate void RequestCallback<RES>(RES response, UnityWebRequest.Result errorType, string errorMsg);

    string mURL;
    string mParamName;
    Dictionary<Type, PacketHandler> mHandlers = new Dictionary<Type, PacketHandler>();


    public void Init(string url, string paramName)
    {
        mURL = url;
        mParamName = paramName;
    }

    bool register<REQ, RES>(PacketCallback<RES> callback)
    {
        Type type = typeof(REQ);
        if (mHandlers.ContainsKey(type))
            return false;

        PacketHandler<RES> handler = new PacketHandler<RES>();
        handler.typeName = type.Name;
        handler.callback = callback;
        mHandlers.Add(type, handler);
        return true;
    }

    void unRegister<REQ>()
    {
        Type type = typeof(REQ);
        mHandlers.Remove(type);
    }

    public void Get<REQ, RES>(REQ req, RequestCallback<RES> callback)
    {
        StartCoroutine(request<REQ, RES>(RequestType.Get, req, callback));
    }

    public void Post<REQ, RES>(REQ req, RequestCallback<RES> callback)
    {
        StartCoroutine(request<REQ, RES>(RequestType.Post, req, callback));
    }

    IEnumerator request<REQ, RES>(RequestType reqType, REQ req, RequestCallback<RES> callback)
    {
        var sendData = encodeBase64(req);
        UnityWebRequest www = null;
        switch (reqType)
        {
            case RequestType.Get:
                {
                    var url = $"{mURL}?{mParamName}={sendData}";
                    www = UnityWebRequest.Get(url);
                    Debug.Log($"Request Get: {url}");
                }
                break;
            default:
                {
                    WWWForm form = new WWWForm();
                    form.AddField(mParamName, sendData);
                    www = UnityWebRequest.Post(mURL, form);

                    var url = $"{mURL}?{mParamName}={sendData}";
                    Debug.Log($"Request Post: {url}");
                }
                break;
        }

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(default(RES), www.result, www.error);
        }
        else
        {
            RES res = UnPack<RES>(www.downloadHandler.data);
            callback?.Invoke(res, UnityWebRequest.Result.Success, string.Empty);
        }
    }


    string encodeBase64<T>(T obj)
    {
        var encoder = new PacketEncoder();
        var data = encoder.ToBase64(obj);
        return data;
    }

    T decodeBase64<T>(string base64)
    {
        var encoder = new PacketEncoder();
        var obj = encoder.FromBase64<T>(base64);
        return obj;
    }

    public T UnPack<T>(byte[] rawData)
    {
        var encoder = new PacketEncoder();
        var obj = encoder.UnPack<T>(rawData);
        return obj;
    }

}
