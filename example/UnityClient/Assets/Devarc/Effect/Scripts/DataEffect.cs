using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    [System.Serializable]
    public class EFFECT_ID
    {
        public static implicit operator string(EFFECT_ID obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.Value;
        }
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Value); }
        }
        public string Value;
    }


    [System.Serializable]
    public class EffectPlayData
    {
        public EFFECT_ID EffectID;
        public Vector2 Offset;
        public float WaitTime;
    }
}



