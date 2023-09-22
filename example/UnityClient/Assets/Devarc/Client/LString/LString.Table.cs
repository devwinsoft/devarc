namespace Devarc
{
    public class _LString_TABLE : TableData<LString, _LString, string>
    {
        public _LString_TABLE()
        {
            TableManager.Instance.registerLoadStringCallback("LString", (textAsset) =>
            {
                LoadJson(textAsset.text);
            });
            TableManager.Instance.registerUnloadStringCallback("LString", () =>
            {
                Clear();
            });
        }
    }
    public partial class Table
	{
		public static _LString_TABLE LString = new _LString_TABLE();
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
