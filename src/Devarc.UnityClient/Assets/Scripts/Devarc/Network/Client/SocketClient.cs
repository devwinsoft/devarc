using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


namespace Devarc
{
    public class SocketClient : MonoBehaviour
    {
        public enum SessionStateType
        {
            None,
            Connecting,
            Connected,
            DisConnecting,
            DisConnected,
        }
        volatile SessionStateType mCurrentState = SessionStateType.None;

        public bool IsConnected => mCurrentState == SessionStateType.Connected;

        Dictionary<string, PacketHandler> mHandlers = new Dictionary<string, PacketHandler>();
        MainThreadDispatcher mDispatcher = new MainThreadDispatcher();
        WebSocket mSocket;
        string mConnString = string.Empty;

        public event EventHandler OnOpen;
        public event EventHandler<CloseEventArgs> OnClose;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<MessageEventArgs> OnMessage;


        private void Update()
        {
            mDispatcher.DoWork();
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


        public void ChangeState(SessionStateType state)
        {
            mCurrentState = state;
        }


        public void SendData<T>(T obj)
        {
            var buf = new PacketEncoder();
            var data = buf.Pack(obj);
            mSocket.Send(data);
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


        public void Connect(string connStr)
        {
            mConnString = connStr;

            mSocket = new WebSocket(mConnString);
            mSocket.OnOpen += onOpen;
            mSocket.OnClose += onClose;
            mSocket.OnError += onError;
            mSocket.OnMessage += onMessage;

            mSocket.ConnectAsync();
            ChangeState(SessionStateType.Connecting);

            Debug.Log($"Connect:{mConnString}");
        }


        public void DisConnect()
        {
            mSocket.CloseAsync();
            ChangeState(SessionStateType.DisConnecting);
        }


        void onOpen(object sender, EventArgs evt)
        {
            ChangeState(SessionStateType.Connected);

            mDispatcher.AddWork((args) =>
            {
                OnOpen?.Invoke(this, (EventArgs)args[0]);
            }, evt);
        }

        void onClose(object sender, CloseEventArgs evt)
        {
            mSocket.OnOpen -= onOpen;
            mSocket.OnClose -= onClose;
            mSocket.OnError -= onError;
            mSocket.OnMessage -= onMessage;
            mSocket = null;

            ChangeState(SessionStateType.DisConnected);

            mDispatcher.AddWork((args) =>
            {
                OnClose?.Invoke(this, (CloseEventArgs)args[0]);
            }, evt);
        }

        void onError(object sender, ErrorEventArgs evt)
        {
            mDispatcher.AddWork((args) =>
            {
                OnError?.Invoke(this, (ErrorEventArgs)args[0]);
            }, evt);
        }

        void onMessage(object sender, MessageEventArgs evt)
        {
            mDispatcher.AddWork((args) =>
            {
                var msg = (MessageEventArgs)args[0];
                ReceiveData(msg.RawData);
            }, evt);
        }
    }
}

