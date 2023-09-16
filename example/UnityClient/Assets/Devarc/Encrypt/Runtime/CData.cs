namespace Devarc
{
    public abstract class CData<T>
    {
        protected abstract T get();
        protected abstract void set(T value);

        [UnityEngine.SerializeField]
        public int[] data1 = new int[4];

        [UnityEngine.SerializeField]
        public int[] data2 = new int[4];

        protected bool isValid = false;
        protected int crc = 0;
        protected byte[] seedAry = null;
        protected static int index = 0;

        public bool IsValid
        {
            get { return isValid; }
        }

        public T Value
        {
            get { return get(); }
            set { set(value); }
        }

        public void Init(int[] _data1, int[] _data2)
        {
            if (_data1.Length == data1.Length && _data2.Length == data2.Length)
            {
                _data1.CopyTo(data1, 0);
                _data2.CopyTo(data2, 0);
            }
            else
            {
                isValid = false;
            }
        }

        protected void seed()
        {
            if (seedAry != null)
            {
                return;
            }
            seedAry = new byte[7] { 0, 1, 2, 3, 4, 5, 6 };
            try
            {
                for (int i = 0; i < seedAry.Length; i++)
                {
                    seedAry[i] = (byte)UnityEngine.Random.Range(0, 255);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
        }

        protected int getSeedValue()
        {
            seed();

            int value = 0;
            value |= seedAry[index] << 8;
            index = (index + 1) % seedAry.Length;
            value |= seedAry[index] << 16;
            index = (index + 1) % seedAry.Length;
            value |= seedAry[index] << 24;
            index = (index + 1) % seedAry.Length;
            return value;
        }
    }
}
