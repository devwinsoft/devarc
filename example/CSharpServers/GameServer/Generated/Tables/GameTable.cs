using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_CHARACTER : RawTableData
	{
		public string               character_id;
		public string               charName;
		public string               age;
		public string               gender;

		public virtual int          get_character_id() => GetInt(character_id);
		public virtual string       get_charName() => (charName);
		public virtual int          get_age() => GetInt(age);
		public virtual GenderType   get_gender() => GetEnum<GenderType>(gender);
	}

	[System.Serializable]
	public partial class _CHARACTER : RawTableData_CHARACTER
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class CHARACTER : ITableData<_CHARACTER, int>
	{
		public int GetKey() { return character_id; }
		[Key(0)]
		public int                  character_id;
		[Key(1)]
		public string               charName;
		[Key(2)]
		public int                  age;
		[Key(3)]
		public GenderType           gender;

		public void Initialize(_CHARACTER data)
		{
			character_id = data.get_character_id();
			charName = data.get_charName();
			age = data.get_age();
			gender = data.get_gender();
		}
	}

	[System.Serializable]
	public partial class RawTableData_SKILL : RawTableData
	{
		public string               skill_id;
		public string               skillName;
		public string               level;
		public string               power;
		public string               account;
		public string               classTest;
		public string               args;

		public virtual string       get_skill_id() => (skill_id);
		public virtual string       get_skillName() => (skillName);
		public virtual int          get_level() => GetInt(level);
		public virtual int          get_power() => GetInt(power);
		public virtual Account      get_account() => GetClass<Account>(account);
		public virtual Account[]    get_classTest() => GetClassArray<Account>(classTest);
		public virtual int[]        get_args() => GetIntArray(args);
	}

	[System.Serializable]
	public partial class _SKILL : RawTableData_SKILL
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class SKILL : ITableData<_SKILL, string>
	{
		public string GetKey() { return skill_id; }
		[Key(0)]
		public string               skill_id;
		[Key(1)]
		public string               skillName;
		[Key(2)]
		public int                  level;
		[Key(3)]
		public int                  power;
		[Key(4)]
		public Account              account;
		[Key(5)]
		public Account[]            classTest;
		[Key(6)]
		public int[]                args;

		public void Initialize(_SKILL data)
		{
			skill_id = data.get_skill_id();
			skillName = data.get_skillName();
			level = data.get_level();
			power = data.get_power();
			account = data.get_account();
			classTest = data.get_classTest();
			args = data.get_args();
		}
	}

}
