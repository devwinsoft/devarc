using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public partial class VECTOR3
    {
        public override VECTOR3 Parse(string value)
        {
            var list = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            VECTOR3 result = new VECTOR3();
            result.x = list.GetFloat(0);
            result.y = list.GetFloat(1);
            result.z = list.GetFloat(2);
            return result;
        }
    }
}

