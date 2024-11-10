using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityBattleAffect : BaseAbilityBattle
    {
        public AFFECT affectTable => mTable;
        AFFECT mTable = null;

        public void Init(ulong uid, AFFECT table)
        {
            mUID = uid;
            mTable = table;
        }
    }
}