namespace Devarc
{
	public class _SOUND_BUNDLE_TABLE : TableData<SOUND_BUNDLE, _SOUND_BUNDLE, int>
	{
		public _SOUND_BUNDLE_TABLE()
		{
			TableManager.Instance.registerLoadTableCallback("SOUND_BUNDLE", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.Instance.registerUnloadTableCallback("SOUND_BUNDLE", () =>
			{
				Clear();
			});
		}
	}
	public class _SOUND_RESOURCE_TABLE : TableData<SOUND_RESOURCE, _SOUND_RESOURCE, int>
	{
		public _SOUND_RESOURCE_TABLE()
		{
			TableManager.Instance.registerLoadTableCallback("SOUND_RESOURCE", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.Instance.registerUnloadTableCallback("SOUND_RESOURCE", () =>
			{
				Clear();
			});
		}
	}
	public partial class Table
	{
		public static _SOUND_BUNDLE_TABLE SOUND_BUNDLE = new _SOUND_BUNDLE_TABLE();
		public static _SOUND_RESOURCE_TABLE SOUND_RESOURCE = new _SOUND_RESOURCE_TABLE();
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
