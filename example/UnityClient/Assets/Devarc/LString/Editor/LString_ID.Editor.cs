using UnityEngine;
using UnityEditor;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(STRING_ID))]
	public class STRING_ID_Drawer : EditorID_Drawer<LString>
	{
		protected override EditorID_Selector<LString> getSelector()
		{
			return STRING_ID_Selector.Instance;
		}
	}

	public class STRING_ID_Selector : EditorID_Selector<LString>
	{
		public new static EditorID_Selector<LString> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<STRING_ID_Selector>("Select LString_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			var language = SystemLanguage.Korean;

            Table.LString.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetStringPath(language, true, TableFormatType.JSON)))
			{
				Table.LString.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("*", DEV_Settings.GetStringPath(language, false, TableFormatType.JSON)))
			{
				Table.LString.LoadJson(textAsset.text);
			}
            foreach (var obj in Table.LString.List)
            {
                string value = obj.value.Trim();
                add(obj.id, $"{obj.id} ({value.Substring(0, Mathf.Min(value.Length, 10))})");
            }
        }
	}
}
