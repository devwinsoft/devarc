using System;
using WebSocketSharp;

namespace Devarc
{
    public enum SessionStateType
    {
        None,
        Connecting,
        Connected,
        DisConnecting,
        DisConnected,
    }

    public abstract class SessionState
    {
        public abstract SessionStateType StateType { get; }
        public abstract void OnEnter();
        public virtual void OnExit() { }

        protected SocketClient mSession = null;


        protected SessionState(SocketClient session)
        {
            mSession = session;
        }
    }
}

