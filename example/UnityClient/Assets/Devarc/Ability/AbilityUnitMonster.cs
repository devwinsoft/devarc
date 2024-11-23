using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AbilityUnitMonster : BaseAbilityUnit
    {
        public override UNIT tableData => mTable;
        UNIT_MONSTER mTable = null;

        public override BaseAbilityUnit Clone()
        {
            var obj = new AbilityUnitMonster();
            obj.mUID = mUID;
            obj.mParentUID = mParentUID;
            obj.mTable = mTable;
            obj.mLevel = mLevel;
            return obj;
        }

        public void Init(ulong uid, UNIT_MONSTER table, int level)
        {
            mUID = uid;
            mTable = table;
            mLevel = level;
        }
    }
}
