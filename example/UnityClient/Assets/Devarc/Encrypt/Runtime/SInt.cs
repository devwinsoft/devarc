using System;

namespace Devarc
{
    public class SInt
    {
        public static implicit operator int(SInt obj)
        {
            if (obj == null)
                return 0;
            return obj.get();
        }
        public static implicit operator ulong(SInt obj)
        {
            if (obj == null)
                return 0;
            return (ulong)obj.get();
        }
        public static implicit operator SInt(int _value)
        {
            return new SInt(_value);
        }

        public SInt()
        {
            seed();
        }

        public SInt(int _value)
        {
            seed();
            set(_value);
        }

        public int Value
        {
            get { return get(); }
            set { set(value); }
        }

        static byte[] temp = new byte[4];
        byte[] data1 = new byte[4];
        byte[] data2 = new byte[4];

        int get()
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (data1[i] ^ data2[i]));
            }
            temp[0] = (byte)(0xff & (data1[0] ^ data2[0]));
            temp[1] = (byte)(0xff & (data1[2] ^ data2[2]));
            temp[2] = (byte)(0xff & (data1[1] ^ data2[1]));
            temp[3] = (byte)(0xff & (data1[3] ^ data2[3]));
            return BitConverter.ToInt32(temp, 0);
        }

        void set(int value)
        {
            byte[] src = BitConverter.GetBytes(value);
            data1[0] = (byte)(0xff & (src[0] ^ data2[0]));
            data1[1] = (byte)(0xff & (src[2] ^ data2[1]));
            data1[2] = (byte)(0xff & (src[1] ^ data2[2]));
            data1[3] = (byte)(0xff & (src[3] ^ data2[3]));
        }

        void seed()
        {
            Random random = new Random();
            for (int i = 0; i < data2.Length; i++)
            {
                data2[i] = (byte)(0xff & random.Next());
            }
        }
    }
}

