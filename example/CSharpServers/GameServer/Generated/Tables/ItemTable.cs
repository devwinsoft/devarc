using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_ITEM_EQUIP : RawTableData
	{
		public string               item_id;
		public string               name_id;

		public virtual string       get_item_id() => (item_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _ITEM_EQUIP : RawTableData_ITEM_EQUIP
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class ITEM_EQUIP : ITableData<_ITEM_EQUIP, string>
	{
		public string GetKey() { return item_id; }
		[Key(0)]
		public string               item_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_ITEM_EQUIP data)
		{
			item_id = data.get_item_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_ITEM_RELIC : RawTableData
	{
		public string               item_id;
		public string               name_id;

		public virtual string       get_item_id() => (item_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _ITEM_RELIC : RawTableData_ITEM_RELIC
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class ITEM_RELIC : ITableData<_ITEM_RELIC, string>
	{
		public string GetKey() { return item_id; }
		[Key(0)]
		public string               item_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_ITEM_RELIC data)
		{
			item_id = data.get_item_id();
			name_id = data.get_name_id();
		}
	}

	[System.Serializable]
	public partial class RawTableData_ITEM_MATERIAL : RawTableData
	{
		public string               item_id;
		public string               name_id;

		public virtual string       get_item_id() => (item_id);
		public virtual string       get_name_id() => (name_id);
	}

	[System.Serializable]
	public partial class _ITEM_MATERIAL : RawTableData_ITEM_MATERIAL
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class ITEM_MATERIAL : ITableData<_ITEM_MATERIAL, string>
	{
		public string GetKey() { return item_id; }
		[Key(0)]
		public string               item_id;
		[Key(1)]
		public string               name_id;

		public void Initialize(_ITEM_MATERIAL data)
		{
			item_id = data.get_item_id();
			name_id = data.get_name_id();
		}
	}

}
