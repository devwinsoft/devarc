using System;
using UnityEngine;

namespace Devarc
{
    [System.Serializable]
    public class CFloat : BaseTableElement<CFloat>, ITableElement<CFloat>
    {
        public CData data = new CData();
   
        public CFloat()
        {
            SetValue(0f);
        }

        public CFloat(float _value)
        {
            SetValue(_value);
        }

        public float GetValue()
        {
            byte[] temp = null;
            if (data.getData(out temp))
            {
                float value = BitConverter.ToSingle(temp, 0);
                return value;
            }
            return 0f;
        }

        public void SetValue(float value)
        {
            byte[] src = BitConverter.GetBytes(value);
            data.SetData(src);
        }

        public override CFloat Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            float result;
            float.TryParse(value, out result);
            return new CFloat(result);
        }

        public static implicit operator float(CFloat obj)
        {
            if (obj == null)
                return 0f;
            return obj.GetValue();
        }

        public static implicit operator CFloat(float _value)
        {
            return new CFloat(_value);
        }
    }
}
