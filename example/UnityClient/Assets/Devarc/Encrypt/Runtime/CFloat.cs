using System;

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
            isValid = false;
        }

        public CFloat(float _value)
        {
            seed();
            set(_value);
        }

        protected override float get()
        {
            byte[] temp = new byte[4];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (data1[i] ^ data2[i]));
            }

            int tempCRC = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                tempCRC += (i + 1) * data1[i];
                tempCRC += (i + 2) * data2[i];
            }

            if (isValid && tempCRC != crc)
            {
                UnityEngine.Debug.LogError("Memory Hack");
                //CEncryptErrors.Instance.Add(LT_ABUSE_TYPE.MEMORY, "CFloatEx CRC Error");
            }
            return BitConverter.ToSingle(temp, 0);
        }

        protected override void set(float value)
        {
            byte[] src = BitConverter.GetBytes(value);
            byte[] xor = BitConverter.GetBytes(UnityEngine.Random.Range(-10000, 10000));
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (0xff & (src[i] ^ xor[i])) | getSeedValue();
                data2[i] = xor[i] | getSeedValue();
            }

            crc = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                crc += (i + 1) * data1[i];
                crc += (i + 2) * data2[i];
            }

            isValid = true;
        }
    }
}
