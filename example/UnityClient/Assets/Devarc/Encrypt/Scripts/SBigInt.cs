using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    [Serializable]
    public struct SBigInt : IComparable, IComparable<SBigInt>
    {
        public static SBigInt Zero = new SBigInt(0f, 0);
        public static SBigInt Max = new SBigInt(1f, int.MaxValue);
        public static SBigInt Min = new SBigInt(-1f, int.MaxValue);
        public static SBigInt MaxDouble = new SBigInt(double.MaxValue);
        public static SBigInt MaxFloat = new SBigInt(float.MaxValue);

        static string[] symbol_0 = new string[] { "", "K", "M", "G", "T"};
        static string[] symbol_1 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        [SerializeField] SFloat mBase; // < 10.0
        [SerializeField] SInt mPow;

        public SBigInt(float _base, int _pow)
        {
            if (_base == 0)
            {
                mBase = 0f;
                mPow = 0;
                return;
            }
            float tmpBase = _base;
            int tmpPow = _pow;
            while (Mathf.Abs(tmpBase) > 10f)
            {
                tmpBase /= 10f;
                tmpPow++;
            }
            while (Mathf.Abs(tmpBase) < 1f)
            {
                tmpBase *= 10f;
                tmpPow--;
            }
            mBase = tmpBase;
            mPow = tmpPow;
        }

        public SBigInt(double value)
        {
            if (value == 0)
            {
                mBase = 0f;
                mPow = 0;
                return;
            }
            double tmpBase = value;
            int tmpPow = 0;
            while (abs(tmpBase) > 10.0)
            {
                tmpBase /= 10f;
                tmpPow++;
            }
            while (abs(tmpBase) < 1f)
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

        static double abs(double value)
        {
            return value >= 0 ? value : -value;
        }

        static (float mBase, int mPow) getData(float _base, int _pow)
        {
            if (_base == 0f)
            {
                return (0, 0);
            }

            double tmpBase = _base;
            int tmpPow = _pow;
            while (abs(tmpBase) > 10.0)
            {
                tmpBase /= 10f;
                tmpPow++;
            }
            while (abs(tmpBase) < 1f)
            {
                tmpBase *= 10f;
                tmpPow--;
            }

            return ((float)tmpBase, tmpPow);
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                var value = (int)obj;
                return CompareTo(new SBigInt(value));
            }
            if (obj.GetType() == typeof(float))
            {
                var value = (float)obj;
                return CompareTo(new SBigInt(value));
            }
            if (obj.GetType() == typeof(double))
            {
                var value = (double)obj;
                return CompareTo(new SBigInt(value));
            }
            Debug.LogError($"[BigInt::CompareTo] Not implemented: type={obj.GetType()}");
            return 0;
        }

        public int CompareTo(SBigInt other)
        {
            if (this.mBase > 0)
            {
                if (other.mBase <= 0f)
                    return 1;
            }
            else if (this.mBase < 0f)
            {
                if (other.mBase >= 0f)
                    return -1;
            }
            else
            {
                if (other.mBase > 0f)
                    return 1;
                else if (other.mBase < 0f)
                    return -1;
                else
                    return 0;
            }
            int result = this.mBase > 0 ? 1 : -1;
            if (this.mPow > other.mPow)
                return result;
            if (this.mPow < other.mPow)
                return -result;
            if (this.mBase > other.mBase)
                return result;
            if (this.mBase < other.mBase)
                return -result;
            return 0;
        }

        public static implicit operator double(SBigInt obj)
        {
            if (obj > SBigInt.MaxDouble)
            {
                throw new OverflowException($"OverflowException: value={obj.ToString()}");
            }
            double value = obj.mBase.GetValue() * UnityEngine.Mathf.Pow(10f, (float)obj.mPow.GetValue());
            return value;
        }

        public static implicit operator float(SBigInt obj)
        {
            if (obj > SBigInt.MaxFloat)
            {
                throw new OverflowException($"OverflowException: value={obj.ToString()}");
            }
            float value = obj.mBase.GetValue() * UnityEngine.Mathf.Pow(10f, (float)obj.mPow.GetValue());
            return value;
        }

        public static SBigInt operator +(SBigInt p1, SBigInt p2)
        {
            float tmpBase = 0f;
            int tmpPow = Mathf.Max(p1.mPow, p2.mPow);
            tmpBase += Mathf.Pow(0.1f, tmpPow - p1.mPow) * p1.mBase;
            tmpBase += Mathf.Pow(0.1f, tmpPow - p2.mPow) * p2.mBase;

            var data = getData(tmpBase, tmpPow);
            var value = new SBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static SBigInt operator -(SBigInt p1, SBigInt p2)
        {
            float tmpBase = 0f;
            int tmpPow = Mathf.Max(p1.mPow, p2.mPow);
            tmpBase += Mathf.Pow(0.1f, tmpPow - p1.mPow) * p1.mBase;
            tmpBase -= Mathf.Pow(0.1f, tmpPow - p2.mPow) * p2.mBase;

            var data = getData(tmpBase, tmpPow);
            var value = new SBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static SBigInt operator *(SBigInt p1, SBigInt p2)
        {
            var data = getData(p1.mBase * p2.mBase, p1.mPow + p2.mPow);
            var value = new SBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static SBigInt operator /(SBigInt p1, SBigInt p2)
        {
            if (p2.mBase == 0f)
            {
                throw new DivideByZeroException();
            }
            var data = getData(p1.mBase / p2.mBase, p1.mPow - p2.mPow);
            var value = new SBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static SBigInt operator /(SBigInt p1, float p2)
        {
            if (p2 == 0f)
            {
                if (p1.mBase >= 0f)
                    return SBigInt.Max;
                else
                    return SBigInt.Min;
            }
            var data = getData(p1.mBase / p2, p1.mPow);
            var value = new SBigInt();
            value.mBase = data.mBase;
            value.mPow = data.mPow;
            return value;
        }

        public static bool operator <(SBigInt p1, SBigInt p2)
        {
            return p1.CompareTo(p2) < 0;
        }
        public static bool operator >(SBigInt p1, SBigInt p2)
        {
            return p1.CompareTo(p2) > 0;
        }

        public static bool operator <=(SBigInt p1, SBigInt p2)
        {
            return p1.CompareTo(p2) <= 0;
        }
        public static bool operator >=(SBigInt p1, SBigInt p2)
        {
            return p1.CompareTo(p2) >= 0;
        }
    }
}
