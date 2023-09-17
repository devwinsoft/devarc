using System;
using UnityEngine;

namespace Devarc
{
    public abstract class CData<T>
    {
        protected abstract T get();
        protected abstract void set(T value);

        [UnityEngine.SerializeField]
        public int data1;

        [UnityEngine.SerializeField]
        public int data2;

        protected bool isValid = false;
        protected int crc = 0;

        public bool IsValid
        {
            get { return isValid; }
        }

        public T Value
        {
            get { return get(); }
            set { set(value); }
        }

        public void Init(int _data1, int _data2)
        {
            data1 = _data1;
            data2 = _data2;
            setupCRC(BitConverter.GetBytes(_data1), BitConverter.GetBytes(_data2));
        }

        protected int getRandom()
        {
            int value = 0;
#if UNITY_EDITOR
#else
            value = UnityEngine.Random.Range(-10000, 10000);
#endif
            return value;
        }

        protected void setupCRC(byte[] temp1, byte[] temp2)
        {
            isValid = true;
            crc = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                crc += (i + 1) * temp1[i];
                crc += (i + 2) * temp2[i];
            }
        }
    }
}
