using MessagePack;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityWebSocket;


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
        PacketEncoder mPacketEncoder = new PacketEncoder();

        public event EventHandler OnOpen;
        public event EventHandler<CloseEventArgs> OnClose;
        public event EventHandler<ErrorEventArgs> OnError;

        private void Update()
        {
            mDispatcher.MainThreadTick();
        }

        public void Init(IFormatterResolver formatterResolver)
        {
            mPacketEncoder.Init(formatterResolver);
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
            try
            {
                var data = mPacketEncoder.Pack(obj);
                mSocket.SendAsync(data);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }


        public bool ReceiveData(byte[] rawData)
        {
            try
            {
                (string typeName, byte[] encoded) = mPacketEncoder.Parse(rawData);

                PacketHandler handler;
                if (mHandlers.TryGetValue(typeName, out handler) == false)
                {
                    return false;
                }
                handler.Invoke(encoded, mPacketEncoder.Options);
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
            switch (mCurrentState)
            {
                case SessionStateType.Connecting:
                case SessionStateType.Connected:
                    return;
                default:
                    break;
            }

            mConnString = connStr;
            mSocket = new WebSocket(mConnString);
            mSocket.OnOpen += onOpen;
            mSocket.OnClose += onClose;
            mSocket.OnError += onError;
            mSocket.OnMessage += onMessage;

            mSocket.ConnectAsync();
            ChangeState(SessionStateType.Connecting);

            Debug.Log($"Connecting:{mConnString}");
        }


        public void DisConnect()
        {
            if (mSocket == null)
            {
                ChangeState(SessionStateType.DisConnected);
                return;
            }

            switch (mSocket.ReadyState)
            {
                case WebSocketState.Connecting:
                case WebSocketState.Open:
                    mSocket.CloseAsync();
                    ChangeState(SessionStateType.DisConnecting);
                    break;
                default:
                    break;
            }
        }


        void onOpen(object sender, EventArgs evt)
        {
            ChangeState(SessionStateType.Connected);

            mDispatcher.Invoke((args) =>
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

            mDispatcher.Invoke((args) =>
            {
                OnClose?.Invoke(this, (CloseEventArgs)args[0]);
            }, evt);
        }

        void onError(object sender, ErrorEventArgs evt)
        {
            mDispatcher.Invoke((args) =>
            {
                OnError?.Invoke(this, (ErrorEventArgs)args[0]);
            }, evt);
        }

        void onMessage(object sender, MessageEventArgs evt)
        {
            mDispatcher.Invoke((args) =>
            {
                var msg = (MessageEventArgs)args[0];
                ReceiveData(msg.RawData);
            }, evt);
        }
    }
}

