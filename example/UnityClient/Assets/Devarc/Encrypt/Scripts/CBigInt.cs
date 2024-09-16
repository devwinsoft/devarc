using System;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Devarc
{
    [MessagePackObject]
    [Serializable]
    public class CBigInt : BaseTableElement<CBigInt>, ITableElement<CBigInt>, IComparable, IComparable<CBigInt>
    {
        static string[] symbol_0 = new string[] { "", "K", "M", "G", "T"};
        static string[] symbol_1 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        [Key(0)]
        public CFloat mBase; // < 10

        [Key(1)]
        public CInt mPow;

        public CBigInt()
        {
            mBase = null;
            mPow = null;
        }

        public CBigInt(float _base, int _pow)
        {
            var tmpBase = 1f;
            var tmpPow = 0;
            for (int i = 0; i < _pow; i++)
            {
                tmpBase *= _base;
                while (tmpBase > 10f)
                {
                    tmpBase /= 10f;
                    tmpPow++;
                }
                while (tmpBase < 1f)
                {
                    tmpBase *= 10f;
                    tmpPow--;
                }
            }
            mBase = tmpBase;
            mPow = tmpPow;
        }

        public CBigInt(double value)
        {
            var tmpBase = value;
            var tmpPow = 0;
            while (tmpBase > 10f)
            {
                tmpBase /= 10f;
                tmpPow++;
            }
            while (tmpBase < 1f)
            {
                tmpBase *= 10f;
                tmpPow--;
            }
            mBase = (float)tmpBase;
            mPow = tmpPow;
        }

        public override string ToString()
        {
            if (mPow < 3)
            {
                var fValue = mBase.GetValue() * Mathf.Pow(10f, mPow.GetValue());
                var iValue = Mathf.RoundToInt(fValue);
                return iValue.ToString();
            }
            else
            {
                var mode = mPow % 3;
                var display = mBase.GetValue() * Mathf.Pow(10f, mode);
                var symbol = getSymbol();
                var remain = Mathf.RoundToInt(display * 100f) % 100;
                if (remain == 0)
                {
                    return string.Format("{0:N0} {1}", display, symbol);
                }
                else if ((remain % 10) == 0)
                {
                    return string.Format("{0:N1} {1}", display, symbol);
                }
                else
                {
                    return string.Format("{0:N2} {1}", display, symbol);
                }
            }
        }

        string getSymbol()
        {
            int i = Mathf.Max(0, mPow / 3);
            if (i < symbol_0.Length)
                return symbol_0[i];

            i = Mathf.Max(0, (mPow / 3) - symbol_0.Length);
            if (i < symbol_1.Length)
                return symbol_1[i];

            List<char> list = new List<char>(4);
            do
            {
                list.Insert(0, symbol_1[i % symbol_1.Length][0]);
                i /= symbol_1.Length;
                i--;
            } while (i >= 0);

            return new string(list.ToArray());
        }

        static (float mBase, int mPow) getData(float _base, int _pow)
        {
            while (_base > 10f)
            {
                _base /= 10f;
                _pow++;
            }
            while (_base < 1f)
            {
                _base *= 10f;
                _pow--;
            }
            return (_base, _pow);
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                var value = (int)obj;
                return CompareTo(new CBigInt(value));
            }
            if (obj.GetType() == typeof(float))
            {
                var value = (float)obj;
                return CompareTo(new CBigInt(value));
            }
            if (obj.GetType() == typeof(double))
            {
                var value = (double)obj;
                return CompareTo(new CBigInt(value));
            }
            Debug.LogError($"[BigInt::CompareTo] Not implemented: type={obj.GetType()}");
            return 0;
        }

        public int CompareTo(CBigInt other)
        {
            if (this.mPow > other.mPow)
                return 1;
            if (this.mPow < other.mPow)
                return -1;
            if (this.mBase > other.mBase)
                return 1;
            if (this.mBase < other.mBase)
                return -1;
            return 0;
        }

        public static CBigInt operator +(CBigInt p1, CBigInt p2)
        {
            float tmpBase = 0f;
            int tmpPow = Mathf.Max(p1.mPow, p2.mPow);
            tmpBase += Mathf.Pow(0.1f, tmpPow - p1.mPow) * p1.mBase;
            tmpBase += Mathf.Pow(0.1f, tmpPow - p2.mPow) * p2.mBase;

            var data = getData(tmpBase, tmpPow);
            var value = new CBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static CBigInt operator *(CBigInt p1, CBigInt p2)
        {
            var data = getData(p1.mBase * p2.mBase, p1.mPow + p2.mPow);
            var value = new CBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static CBigInt operator *(CBigInt p1, double p2)
        {
            return p1 * new CBigInt(p2);
        }

        public static CBigInt operator /(CBigInt p1, CBigInt p2)
        {
            var data = getData(p1.mBase / p2.mBase, p1.mPow - p2.mPow);
            var value = new CBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static CBigInt operator /(CBigInt p1, double p2)
        {
            return p1 / new CBigInt(p2);
        }


        public static bool operator <(CBigInt p1, CBigInt p2)
        {
            return p1.CompareTo(p2) < 0;
        }

        public static bool operator >(CBigInt p1, CBigInt p2)
        {
            return p1.CompareTo(p2) > 0;
        }
    }
}
