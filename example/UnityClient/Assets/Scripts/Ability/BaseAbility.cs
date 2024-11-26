using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseAbility
    {
        public ulong UID => mUID;
        protected ulong mUID = 0;

        Dictionary<STAT_TYPE, int> mStats = new Dictionary<STAT_TYPE, int>();

        public int this[STAT_TYPE type]
        {
            get
            {
                int value = 0;
                mStats.TryGetValue(type, out value);
                return value;
            }
        }

        public void AddStat(STAT_TYPE type, int add)
        {
            int value;
            mStats.TryGetValue(type, out value);
            mStats[type] = value + add;
        }

        public void AddStat(BaseAbility ability)
        {
            foreach (var temp in ability.mStats)
            {
                AddStat(temp.Key, temp.Value);
            }
        }
    }
}
