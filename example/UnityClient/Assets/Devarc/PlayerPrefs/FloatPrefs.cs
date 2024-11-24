using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    public class FloatPrefs
    {
        string name;
        float initValue;

        public FloatPrefs(string key, float defaultValue)
        {
            name = key;
            initValue = defaultValue;
        }

        public static implicit operator float(FloatPrefs obj)
        {
            if (obj == null) return 0;
            return obj.get();
        }

        public float Value
        {
            get => get();
            set => set(value);
        }

        float get()
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetFloat(name, initValue);
            }
            else
            {
                set(initValue);
                return initValue;
            }
        }

        void set(float value)
        {
            PlayerPrefs.SetFloat(name, value);
        }
    }
}
