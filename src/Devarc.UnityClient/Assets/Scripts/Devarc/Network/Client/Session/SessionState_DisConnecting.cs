using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using WebSocketSharp;

namespace Devarc
{
    public class SessionState_DisConnecting : SessionState
    {
        public override SessionStateType StateType => SessionStateType.DisConnecting;


        public SessionState_DisConnecting(SocketClient session) : base(session)
        {
        }


        public override void OnEnter()
        {
        }
    }
}

