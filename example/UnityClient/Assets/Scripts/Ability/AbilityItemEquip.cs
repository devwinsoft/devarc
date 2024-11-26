using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityItemEquip : BaseAbilityItem
    {
        ITEM_EQUIP mTable = null;

        public void Init(ulong uid, ITEM_EQUIP table)
        {
            mUID = uid;
            mTable = table;
        }
    }
}
