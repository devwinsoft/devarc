namespace Devarc
{
	public partial class Table
	{
		public static TableData<LString, _LString, string> LString = new TableData<LString, _LString, string>();
	}

	[System.Serializable]
	public class STRING_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(STRING_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator STRING_ID(string value)
		{
			STRING_ID obj = new STRING_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class StringTable_Extension
	{
		public static bool IsValid(this STRING_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
