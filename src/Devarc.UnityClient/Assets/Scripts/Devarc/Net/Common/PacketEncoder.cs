using MessagePack;
using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace Devarc
{
    public class PacketEncoder
    {
        MemoryStream mStream = new MemoryStream();
        ReadOnlySequence<byte> mBuffer;
        int mPosition = 0;


        public byte[] Pack<T>(T obj)
        {
            var type = typeof(T);
            write(type.Name);
            write(MessagePackSerializer.Serialize<T>(obj));
            return mStream.ToArray();
        }


        public T UnPack<T>(byte[] rawData)
        {
            mBuffer = new ReadOnlySequence<byte>(rawData);
            mPosition = 0;

            var typeName = read_String();
            var encoded = read_Remain();
            var obj = MessagePackSerializer.Deserialize<T>(encoded);
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
            var obj = MessagePackSerializer.Deserialize<T>(encoded);

            return obj;
        }


        public (string typeName, byte[] encoded) Parse(byte[] rawData)
        {
            mBuffer = new ReadOnlySequence<byte>(rawData);
            mPosition = 0;

            var typeName = read_String();
            var encoded = read_Remain();
            return (typeName, encoded);
        }


        short read_Byte()
        {
            var data = mBuffer.Slice(mPosition, 1);
            var value = data.ToArray()[0];
            mPosition += 1;
            return value;
        }

        short read_Int16()
        {
            var data = mBuffer.Slice(mPosition, 2);
            var value = BitConverter.ToInt16(data.ToArray(), 0);
            mPosition += 2;
            return value;
        }


        string read_String()
        {
            short size = read_Int16();
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

        void write(byte value)
        {
            mStream.WriteByte(value);
        }

        void write(short value)
        {
            var data = BitConverter.GetBytes(value);
            mStream.Write(data, 0, data.Length);
        }

        void write(int value)
        {
            var data = BitConverter.GetBytes(value);
            mStream.Write(data, 0, data.Length);
        }

        void write(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            write((short)data.Length);
            mStream.Write(data, 0, data.Length);
        }

        void write(byte[] data)
        {
            mStream.Write(data, 0, data.Length);
        }
    }

}