using MessagePack;

namespace Devarc
{
    public delegate void PacketCallback<T>(T packet);

    public abstract class PacketHandler
    {
        public string typeName;

        public abstract void Invoke(byte[] packetData, MessagePackSerializerOptions options);
    }

    public class PacketHandler<T> : PacketHandler
    {
        public PacketCallback<T> callback;

        public override void Invoke(byte[] packetData, MessagePackSerializerOptions options)
        {
            var obj = MessagePackSerializer.Deserialize<T>(packetData, options);
            callback.Invoke(obj);
        }
    }
}