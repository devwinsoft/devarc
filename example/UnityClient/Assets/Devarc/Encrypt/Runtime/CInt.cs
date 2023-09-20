using System;
using UnityEngine;

namespace Devarc
{
    [System.Serializable]
    public class CInt : CData<int>
    {
        public static implicit operator int(CInt obj)
        {
            if (obj == null)
                return 0;
            return obj.get();
        }

        public static implicit operator CInt(int value)
        {
            return new CInt(value);
        }

        public CInt()
        {
            set(0);
        }

        public CInt(int _value)
        {
            set(_value);
        }

        protected override int get()
        {
            var temp = new byte[4];
            var temp1 = BitConverter.GetBytes(data1);
            var temp2 = BitConverter.GetBytes(data2);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (temp1[i] ^ temp2[i]));
            }

            int tempCRC = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                tempCRC += (i + 1) * temp1[i];
                tempCRC += (i + 2) * temp2[i];
            }
            if (isValid && tempCRC != crc)
            {
                UnityEngine.Debug.LogError("[CInt::get] CRC Error");
            }

            int value = BitConverter.ToInt32(temp, 0);
            return value;
        }

        protected override void set(int value)
        {
            byte[] src = BitConverter.GetBytes(value);
            byte[] xor = BitConverter.GetBytes(getRandom());
            byte[] temp1 = new byte[4];
            byte[] temp2 = new byte[4];

            for (int i = 0; i < temp1.Length; i++)
            {
                temp1[i] = (byte)(0xff & (src[i] ^ xor[i]));
                temp2[i] = xor[i];
            }

            data1 = BitConverter.ToInt32(temp1, 0);
            data2 = BitConverter.ToInt32(temp2, 0);

            setupCRC(temp1, temp2);
        }
    }
}

