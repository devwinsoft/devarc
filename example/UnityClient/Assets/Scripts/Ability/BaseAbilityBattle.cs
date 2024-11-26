using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class BaseAbilityBattle : BaseAbility
    {
        public ulong CasterUID => CasterAbility != null ? mCasterAbility.UID : 0;

        public BaseAbilityUnit CasterAbility => mCasterAbility;
        BaseAbilityUnit mCasterAbility = null;
    }
}
