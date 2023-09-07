using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public partial class _SKILL
    {
        public override Account GetClass<Account>(string value)
        {
            Account obj = new Account();
            string[] list = value.Split(',');
            return obj;
        }
    }
}

