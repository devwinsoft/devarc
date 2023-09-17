using UnityEngine;
using Devarc;
using System.Net.Sockets;
using System;

public class GameNetwork : SocketClient
{
    void Awake()
    {
        OnOpen += (sender, evt) =>
        {
            Debug.Log("Connected...");

            C2Game.RequestLogin request = new C2Game.RequestLogin();
            request.sessionID = AppManager.Instance.sessionID;
            request.secret = AppManager.Instance.secret;
            SendData(request);

            Debug.Log(JsonUtility.ToJson(request));
        };

        OnClose += (sender, evt) =>
        {
            Debug.Log($"Diconnected: code={evt.Code}, reason={evt.Reason}");
        };

        OnError += (sender, evt) =>
        {
            Debug.LogError(evt.Message);
            DisConnect();
        };

        RegisterHandler<Game2C.NotifyLogin>(onReceive);
    }


    void onReceive(Game2C.NotifyLogin packet)
    {
        if (packet.errorCode == ErrorType.SUCCESS)
        {
        }
        else
        {
            DisConnect();
        }
        Debug.Log(JsonUtility.ToJson(packet));
    }
}
