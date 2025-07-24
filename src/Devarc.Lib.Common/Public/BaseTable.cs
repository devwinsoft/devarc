#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;
#else
using Newtonsoft.Json;
#endif
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;

namespace Devarc
{
    public class RawTableData
    {
        public bool GetBool(string value)
        {
            bool result;
            bool.TryParse(value, out result);
            return result;
        }

        public bool[] GetBoolArray(string value)
        {
            var list = value.Split(',');
            var result = new bool[list.Length];
            for (var i = 0; i < list.Length; i++) bool.TryParse(list[i].Trim(), out result[i]);
            return result;
        }

        public short GetShort(string value)
        {
            short result;
            short.TryParse(value, out result);
            return result;
        }

        public short[] GetShortArray(string value)
        {
            var list = value.Split(',');
            var result = new short[list.Length];
            for (var i = 0; i < list.Length; i++) short.TryParse(list[i].Trim(), out result[i]);
            return result;
        }

        public int GetInt(string value)
        {
            int result;
            int.TryParse(value, out result);
            return result;
        }

        public int[] GetIntArray(string value)
        {
            var list = value.Split(',');
            var result = new int[list.Length];
            for (var i = 0; i < list.Length; i++) int.TryParse(list[i].Trim(), out result[i]);
            return result;
        }

        public float GetFloat(string value)
        {
            float result;
            float.TryParse(value, out result);
            return result;
        }

        public float[] GetFloatArray(string value)
        {
            var list = value.Split(',');
            var result = new float[list.Length];
            for (var i = 0; i < list.Length; i++) float.TryParse(list[i].Trim(), out result[i]);
            return result;
        }

        public T GetEnum<T>(string value) where T : Enum
        {
            try
            {
                var result = (T)Enum.Parse(typeof(T), value);
                return result;
            }
            catch
            {
                return default;
            }
        }

        public T[] GetEnumArray<T>(string value) where T : Enum
        {
            var list = value.Split(',');
            var result = new T[list.Length];
            for (var i = 0; i < list.Length; i++) result[i] = GetEnum<T>(list[i].Trim());
            return result;
        }

        public string[] GetStringArray(string value)
        {
            var list = value.Split(',');
            var result = new string[list.Length];
            for (var i = 0; i < list.Length; i++) result[i] = list[i].Trim();
            return result;
        }

        public virtual T GetClass<T>(string value) where T : new()
        {
            return default;
        }

        public virtual T[] GetClassArray<T>(string value) where T : new()
        {
            return new T[0];
        }
    }


    public interface ITableData<TRaw, TKey>
    {
        TKey GetKey();
        void Initialize(TRaw raw);
    }

    [Serializable]
    public class TableContents<T>
    {
        public List<T> list;
    }

    public abstract class TableDataBase
    {
        private ReadOnlySequence<byte> mBuffer;
        private int mOffset;

        public void InitLoad(byte[] data)
        {
            mBuffer = new ReadOnlySequence<byte>(data);
            mOffset = 0;
        }

        public int ReadInt()
        {
            var data = mBuffer.Slice(mOffset, 4);
            mOffset += 4;
            return BitConverter.ToInt32(data.ToArray(), 0);
        }

        public byte[] ReadBytes(int size)
        {
            var data = mBuffer.Slice(mOffset, size);
            mOffset += size;
            return data.ToArray();
        }

#if UNITY_2019_1_OR_NEWER
        protected static string getTextFromTextAsset(string fileName)
        {
            var textAsset = AssetManager.Instance.GetAsset<TextAsset>(fileName);
            if (textAsset == null)
            {
                Debug.LogError($"[Table::LoadFile] Cannot find TextAsset: {fileName}");
                return string.Empty;
            }
            return textAsset.text;
        }
#endif
    }

    public class TableData<T, TRaw, TKey> : TableDataBase
        where T : ITableData<TRaw, TKey>, new()
        where TRaw : RawTableData
    {
        public IEnumerable<T> List => mList.Values;
        private readonly Dictionary<TKey, T> mList = new();

        public void Clear()
        {
            mList.Clear();
        }

        public bool Add(TKey key, T value)
        {
            if (mList.ContainsKey(key))
            {
                Debugging.LogError($"Duplicated table id: type={typeof(T).Name}, key={key}");
                return false;
            }

            mList.Add(key, value);
            return true;
        }

        public T Get(TKey key)
        {
            T obj;
            mList.TryGetValue(key, out obj);
            return obj;
        }

        public byte[] GetBytes()
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(List.Count()));
            foreach (var obj in List)
            {
                var temp = MessagePackSerializer.Serialize(obj);
                ms.Write(BitConverter.GetBytes(temp.Length));
                ms.Write(temp);
            }

            return ms.ToArray();
        }


#if UNITY_2019_1_OR_NEWER
        public void LoadJson(string json)
        {
            var contents = JsonUtility.FromJson<TableContents<RAW>>(json);
            foreach (var raw in contents.list)
            {
                T obj = new T();
                obj.Initialize(raw);
                Add(obj.GetKey(), obj);
            }
        }

        public void LoadFromFile(string fileName)
        {
            LoadJson(getTextFromTextAsset(fileName));
        }
#else
        public void LoadJson(string json)
        {
            var contents = JsonConvert.DeserializeObject<TableContents<TRaw>>(json);
            foreach (var raw in contents.list)
            {
                var obj = new T();
                obj.Initialize(raw);
                Add(obj.GetKey(), obj);
            }
        }
#endif
    }
}