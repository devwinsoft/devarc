using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityBattleSkill : BaseAbilityBattle
    {
        public SKILL skillTable => mTable;
        SKILL mTable = null;

        public void Init(ulong uid, SKILL table)
        {
            mUID = uid;
            mTable = table;
        }
    }
}