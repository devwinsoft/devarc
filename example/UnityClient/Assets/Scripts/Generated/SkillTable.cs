using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_SKILL : RawTableData
	{
		public string               skill_id;
		public string               name_id;

		public virtual string       get_skill_id() => (skill_id);
		public virtual string       get_name_id() => (name_id);
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
		public string               name_id;

		public void Initialize(_SKILL data)
		{
			skill_id = data.get_skill_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_PROJECTILE : RawTableData
	{
		public string               projectile_id;
		public string               name_id;

		public virtual string       get_projectile_id() => (projectile_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _PROJECTILE : RawTableData_PROJECTILE
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class PROJECTILE : ITableData<_PROJECTILE, string>
	{
		public string GetKey() { return projectile_id; }
		[Key(0)]
		public string               projectile_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_PROJECTILE data)
		{
			projectile_id = data.get_projectile_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_AFFECT : RawTableData
	{
		public string               affect_id;
		public string               name_id;

		public virtual string       get_affect_id() => (affect_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _AFFECT : RawTableData_AFFECT
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class AFFECT : ITableData<_AFFECT, string>
	{
		public string GetKey() { return affect_id; }
		[Key(0)]
		public string               affect_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_AFFECT data)
		{
			affect_id = data.get_affect_id();
			name_id = data.get_name_id();
		}
	}

}
