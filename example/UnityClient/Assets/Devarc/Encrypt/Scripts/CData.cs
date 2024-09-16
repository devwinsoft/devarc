using System;
using UnityEngine;
using MessagePack;

namespace Devarc
{
    [Serializable]
    public class CData
    {
        protected static System.Random random = new System.Random();

        [SerializeField]
		[Key(0)]
        public int save1;

        [SerializeField]
		[Key(1)]
        public int save2;

        protected bool isValid = false;
        protected int crc = 0;

        public bool IsValid
        {
            get { return isValid; }
        }

        public void Init(int _save1, int _save2)
        {
            save1 = _save1;
            save2 = _save2;
        }

        protected int getRandom()
        {
            int value = random.Next();
            return value;
        }


        public bool getData(out byte[] value)
        {
            value = new byte[4];
            var temp1 = BitConverter.GetBytes(save1);
            var temp2 = BitConverter.GetBytes(save2);

            for (int i = 0; i < value.Length; i++)
            {
                value[i] = (byte)(0xff & (temp1[i] ^ temp2[i]));
            }

            int tempCRC = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                tempCRC += (i + 1) * temp1[i];
                tempCRC += (i + 2) * temp2[i];
            }
            if (isValid && tempCRC != crc)
            {
                UnityEngine.Debug.LogError("[CData::getData] CRC Error");
                return false;
            }
            return true;
        }

        public void SetData(byte[] value)
        {
            byte[] xor = BitConverter.GetBytes(getRandom());
            byte[] temp1 = new byte[4];
            byte[] temp2 = new byte[4];

            for (int i = 0; i < temp1.Length; i++)
            {
                temp1[i] = (byte)(0xff & (value[i] ^ xor[i]));
                temp2[i] = xor[i];
            }

            save1 = BitConverter.ToInt32(temp1, 0);
            save2 = BitConverter.ToInt32(temp2, 0);
        }
    }
}
