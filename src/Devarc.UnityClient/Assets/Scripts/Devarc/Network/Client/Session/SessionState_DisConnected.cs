using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

namespace Devarc
{
    public class SessionState_DisConnected : SessionState
    {
        public override SessionStateType StateType => SessionStateType.DisConnected;


        public SessionState_DisConnected(SocketClient session) : base(session)
        {
        }


        public override void OnEnter()
        {
        }
    }
}

