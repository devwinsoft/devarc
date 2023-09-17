using System;
using UnityEngine;

namespace Devarc
{
    [System.Serializable]
    public class CFloat : CData<float>
    {
        public static implicit operator float(CFloat obj)
        {
            if (obj == null)
                return 0f;
            return obj.get();
        }

        public static implicit operator CFloat(float _value)
        {
            return new CFloat(_value);
        }

        public CFloat()
        {
            set(0f);
        }

        public CFloat(float _value)
        {
            set(_value);
        }

        protected override float get()
        {
            byte[] temp = new byte[4];
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
                UnityEngine.Debug.LogError("[CFloat::get] CRC Error");
            }

            float value = BitConverter.ToSingle(temp, 0);
            return value;
        }


        protected override void set(float value)
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

            crc = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                crc += (i + 1) * temp1[i];
                crc += (i + 2) * temp2[i];
            }

            isValid = true;
            data1 = BitConverter.ToInt32(temp1, 0);
            data2 = BitConverter.ToInt32(temp2, 0);
        }
    }
}
