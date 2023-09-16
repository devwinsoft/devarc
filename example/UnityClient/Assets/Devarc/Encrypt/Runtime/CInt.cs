using System;

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
            isValid = false;
        }

        public CInt(int _value)
        {
            seed();
            set(_value);
        }

        protected override int get()
        {
            byte[] temp = new byte[4];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)(0xff & (data1[i] ^ data2[i]));
            }

            int tempCRC = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                tempCRC += (i + 1) * data1[i];
                tempCRC += (i + 2) * data2[i];
            }

            if (isValid && tempCRC != crc)
            {
                //UnityEngine.Debug.LogError(LogHeader.Function + "CIntEx CRC Error");
                //CEncryptErrors.Instance.Add(LT_ABUSE_TYPE.MEMORY, "CIntEx CRC Error");
            }
            return BitConverter.ToInt32(temp, 0);
        }

        protected override void set(int value)
        {
            byte[] src = BitConverter.GetBytes(value);
            byte[] xor = BitConverter.GetBytes(UnityEngine.Random.Range(-10000, 10000));
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (0xff & (src[i] ^ xor[i])) | getSeedValue();
                data2[i] = xor[i] | getSeedValue();
            }

            crc = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                crc += (i + 1) * data1[i];
                crc += (i + 2) * data2[i];
            }

            isValid = true;
        }

#if UNITY_EDITOR
        public int ReadFrom(UnityEditor.SerializedProperty property)
        {
            UnityEditor.SerializedProperty propData1 = property.FindPropertyRelative("data1");
            UnityEditor.SerializedProperty propData2 = property.FindPropertyRelative("data2");
            for (int i = 0; i < propData1.arraySize; i++)
            {
                data1[i] = propData1.GetArrayElementAtIndex(i).intValue;
            }
            for (int i = 0; i < propData2.arraySize; i++)
            {
                data2[i] = propData2.GetArrayElementAtIndex(i).intValue;
            }
            return Value;
        }

        public void WriteTo(UnityEditor.SerializedProperty property)
        {
            UnityEditor.SerializedProperty propData1 = property.FindPropertyRelative("data1");
            UnityEditor.SerializedProperty propData2 = property.FindPropertyRelative("data2");
            for (int i = 0; i < propData1.arraySize; i++)
            {
                propData1.GetArrayElementAtIndex(i).intValue = data1[i];
            }
            for (int i = 0; i < propData2.arraySize; i++)
            {
                propData2.GetArrayElementAtIndex(i).intValue = data2[i];
            }
        }
#endif
    }
}

