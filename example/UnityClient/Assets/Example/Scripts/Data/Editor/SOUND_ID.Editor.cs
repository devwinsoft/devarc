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
            Table.SOUND_BUNDLE.Clear();
            foreach (var textAsset in AssetManager.FindAssets<TextAsset>("SOUND", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
            {
                Table.SOUND_BUNDLE.LoadJson(textAsset.text);
            }
            foreach (var obj in Table.SOUND_BUNDLE.List)
            {
                add(obj.sound_id);
            }

            Table.SOUND_RESOURCE.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("SOUND", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.SOUND_RESOURCE.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.SOUND_RESOURCE.List)
			{
                add(obj.sound_id);
            }
        }
	}
}
