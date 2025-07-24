using System;
using System.Buffers;
using System.IO;
using System.Text;
using MessagePack;

namespace Devarc
{
    public class PacketEncoder
    {
        private ReadOnlySequence<byte> mBuffer;
        private int mPosition;

        private readonly MemoryStream mStream = new();
        public MessagePackSerializerOptions Options { get; private set; }

        public void Init(IFormatterResolver formatterResolver)
        {
            Options = MessagePackSerializerOptions.Standard.WithResolver(formatterResolver);
        }

        public byte[] Pack<T>(T obj)
        {
            mStream.SetLength(0);

            var type = typeof(T);
            write(type.Name);
            write(MessagePackSerializer.Serialize(obj, Options));
            return mStream.ToArray();
        }


        public T UnPack<T>(byte[] rawData)
        {
            mBuffer = new ReadOnlySequence<byte>(rawData);
            mPosition = 0;

            var typeName = read_String();
            var encoded = read_Remain();
            var obj = MessagePackSerializer.Deserialize<T>(encoded, Options);

            return obj;
        }


        public string ToBase64<T>(T obj)
        {
            var data = Pack(obj);
            var encoded = Convert.ToBase64String(data);
            return encoded;
        }


        public T FromBase64<T>(string base64)
        {
            var rawData = Convert.FromBase64String(base64);
            mBuffer = new ReadOnlySequence<byte>(rawData);
            mPosition = 0;

            var typeName = read_String();
            var encoded = read_Remain();
            var obj = MessagePackSerializer.Deserialize<T>(encoded, Options);

            return obj;
        }


        public (string typeName, byte[] packetData) Parse(byte[] rawData)
        {
            mBuffer = new ReadOnlySequence<byte>(rawData);
            mPosition = 0;

            var typeName = read_String();
            var encoded = read_Remain();
            return (typeName, encoded);
        }


        private short read_Byte()
        {
            var data = mBuffer.Slice(mPosition, 1);
            var value = data.ToArray()[0];
            mPosition += 1;
            return value;
        }

        private short read_Int16()
        {
            var data = mBuffer.Slice(mPosition, 2);
            var value = BitConverter.ToInt16(data.ToArray(), 0);
            mPosition += 2;
            return value;
        }


        private string read_String()
        {
            var size = read_Int16();
            var data = mBuffer.Slice(mPosition, size);
            mPosition += size;
            var value = Encoding.UTF8.GetString(data.ToArray(), 0, size);
            return value;
        }

        public byte[] read_Bytes(int size)
        {
            var data = mBuffer.Slice(mPosition, size);
            mPosition += size;
            var value = data.ToArray();
            return value;
        }

        public byte[] read_Remain()
        {
            var size = (int)mBuffer.Length - mPosition;
            var data = mBuffer.Slice(mPosition, size);
            mPosition += size;
            var value = data.ToArray();
            return value;
        }

        private void write(byte value)
        {
            mStream.WriteByte(value);
        }

        private void write(short value)
        {
            var data = BitConverter.GetBytes(value);
            mStream.Write(data, 0, data.Length);
        }

        private void write(int value)
        {
            var data = BitConverter.GetBytes(value);
            mStream.Write(data, 0, data.Length);
        }

        private void write(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            write((short)data.Length);
            mStream.Write(data, 0, data.Length);
        }

        private void write(byte[] data)
        {
            mStream.Write(data, 0, data.Length);
        }
    }
}