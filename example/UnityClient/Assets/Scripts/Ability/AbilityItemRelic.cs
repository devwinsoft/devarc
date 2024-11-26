using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityItemRelic : BaseAbilityItem
    {
        ITEM_RELIC mTable = null;

        public void Init(ulong uid, ITEM_RELIC table)
        {
            mUID = uid;
            mTable = table;
        }
    }
}
