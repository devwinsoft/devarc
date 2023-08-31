using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using WebSocketSharp;

namespace Devarc
{
    public class SessionState_Connecting : SessionState
    {
        public override SessionStateType StateType => SessionStateType.Connecting;


        public SessionState_Connecting(SocketClient session) : base(session)
        {
        }

        public override void OnEnter()
        {
        }
    }
}
