using UnityEngine;

namespace Devarc
{
    public class StringPrefs
    {
        string name;
        string initValue;

        public StringPrefs(string key, string defaultValue)
        {
            this.name = key;
            initValue = defaultValue;
        }

        public static implicit operator string(StringPrefs obj)
        {
            if (obj == null) return string.Empty;
            return obj.get();
        }

        public string Value
        {
            get => get();
            set => set(value);
        }

        string get()
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetString(name, initValue);
            }
            else
            {
                set(initValue);
                return initValue;
            }
        }

        void set(string value)
        {
            PlayerPrefs.SetString(name, value);
        }
    }
}
