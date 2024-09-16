using System;

namespace Devarc
{
    [System.Serializable]
    public class CInt : BaseTableElement<CInt>, ITableElement<CInt>
    {
        public CData data = new CData();

        public CInt()
        {
            SetValue(0);
        }

        public CInt(int _value)
        {
            SetValue(_value);
        }

        public int GetValue()
        {
            byte[] temp = null;
            if (data.getData(out temp))
            {
                int value = BitConverter.ToInt32(temp, 0);
                return value;
            }
            return 0;
        }

        public void SetValue(int value)
        {
            byte[] src = BitConverter.GetBytes(value);
            data.SetData(src);
        }

        public override CInt Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            int result;
            int.TryParse(value, out result);
            return new CInt(result);
        }

        public static implicit operator int(CInt obj)
        {
            if (obj == null)
                return 0;
            return obj.GetValue();
        }

        public static implicit operator CInt(int value)
        {
            return new CInt(value);
        }
    }
}

