using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(SOUND_BUNDLE_ID))]
	public class SOUND_BUNDLE_ID_Drawer : EditorID_Drawer<SOUND_BUNDLE>
	{
		protected override EditorID_Selector<SOUND_BUNDLE> getSelector()
		{
			return SOUND_BUNDLE_ID_Selector.Instance;
		}
	}

	public class SOUND_BUNDLE_ID_Selector : EditorID_Selector<SOUND_BUNDLE>
	{
		public new static EditorID_Selector<SOUND_BUNDLE> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<SOUND_BUNDLE_ID_Selector>("Select SOUND_BUNDLE_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.SOUND_BUNDLE.Clear();
			foreach (var textAsset in AssetManager.LoadDatabaseAssets<TextAsset>("SOUND_BUNDLE", DEV_Settings.GetTable_BundlePath()))
			{
				Table.SOUND_BUNDLE.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.LoadDatabaseAssets<TextAsset>("SOUND_BUNDLE", DEV_Settings.GetTable_BuiltinPath()))
			{
				Table.SOUND_BUNDLE.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.SOUND_BUNDLE.List) add(obj.sound_id);
		}
	}
}
