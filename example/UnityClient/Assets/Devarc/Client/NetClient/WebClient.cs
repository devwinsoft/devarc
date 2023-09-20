using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Devarc;
using MessagePack;

//public class CertificateHandler_AcceptAll : CertificateHandler
//{
//    protected override bool ValidateCertificate(byte[] certificateData)
//    {
//        return true;
//    }
//}

public abstract class WebClient : MonoBehaviour
{
    enum RequestType
    {
        Get,
        Post,
    }
    public delegate void RequestCallback<RES>(RES response, UnityWebRequest.Result errorType, string errorMsg);


    public string BaseURL => $"https://{mDomain}:{mPort}";

    public string Domain => mDomain;
    string mDomain;

    public int Port => mPort;
    int mPort;

    string mDirectory;
    string mArgName;
    PacketEncoder mPacketEncoder = new PacketEncoder();
    Dictionary<Type, PacketHandler> mHandlers = new Dictionary<Type, PacketHandler>();


    public void InitConnection(string doamin, int port)
    {
        mDomain = doamin;
        mPort = port;
    }

    public void InitProtocol(string directory , string argName, IFormatterResolver formatterResolver)
    {
        mDirectory = directory;
        mArgName = argName;
        mPacketEncoder.Init(formatterResolver);
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
        // Debugging...
        {
            var a = decodeBase64<REQ>(sendData);
            Debug.LogWarning(JsonUtility.ToJson(a));
        }
        UnityWebRequest www = null;
        switch (reqType)
        {
            case RequestType.Get:
                {
                    var url = $"{BaseURL}/{mDirectory}?{mArgName}={sendData}";
                    www = UnityWebRequest.Get(url);
                    Debug.Log($"Request Get: {url}");
                }
                break;
            default:
                {
                    WWWForm form = new WWWForm();
                    form.AddField(mArgName, sendData);

                    www = UnityWebRequest.Post($"{BaseURL}/{mDirectory}", form);
                    var url = $"{BaseURL}/{mDirectory}?{mArgName}={sendData}";
                    Debug.Log($"Request Post: {url}");
                }
                break;
        }

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(default(RES), www.result, www.error);
        }
        else if (www.downloadHandler.data == null)
        {
            callback?.Invoke(default(RES), UnityWebRequest.Result.DataProcessingError, string.Empty);
        }
        else
        {
            RES res = UnPack<RES>(www.downloadHandler.data);
            callback?.Invoke(res, UnityWebRequest.Result.Success, string.Empty);
        }
    }


    string encodeBase64<T>(T obj)
    {
        var data = mPacketEncoder.ToBase64(obj);
        return data;
    }

    T decodeBase64<T>(string base64)
    {
        var obj = mPacketEncoder.FromBase64<T>(base64);
        return obj;
    }

    public T UnPack<T>(byte[] rawData)
    {
        var obj = mPacketEncoder.UnPack<T>(rawData);
        return obj;
    }

}
