using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_CHARACTER : RawTableData
	{
		public string               character_id;
		public string               charName;
		public string               age;

		public virtual int          get_character_id() => GetInt(character_id);
		public virtual string       get_charName() => (charName);
		public virtual int          get_age() => GetInt(age);
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

		public void Initialize(_CHARACTER data)
		{
			character_id = data.get_character_id();
			charName = data.get_charName();
			age = data.get_age();
		}
	}

}
