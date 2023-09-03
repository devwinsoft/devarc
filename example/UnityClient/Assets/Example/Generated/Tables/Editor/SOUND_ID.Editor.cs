using UnityEngine;
using UnityEditor;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(SOUND_ID))]
	public class SOUND_ID_Drawer : EditorID_Drawer<SOUND>
	{
		protected override EditorID_Selector<SOUND> getSelector()
		{
			return SOUND_ID_Selector.Instance;
		}
	}

	public class SOUND_ID_Selector : EditorID_Selector<SOUND>
	{
		public new static EditorID_Selector<SOUND> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<SOUND_ID_Selector>("Select SOUND_ID");
				return msInstance;
			}
		}

		protected override void reload()
		{
			var textAsset = AssetManager.LoadAssetAtPath<TextAsset>("Example/Bundles/Tables/SOUND");
			if (textAsset == null) return;
			Table.SOUND.Clear();
			Table.SOUND.LoadJson(textAsset.text);
			foreach (var obj in Table.SOUND.List) add(obj.sound_id);
		}
	}
}
