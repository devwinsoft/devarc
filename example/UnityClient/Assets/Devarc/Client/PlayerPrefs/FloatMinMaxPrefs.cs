using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    public class FloatMinMaxPrefs
    {
        string minName;
        string maxName;
        float minValue;
        float maxValue;

        public FloatMinMaxPrefs(string key, float minValue, float maxValue)
        {
            minName = key + ".min";
            maxName = key + ".max";
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public static implicit operator (float min, float max)(FloatMinMaxPrefs obj)
        {
            if (obj == null) return (0f, 0f);
            return obj.get();
        }

        public float MinValue
        {
            get => getMin();
            set => setMin(value);
        }

        public float MaxValue
        {
            get => getMax();
            set => setMax(value);
        }

        public (float min, float max) Values
        {
            get => get();
            set => set(value.min, value.max);
        }


        float getMin()
        {
            if (PlayerPrefs.HasKey(minName))
            {
                return PlayerPrefs.GetFloat(minName);
            }
            else
            {
                setMin(minValue);
                return minValue;
            }
        }

        void setMin(float value)
        {
            PlayerPrefs.SetFloat(minName, value);
        }


        float getMax()
        {
            if (PlayerPrefs.HasKey(maxName))
            {
                return PlayerPrefs.GetFloat(maxName);
            }
            else
            {
                setMax(maxValue);
                return maxValue;
            }
        }

        void setMax(float value)
        {
            PlayerPrefs.SetFloat(maxName, value);
        }


        (float, float) get()
        {
            if (PlayerPrefs.HasKey(minName))
            {
                float minValue = PlayerPrefs.GetFloat(minName);
                float maxValue = PlayerPrefs.GetFloat(maxName);
                return (minValue, maxValue);
            }
            else
            {
                set(minValue, maxValue);
                return (minValue, maxValue);
            }
        }

        void set(float min, float max)
        {
            PlayerPrefs.SetFloat(minName, min);
            PlayerPrefs.SetFloat(maxName, max);
        }
    }
}
