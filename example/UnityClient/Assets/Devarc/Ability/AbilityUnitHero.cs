using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityUnitHero : BaseAbilityUnit
    {
        public override UNIT tableData => mTable;
        UNIT_HERO mTable = null;

        public override BaseAbilityUnit clone()
        {
            var obj = new AbilityUnitHero();
            obj.mUID = mUID;
            obj.mParentUID = mParentUID;
            obj.mTable = mTable;
            obj.mLevel = mLevel;
            return obj;
        }

        public void Init(ulong uid, UNIT_HERO table, int level)
        {
            mUID = uid;
            mTable = table;
            mLevel = level;
        }
    }
}
