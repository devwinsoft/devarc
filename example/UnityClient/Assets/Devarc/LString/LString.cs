using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_LString : RawTableData
	{
		public string               id;
		public string               value;

		public virtual string       get_id() => (id);
		public virtual string       get_value() => (value);
	}

	[System.Serializable]
	public partial class _LString : RawTableData_LString
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class LString : ITableData<_LString, string>
	{
		public string GetKey() { return id; }
		[Key(0)]
		public string               id;
		[Key(1)]
		public string               value;

		public void Initialize(_LString data)
		{
			id = data.get_id();
			value = data.get_value();
		}
	}

}
