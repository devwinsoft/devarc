using System;

namespace Devarc
{
    public class SFloat
    {
        public static implicit operator float(SFloat obj)
        {
            if (obj == null)
                return 0f;
            return obj.get();
        }
        public static implicit operator SFloat(float _value)
        {
            return new SFloat(_value);
        }

        public SFloat()
        {
            seed();
        }

        public SFloat(float _value)
        {
            seed();
            set(_value);
        }

        public float Value
        {
            get { return get(); }
        }

        static byte[] temp = new byte[4];
        byte[] data1 = new byte[4];
        byte[] data2 = new byte[4];

        float get()
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (data1[i] ^ data2[i]));
            }
            temp[0] = (byte)(0xff & (data1[0] ^ data2[0]));
            temp[1] = (byte)(0xff & (data1[2] ^ data2[2]));
            temp[2] = (byte)(0xff & (data1[1] ^ data2[1]));
            temp[3] = (byte)(0xff & (data1[3] ^ data2[3]));
            return BitConverter.ToSingle(temp, 0);
        }

        void set(float value)
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

