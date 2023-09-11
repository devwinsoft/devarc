using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Devarc
{
    public abstract partial class RawTableData
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
            bool[] result = new bool[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                bool.TryParse(list[i].Trim(), out result[i]);
            }
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
            int[] result = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                int.TryParse(list[i].Trim(), out result[i]);
            }
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
            float[] result = new float[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                float.TryParse(list[i].Trim(), out result[i]);
            }
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
                return default(T);
            }
        }

        public T[] GetEnumArray<T>(string value) where T : Enum
        {
            var list = value.Split(',');
            T[] result = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                result[i] = GetEnum<T>(list[i].Trim());
            }
            return result;
        }

        public virtual T GetClass<T>(string value) where T : new()
        {
            return default(T);
        }

        public virtual T[] GetClassArray<T>(string value) where T : new()
        {
            return new T[0];
        }
    }


    public interface ITableData<RAW, KEY>
    {
        KEY GetKey();
        void Initialize(RAW raw);
    }

    [System.Serializable]
    public class TableContents<T>
    {
        public List<T> list;
    }

    public abstract class TableDataBase
    {
    }

    public class TableData<T, RAW, KEY> : TableDataBase
        where T : ITableData<RAW, KEY>, new()
        where RAW : RawTableData
    {
        public IEnumerable<T> List => mList.Values;
        Dictionary<KEY, T> mList = new Dictionary<KEY, T>();

        public void Clear()
        {
            mList.Clear();
        }

        public bool Add(KEY key, T value)
        {
            if (mList.ContainsKey(key)) 
            {
                return false;
            }
            mList.Add(key, value);
            return true;
        }

        public T Get(KEY key)
        {
            T obj;
            mList.TryGetValue(key, out obj);
            return obj;
        }

        public void LoadJson(string json, System.Action<T> callback = null)
        {
            var contents = JsonConvert.DeserializeObject<TableContents<RAW>>(json);
            foreach (var raw in contents.list)
            {
                T obj = new T();
                obj.Initialize(raw);
                Add(obj.GetKey(), obj);
                callback?.Invoke(obj);
            }
        }
    }
}
