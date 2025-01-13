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
        public static implicit operator EFFECT_ID(string value)
        {
            EFFECT_ID obj = new EFFECT_ID();
            obj.Value = value;
            return obj;
        }
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Value); }
        }
        public string Value;
    }


    [System.Serializable]
    public class EffectDataPlay
    {
        public EFFECT_ID EffectID;
        public Vector3 offset;
        public Vector3 euler;
        public float WaitTime;
    }
}



