using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(CHARACTER_ID))]
	public class CHARACTER_ID_Drawer : EditorID_Drawer<CHARACTER>
	{
		protected override EditorID_Selector<CHARACTER> getSelector()
		{
			return CHARACTER_ID_Selector.Instance;
		}
	}

	public class CHARACTER_ID_Selector : EditorID_Selector<CHARACTER>
	{
		public new static EditorID_Selector<CHARACTER> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<CHARACTER_ID_Selector>("Select CHARACTER_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.CHARACTER.Clear();
			foreach (var textAsset in AssetManager.LoadDatabase_Assets<TextAsset>("CHARACTER", DEV_Settings.GetTable_BundlePath()))
			{
				Table.CHARACTER.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.LoadDatabase_Assets<TextAsset>("CHARACTER", DEV_Settings.GetTable_BuiltinPath()))
			{
				Table.CHARACTER.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.CHARACTER.List) add($"{obj.character_id}:{obj.charName}");
		}
	}
}
