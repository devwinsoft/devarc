using System;
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    public interface UNIT
    {
    }

    public partial class UNIT_HERO : UNIT
    {
    }

    public partial class UNIT_MONSTER : UNIT
    {
    }

    public abstract class BaseAbilityUnit : BaseAbility
    {
        public ulong ParentUID => mParentUID;
        protected ulong mParentUID = 0;

        public int Level => mLevel;
        protected int mLevel = 0;

        public abstract UNIT tableData { get; }
        public abstract BaseAbilityUnit Clone();
    }
}
