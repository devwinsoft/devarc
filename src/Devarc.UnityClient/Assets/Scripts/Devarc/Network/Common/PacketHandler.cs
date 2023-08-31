using MessagePack;
using System;


namespace Devarc
{
    public delegate void PacketCallback<T>(T packet);

    public abstract class PacketHandler
    {
        public string typeName;

        public abstract void Invoke(byte[] encoded);
    }

    public class PacketHandler<T> : PacketHandler
    {
        public PacketCallback<T> callback;

        public override void Invoke(byte[] encoded)
        {
            var obj = MessagePackSerializer.Deserialize<T>(encoded);
            callback.Invoke(obj);
        }
    }
}


