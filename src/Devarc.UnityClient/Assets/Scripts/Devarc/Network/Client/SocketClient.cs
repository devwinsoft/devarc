using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


namespace Devarc
{
    public class SocketClient : MonoBehaviour
    {
        public long SessionID => mSessionID;
        long mSessionID = 0;

        public bool IsConnected => mCurrentState != null && mCurrentState.StateType == SessionStateType.Connected;

        public WebSocket Socket => mSocket;
        WebSocket mSocket;

        string mConnString = string.Empty;

        SessionState mCurrentState = null;
        Dictionary<SessionStateType, SessionState> mStates = new Dictionary<SessionStateType, SessionState>();
        Dictionary<string, PacketHandler> mHandlers = new Dictionary<string, PacketHandler>();

        public void Init(string connStr)
        {
            mConnString = connStr;

            mSocket = new WebSocket(connStr);

            mSocket.OnOpen += (sender, evt) =>
            {
                ChangeState(SessionStateType.Connected);
            };

            mSocket.OnClose += (sender, evt) =>
            {
                ChangeState(SessionStateType.DisConnected);
            };

            mSocket.OnMessage += (sender, evt) =>
            {
                ReceiveData(evt.RawData);
            };

            registerState(new SessionState_Connecting(this));
            registerState(new SessionState_Connected(this));
            registerState(new SessionState_DisConnecting(this));
            registerState(new SessionState_DisConnected(this));

            ChangeState(SessionStateType.DisConnected);
        }


        public void RegisterHandler<T>(PacketCallback<T> callback) where T : class
        {
            Type type = typeof(T);
            PacketHandler<T> handler = new PacketHandler<T>();
            handler.typeName = type.Name;
            handler.callback = callback;
            mHandlers.Add(handler.typeName, handler);
        }

        public void UnRegisterHandler<T>() where T : class
        {
            mHandlers.Remove(typeof(T).Name);
        }


        void registerState<T>(T state) where T : SessionState
        {
            mStates.Add(state.StateType, state);
        }


        public bool ChangeState(SessionStateType state)
        {
            SessionState nextState = null;
            if (mStates.TryGetValue(state, out nextState) == false)
            {
                return false;
            }

            mCurrentState?.OnExit();
            mCurrentState = nextState;
            mCurrentState.OnEnter();
            return true;
        }


        public void SendData<T>(T obj)
        {
            var buf = new PacketEncoder();
            var data = buf.Pack(obj);
            Socket.Send(data);
        }


        public bool ReceiveData(byte[] rawData)
        {
            try
            {
                var buf = new PacketEncoder();
                (string typeName, byte[] encoded) = buf.Parse(rawData);

                PacketHandler handler;
                if (mHandlers.TryGetValue(typeName, out handler) == false)
                {
                    return false;
                }
                handler.Invoke(encoded);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }


        public void Connect()
        {
            Debug.Log($"Connect:{mConnString}");
            Socket.ConnectAsync();
            ChangeState(SessionStateType.Connecting);
        }


        public void DisConnect()
        {
            Socket.CloseAsync();
            ChangeState(SessionStateType.DisConnecting);
        }

    }
}

