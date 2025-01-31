using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_UNIT_HERO : RawTableData
	{
		public string               unit_id;
		public string               name_id;

		public virtual string       get_unit_id() => (unit_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _UNIT_HERO : RawTableData_UNIT_HERO
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class UNIT_HERO : ITableData<_UNIT_HERO, string>
	{
		public string GetKey() { return unit_id; }
		[Key(0)]
		public string               unit_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_UNIT_HERO data)
		{
			unit_id = data.get_unit_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_UNIT_MONSTER : RawTableData
	{
		public string               unit_id;
		public string               name_id;

		public virtual string       get_unit_id() => (unit_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _UNIT_MONSTER : RawTableData_UNIT_MONSTER
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class UNIT_MONSTER : ITableData<_UNIT_MONSTER, string>
	{
		public string GetKey() { return unit_id; }
		[Key(0)]
		public string               unit_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_UNIT_MONSTER data)
		{
			unit_id = data.get_unit_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_UNIT_LEVEL : RawTableData
	{
		public string               index;
		public string               unit_id;
		public string               level;

		public virtual int          get_index() => GetInt(index);
		public virtual int          get_unit_id() => GetInt(unit_id);
		public virtual int          get_level() => GetInt(level);
	}

	[System.Serializable]
	public partial class _UNIT_LEVEL : RawTableData_UNIT_LEVEL
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class UNIT_LEVEL : ITableData<_UNIT_LEVEL, int>
	{
		public int GetKey() { return index; }
		[Key(0)]
		public int                  index;
		[Key(1)]
		public int                  unit_id;
		[Key(2)]
		public int                  level;

		public void Initialize(_UNIT_LEVEL data)
		{
			index = data.get_index();
			unit_id = data.get_unit_id();
			level = data.get_level();
		}
	}

}
