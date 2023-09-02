using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Devarc
{
    public abstract class SocketUser : WebSocketBehavior
    {
        Dictionary<string, PacketHandler> mHandlers = new Dictionary<string, PacketHandler>();

        // Todo: Refactoring
        protected PacketEncoder mEncoder = new PacketEncoder();

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                (var typeName, var packetData) = mEncoder.Parse(e.RawData);

                PacketHandler handler = null;
                if (mHandlers.TryGetValue(typeName, out handler))
                {
                    handler.Invoke(packetData, mEncoder.Options);
                }
            }
        }

        protected void InitMessagePackResolvers(params IFormatterResolver[] resolvers)
        {
            var formatterResolver = CompositeResolver.Create(resolvers);
            mEncoder.Init(formatterResolver);
        }


        protected void RegisterHandler<T>(PacketCallback<T> callback) where T : class
        {
            Type type = typeof(T);
            PacketHandler<T> handler = new PacketHandler<T>();
            handler.typeName = type.Name;
            handler.callback = callback;
            mHandlers.Add(handler.typeName, handler);
        }


        protected void SendData<T>(T obj)
        {
            try
            {
                var data = mEncoder.Pack(obj);
                Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        protected bool ReceiveData(byte[] rawData)
        {
            try
            {
                (string typeName, byte[] encoded) = mEncoder.Parse(rawData);

                PacketHandler handler;
                if (mHandlers.TryGetValue(typeName, out handler) == false)
                {
                    return false;
                }
                handler.Invoke(encoded, mEncoder.Options);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
