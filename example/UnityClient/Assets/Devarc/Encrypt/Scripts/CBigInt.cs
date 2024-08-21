using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Devarc
{
    public struct CBigInt : IComparable, IComparable<CBigInt>
    {
        static string[] symbol_0 = new string[] { "", "K", "M", "G", "T"};
        static string[] symbol_1 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        //static string[] symbols = new string[]
        //{ "", "K", "M", "G", "T"
        //, "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az"
        //, "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz"
        //, "ca", "cb", "cc", "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn", "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", "cy", "cz"
        //, "da", "db", "dc", "dd", "de", "df", "dg", "dh", "di", "dj", "dk", "dl", "dm", "dn", "do", "dp", "dq", "dr", "ds", "dt", "du", "dv", "dw", "dx", "dy", "dz"
        //, "ea", "eb", "ec", "ed", "ee", "ef", "eg", "eh", "ei", "ej", "ek", "el", "em", "en", "eo", "ep", "eq", "er", "es", "et", "eu", "ev", "ew", "ex", "ey", "ez"
        //, "fa", "fb", "fc", "fd", "fe", "ff", "fg", "fh", "fi", "fj", "fk", "fl", "fm", "fn", "fo", "fp", "fq", "fr", "fs", "ft", "fu", "fv", "fw", "fx", "fy", "fz"
        //, "ga", "gb", "gc", "gd", "ge", "gf", "gg", "gh", "gi", "gj", "gk", "gl", "gm", "gn", "go", "gp", "gq", "gr", "gs", "gt", "gu", "gv", "gw", "gx", "gy", "gz"
        //, "ha", "hb", "hc", "hd", "he", "hf", "hg", "hh", "hi", "hj", "hk", "hl", "hm", "hn", "ho", "hp", "hq", "hr", "hs", "ht", "hu", "hv", "hw", "hx", "hy", "hz"
        //, "ia", "ib", "ic", "id", "ie", "if", "ig", "ih", "ii", "ij", "ik", "il", "im", "in", "io", "ip", "iq", "ir", "is", "it", "iu", "iv", "iw", "ix", "iy", "iz"
        //, "ja", "jb", "jc", "jd", "je", "jf", "jg", "jh", "ji", "jj", "jk", "jl", "jm", "jn", "jo", "jp", "jq", "jr", "js", "jt", "ju", "jv", "jw", "jx", "jy", "jz"
        //, "ka", "kb", "kc", "kd", "ke", "kf", "kg", "kh", "ki", "kj", "kk", "kl", "km", "kn", "ko", "kp", "kq", "kr", "ks", "kt", "ku", "kv", "kw", "kx", "ky", "kz"
        //, "la", "lb", "lc", "ld", "le", "lf", "lg", "lh", "li", "lj", "lk", "ll", "lm", "ln", "lo", "lp", "lq", "lr", "ls", "lt", "lu", "lv", "lw", "lx", "ly", "lz"
        //, "ma", "mb", "mc", "md", "me", "mf", "mg", "mh", "mi", "mj", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz"
        //, "na", "nb", "nc", "nd", "ne", "nf", "ng", "nh", "ni", "nj", "nk", "nl", "nm", "nn", "no", "np", "nq", "nr", "ns", "nt", "nu", "nv", "nw", "nx", "ny", "nz"
        //, "oa", "ob", "oc", "od", "oe", "of", "og", "oh", "oi", "oj", "ok", "ol", "om", "on", "oo", "op", "oq", "or", "os", "ot", "ou", "ov", "ow", "ox", "oy", "oz"
        //, "pa", "pb", "pc", "pd", "pe", "pf", "pg", "ph", "pi", "pj", "pk", "pl", "pm", "pn", "po", "pp", "pq", "pr", "ps", "pt", "pu", "pv", "pw", "px", "py", "pz"
        //, "qa", "qb", "qc", "qd", "qe", "qf", "qg", "qh", "qi", "qj", "qk", "ql", "qm", "qn", "qo", "qp", "qq", "qr", "qs", "qt", "qu", "qv", "qw", "qx", "qy", "qz"
        //, "ra", "rb", "rc", "rd", "re", "rf", "rg", "rh", "ri", "rj", "rk", "rl", "rm", "rn", "ro", "rp", "rq", "rr", "rs", "rt", "ru", "rv", "rw", "rx", "ry", "rz"
        //, "sa", "sb", "sc", "sd", "se", "sf", "sg", "sh", "si", "sj", "sk", "sl", "sm", "sn", "so", "sp", "sq", "sr", "ss", "st", "su", "sv", "sw", "sx", "sy", "sz"
        //, "ta", "tb", "tc", "td", "te", "tf", "tg", "th", "ti", "tj", "tk", "tl", "tm", "tn", "to", "tp", "tq", "tr", "ts", "tt", "tu", "tv", "tw", "tx", "ty", "tz"
        //, "ua", "ub", "uc", "ud", "ue", "uf", "ug", "uh", "ui", "uj", "uk", "ul", "um", "un", "uo", "up", "uq", "ur", "us", "ut", "uu", "uv", "uw", "ux", "uy", "uz"
        //, "va", "vb", "vc", "vd", "ve", "vf", "vg", "vh", "vi", "vj", "vk", "vl", "vm", "vn", "vo", "vp", "vq", "vr", "vs", "vt", "vu", "vv", "vw", "vx", "vy", "vz"
        //, "wa", "wb", "wc", "wd", "we", "wf", "wg", "wh", "wi", "wj", "wk", "wl", "wm", "wn", "wo", "wp", "wq", "wr", "ws", "wt", "wu", "wv", "ww", "wx", "wy", "wz"
        //, "xa", "xb", "xc", "xd", "xe", "xf", "xg", "xh", "xi", "xj", "xk", "xl", "xm", "xn", "xo", "xp", "xq", "xr", "xs", "xt", "xu", "xv", "xw", "xx", "xy", "xz"
        //, "ya", "yb", "yc", "yd", "ye", "yf", "yg", "yh", "yi", "yj", "yk", "yl", "ym", "yn", "yo", "yp", "yq", "yr", "ys", "yt", "yu", "yv", "yw", "yx", "yy", "yz"
        //, "za", "zb", "zc", "zd", "ze", "zf", "zg", "zh", "zi", "zj", "zk", "zl", "zm", "zn", "zo", "zp", "zq", "zr", "zs", "zt", "zu", "zv", "zw", "zx", "zy", "zz"
        //};

        public SFloat mBase; // < 10
        public SInt mPow;

        public CBigInt(float _base, int _pow)
        {
            mBase = _base;
            mPow = _pow;
            while (mBase > 10f)
            {
                mBase /= 10f;
                mPow++;
            }
            while (mBase < 1f)
            {
                mBase *= 10f;
                mPow--;
            }
        }

        public CBigInt(double value)
        {
            mPow = 0;
            var temp = value;
            while (temp > 10f)
            {
                temp /= 10f;
                mPow++;
            }
            while (temp < 1f)
            {
                temp *= 10f;
                mPow--;
            }
            mBase = (float)temp;
        }

        public override string ToString()
        {
            if (mPow < 3)
            {
                var fValue = mBase.Value * Mathf.Pow(10f, mPow.Value);
                var iValue = Mathf.RoundToInt(fValue);
                return iValue.ToString();
            }
            else
            {
                var mode = mPow % 3;
                var display = mBase.Value * Mathf.Pow(10f, mode);
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

            //if (i < symbols.Length)
            //    return symbols[i];
            //return string.Format("E^{0}", i);
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
            var value = new CBigInt();
            value.mPow = Mathf.Max(p1.mPow, p2.mPow);
            value.mBase += Mathf.Pow(0.1f, value.mPow - p1.mPow) * p1.mBase;
            value.mBase += Mathf.Pow(0.1f, value.mPow - p2.mPow) * p2.mBase;
            return value;
        }

        public static CBigInt operator *(CBigInt p1, CBigInt p2)
        {
            var value = new CBigInt();
            value.mPow = p1.mPow + p2.mPow;
            value.mBase = p1.mBase * p2.mBase;
            while (value.mBase > 10f)
            {
                value.mBase /= 10f;
                value.mPow++;
            }
            return value;
        }

        public static CBigInt operator *(CBigInt p1, double p2)
        {
            return p1 * new CBigInt(p2);
        }

        public static CBigInt operator /(CBigInt p1, CBigInt p2)
        {
            var value = new CBigInt();
            value.mPow = p1.mPow - p2.mPow;
            value.mBase = p1.mBase / p2.mBase;
            while (value.mBase < 1f)
            {
                value.mBase *= 10f;
                value.mPow--;
            }
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
