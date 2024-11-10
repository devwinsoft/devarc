using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityBattleProjectile : BaseAbilityBattle
    {
        public PROJECTILE projectileTable => mTable;
        PROJECTILE mTable = null;

        public void Init(ulong uid, PROJECTILE table)
        {
            mUID = uid;
            mTable = table;
        }

    }
}