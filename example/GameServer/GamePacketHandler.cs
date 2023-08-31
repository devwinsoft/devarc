using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Devarc
{
    public class GamePacketHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsBinary)
                Send(e.RawData);
            else
                Send(e.Data);
        }
    }
}
