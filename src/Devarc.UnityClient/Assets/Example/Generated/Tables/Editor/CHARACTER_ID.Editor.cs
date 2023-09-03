using UnityEngine;
using UnityEditor;

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

		protected override void reload()
		{
			var textAsset = AssetManager.LoadAssetAtPath<TextAsset>("Example/Bundles/Tables/CHARACTER");
			if (textAsset == null) return;
			GameTable.CHARACTER.Clear();
			GameTable.CHARACTER.LoadJson(textAsset.text);
			foreach (var obj in GameTable.CHARACTER.List) add($"{obj.character_id}:{obj.charName}");
		}
	}
}
