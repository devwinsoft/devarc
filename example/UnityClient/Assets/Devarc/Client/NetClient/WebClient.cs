using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Devarc;
using MessagePack;

public class CertificateHandler_AcceptAll : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public abstract class WebClient : MonoBehaviour
{
    enum RequestType
    {
        Get,
        Post,
    }
    public delegate void RequestCallback<RES>(RES response);
    public delegate void ErrorCallback(UnityWebRequest.Result errorType, string errorMsg);

    public event ErrorCallback OnError;

    public string URL => mURL;
    string mURL;
    string mDirectory;
    string mArgName;
    PacketEncoder mPacketEncoder = new PacketEncoder();
    Dictionary<Type, PacketHandler> mHandlers = new Dictionary<Type, PacketHandler>();


    public void Init(string baseURL, string directory , string argName, IFormatterResolver formatterResolver)
    {
        mURL = baseURL;
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

    public void Get<REQ, RES>(REQ req, RequestCallback<RES> responseCallback, ErrorCallback errorCallback = null)
    {
        StartCoroutine(request<REQ, RES>(RequestType.Get, req, responseCallback, errorCallback));
    }

    public void Post<REQ, RES>(REQ req, RequestCallback<RES> responseCallback, ErrorCallback errorCallback = null)
    {
        StartCoroutine(request<REQ, RES>(RequestType.Post, req, responseCallback, errorCallback));
    }

    IEnumerator request<REQ, RES>(RequestType reqType, REQ req, RequestCallback<RES> responseCallback, ErrorCallback errorCallback)
    {
        var sendData = encodeBase64(req);
        UnityWebRequest www = null;
        switch (reqType)
        {
            case RequestType.Get:
                {
                    var url = $"{URL}/{mDirectory}?{mArgName}={sendData}";
                    www = UnityWebRequest.Get(url);
                    if (Application.isMobilePlatform)
                    {
                        www.certificateHandler = new CertificateHandler_AcceptAll();
                    }
                    Debug.Log($"Request Get: {url}");
                }
                break;
            default:
                {
                    WWWForm form = new WWWForm();
                    form.AddField(mArgName, sendData);

                    www = UnityWebRequest.Post($"{URL}/{mDirectory}", form);
                    if (Application.isMobilePlatform)
                    {
                        www.certificateHandler = new CertificateHandler_AcceptAll();
                    }
                    var url = $"{URL}/{mDirectory}?{mArgName}={sendData}";
                    Debug.Log($"Request Post: {url}");
                }
                break;
        }

        yield return www.SendWebRequest();
        if (Application.isMobilePlatform)
        {
            www.certificateHandler.Dispose();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            errorCallback?.Invoke(www.result, www.error);
            OnError?.Invoke(www.result, www.error);
        }
        else if (www.downloadHandler.data == null)
        {
            errorCallback?.Invoke(UnityWebRequest.Result.DataProcessingError, www.error);
            OnError?.Invoke(UnityWebRequest.Result.DataProcessingError, www.error);
        }
        else
        {
            try
            {
                RES res = UnPack<RES>(www.downloadHandler.data);
                responseCallback?.Invoke(res);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(UnityWebRequest.Result.DataProcessingError, e.Message);
                OnError?.Invoke(UnityWebRequest.Result.DataProcessingError, e.Message);
            }
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
