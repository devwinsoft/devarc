namespace Devarc
{
	public partial class Table
	{
		public static TableData<SOUND_BUNDLE, _SOUND_BUNDLE, int> SOUND_BUNDLE = new TableData<SOUND_BUNDLE, _SOUND_BUNDLE, int>();
		public static TableData<SOUND_RESOURCE, _SOUND_RESOURCE, int> SOUND_RESOURCE = new TableData<SOUND_RESOURCE, _SOUND_RESOURCE, int>();
	}

	[System.Serializable]
	public class SOUND_BUNDLE_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SOUND_BUNDLE_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SOUND_BUNDLE_ID(string value)
		{
			SOUND_BUNDLE_ID obj = new SOUND_BUNDLE_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class SOUND_RESOURCE_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SOUND_RESOURCE_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SOUND_RESOURCE_ID(string value)
		{
			SOUND_RESOURCE_ID obj = new SOUND_RESOURCE_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class SoundTable_Extension
	{
		public static bool IsValid(this SOUND_BUNDLE_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this SOUND_RESOURCE_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
