using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Devarc
{
    public class GameNetwork : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                var encoder = new PacketEncoder();
                var packet = encoder.UnPack<C2Game.RequestLogin>(e.RawData);
                Game2C.NotifyLogin response = new Game2C.NotifyLogin();
                var data = encoder.Pack(response);
                Send(data);
            }
            else
            {
                Send(e.Data);
            }
        }
    }
}
