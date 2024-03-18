using System;
using UnityEngine;
using MessagePack;

namespace Devarc
{
    [Serializable]
    [MessagePackObject]
    public abstract class CData
    {
        protected static System.Random random = new System.Random();

        [SerializeField]
		[Key(0)]
        public int data1;

        [SerializeField]
		[Key(1)]
        public int data2;

        protected bool isValid = false;
        protected int crc = 0;

        public bool IsValid
        {
            get { return isValid; }
        }

        public void Init(int _data1, int _data2)
        {
            data1 = _data1;
            data2 = _data2;
        }

        protected int getRandom()
        {
            int value = random.Next();
            return value;
        }
    }

    public abstract class CData<T> : CData
    {
        protected abstract T get();
        protected abstract void set(T value);

        public T Value
        {
            get { return get(); }
            set { set(value); }
        }
    }
}
