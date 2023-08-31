using UnityEngine;
using Devarc;

public class GameNetwork : SocketClient
{
    void Start()
    {
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
