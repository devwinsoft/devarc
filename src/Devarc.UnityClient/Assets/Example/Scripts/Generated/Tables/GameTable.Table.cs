namespace Devarc
{
	public partial class Table
	{
		public static TableData<CHARACTER, _CHARACTER, int> CHARACTER = new TableData<CHARACTER, _CHARACTER, int>();
		public static TableData<SKILL, _SKILL, string> SKILL = new TableData<SKILL, _SKILL, string>();
	}

	[System.Serializable]
	public class CHARACTER_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(CHARACTER_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator CHARACTER_ID(string value)
		{
			CHARACTER_ID obj = new CHARACTER_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class SKILL_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SKILL_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SKILL_ID(string value)
		{
			SKILL_ID obj = new SKILL_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class GameTable_Extension
	{
		public static bool IsValid(this CHARACTER_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this SKILL_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
