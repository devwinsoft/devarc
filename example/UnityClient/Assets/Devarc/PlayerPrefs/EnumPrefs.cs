using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class EnumPrefs<T> where T : struct, System.Enum
    {
        string name;
        string initValue;

        public EnumPrefs(string key, T defaultValue)
        {
            name = key;
            initValue = defaultValue.ToString();
        }

        public static implicit operator T(EnumPrefs<T> obj)
        {
            if (obj == null) return default(T);
            T value = default(T);
            if (PlayerPrefs.HasKey(obj.name))
            {
                Enum.TryParse<T>(PlayerPrefs.GetString(obj.name, obj.initValue), out value);
            }
            return value;
        }

        public static implicit operator string(EnumPrefs<T> obj)
        {
            if (obj == null) return string.Empty;
            return PlayerPrefs.GetString(obj.name, obj.initValue);
        }


        public T Value
        {
            get => get();
            set => set(value);
        }

        T get()
        {
            T value = default(T);
            if (PlayerPrefs.HasKey(name))
            {
                Enum.TryParse<T>(PlayerPrefs.GetString(name, initValue), out value);
            }
            return value;
        }

        void set(T value)
        {
            PlayerPrefs.SetString(name, value.ToString());
        }
    }
}