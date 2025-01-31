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

	[System.Serializable]
	public partial class RawTableData_BLOCK : RawTableData
	{
		public string               block_id;
		public string               block_type;
		public string               affect_list;
		public string               icon;

		public virtual string       get_block_id() => (block_id);
		public virtual string       get_block_type() => (block_type);
		public virtual string[]     get_affect_list() => GetStringArray(affect_list);
		public virtual string       get_icon() => (icon);
	}

	[System.Serializable]
	public partial class _BLOCK : RawTableData_BLOCK
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class BLOCK : ITableData<_BLOCK, string>
	{
		public string GetKey() { return block_id; }
		[Key(0)]
		public string               block_id;
		[Key(1)]
		public string               block_type;
		[Key(2)]
		public string[]             affect_list;
		[Key(3)]
		public string               icon;

		public void Initialize(_BLOCK data)
		{
			block_id = data.get_block_id();
			block_type = data.get_block_type();
			affect_list = data.get_affect_list();
			icon = data.get_icon();
		}
	}

}
