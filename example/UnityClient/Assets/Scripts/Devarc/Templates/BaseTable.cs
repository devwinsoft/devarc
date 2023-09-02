using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Devarc
{
    public interface ITableData<KEY>
    {
        KEY GetKey();
    }

    [System.Serializable]
    public class TableContents<T>
    {
        public List<T> list;
    }    

    public class TableManager<T, KEY> where T : ITableData<KEY>
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
                Debug.LogError($"Duplicated table id: type={typeof(T).Name}, key={key}");
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

        public void LoadJson(string json)
        {
            var contents = JsonConvert.DeserializeObject<TableContents<T>>(json);
            foreach (var obj in contents.list)
            {
                Add(obj.GetKey(), obj);
            }
        }
    }
}
