using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    [System.Serializable]
    public class SOUND_ID
    {
        public string Value = string.Empty;
        public static implicit operator string(SOUND_ID obj)
        {
            if (obj == null) return string.Empty;
            return obj.Value;
        }
        public static implicit operator SOUND_ID(string value)
        {
            SOUND_ID obj = new SOUND_ID();
            obj.Value = value;
            return obj;
        }
    }

    public class SOUND
    {
    }
}

