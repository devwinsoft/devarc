using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityItemMaterial : BaseAbilityItem
    {
        ITEM_MATERIAL mTable = null;

        public void Init(ulong uid, ITEM_MATERIAL table)
        {
            mUID = uid;
            mTable = table;
        }
    }
}
