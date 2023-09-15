using UnityEngine;
using UnityEditor;
using System.Collections;

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

		public override void Reload()
		{
			Table.SOUND.Clear();
			foreach (var textAsset in AssetManager.LoadAssets_Database<TextAsset>("SOUND", DEV_Settings.GetTable_BundlePath()))
			{
				Table.SOUND.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.LoadAssets_Database<TextAsset>("SOUND", DEV_Settings.GetTable_BuiltinPath()))
			{
				Table.SOUND.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.SOUND.List) add(obj.sound_id);
		}
	}
}
