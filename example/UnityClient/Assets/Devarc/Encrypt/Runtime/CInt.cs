using System;
using UnityEngine;

namespace Devarc
{
    [System.Serializable]
    public class CInt : CData<int>
    {
        public static implicit operator int(CInt obj)
        {
            if (obj == null)
                return 0;
            return obj.get();
        }

        public static implicit operator float(CInt obj)
        {
            if (obj == null)
                return 0;
            return obj.get();
        }

        public static implicit operator CInt(int _value)
        {
            return new CInt(_value);
        }

        public CInt()
        {
            set(0);
        }

        public CInt(int _value)
        {
            set(_value);
        }

        protected override int get()
        {
            var temp = new byte[4];
            var temp1 = BitConverter.GetBytes(data1);
            var temp2 = BitConverter.GetBytes(data2);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (temp1[i] ^ temp2[i]));
            }

            int tempCRC = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                tempCRC += (i + 1) * temp1[i];
                tempCRC += (i + 2) * temp2[i];
            }
            if (isValid && tempCRC != crc)
            {
                UnityEngine.Debug.LogError("[CInt::get] CRC Error");
            }

            int value = BitConverter.ToInt32(temp, 0);
            return value;
        }

        protected override void set(int value)
        {
            byte[] src = BitConverter.GetBytes(value);
            byte[] xor = BitConverter.GetBytes(getRandom());
            byte[] temp1 = new byte[4];
            byte[] temp2 = new byte[4];

            for (int i = 0; i < temp1.Length; i++)
            {
                temp1[i] = (byte)(0xff & (src[i] ^ xor[i]));
                temp2[i] = xor[i];
            }

            crc = 0;
            for (int i = 0; i < temp1.Length; i++)
            {
                crc += (i + 1) * temp1[i];
                crc += (i + 2) * temp2[i];
            }

            isValid = true;
            data1 = BitConverter.ToInt32(temp1, 0);
            data2 = BitConverter.ToInt32(temp2, 0);
        }

//#if UNITY_EDITOR
//        public int ReadFrom(UnityEditor.SerializedProperty property)
//        {
//            UnityEditor.SerializedProperty propData1 = property.FindPropertyRelative("data1");
//            UnityEditor.SerializedProperty propData2 = property.FindPropertyRelative("data2");
            
//            var temp1 = BitConverter.GetBytes(propData1.intValue);
//            var temp2 = BitConverter.GetBytes(propData2.intValue);

//            for (int i = 0; i < propData1.arraySize; i++)
//            {
//                data1[i] = propData1.GetArrayElementAtIndex(i).intValue;
//            }
//            for (int i = 0; i < propData2.arraySize; i++)
//            {
//                data2[i] = propData2.GetArrayElementAtIndex(i).intValue;
//            }
//            return Value;
//        }

//        public void WriteTo(UnityEditor.SerializedProperty property)
//        {
//            UnityEditor.SerializedProperty propData1 = property.FindPropertyRelative("data1");
//            UnityEditor.SerializedProperty propData2 = property.FindPropertyRelative("data2");
//            for (int i = 0; i < propData1.arraySize; i++)
//            {
//                propData1.GetArrayElementAtIndex(i).intValue = data1[i];
//            }
//            for (int i = 0; i < propData2.arraySize; i++)
//            {
//                propData2.GetArrayElementAtIndex(i).intValue = data2[i];
//            }
//        }
//#endif
    }
}

