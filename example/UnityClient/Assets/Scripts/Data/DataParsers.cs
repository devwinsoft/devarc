using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public partial class Account
    {
        public override Account Parse(string value)
        {
            string[] list = value.Split(',');
            if (list.Length > 1)
            {
                Account account = new Account();
                int.TryParse(list[1], out account.level);
                account.nickName = list[2];
                return account;
            }
            return null;
        }
    }
}

