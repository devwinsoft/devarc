using MessagePack;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WebSocketSharp;

namespace Devarc
{
    public class SessionState_Connected : SessionState
    {
        public override SessionStateType StateType => SessionStateType.Connected;


        public SessionState_Connected(SocketClient session) : base(session)
        {
        }


        public override void OnEnter()
        {
            //var packet = new USER();
            //packet.seq = 777;
            //packet.parts.Add(new ITEM());
            //mSession.SendData(packet);
        }
    }

}

