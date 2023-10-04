using UnityEngine;


namespace Devarc
{
    public class IntPrefs
    {
        string name;
        int initValue;

        public IntPrefs(string key, int defaultValue)
        {
            name = key;
            initValue = defaultValue;
        }

        public static implicit operator int(IntPrefs obj)
        {
            if (obj == null) return 0;
            return obj.get();
        }

        public int Value
        {
            get => get();
            set => set(value);
        }
        
        int get()
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetInt(name, initValue);
            }
            else
            {
                set(initValue);
                return initValue;
            }
        }

        void set(int value)
        {
            PlayerPrefs.SetInt(name, value);
        }
    }
}

