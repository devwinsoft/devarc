using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(SOUND_RESOURCE_ID))]
	public class SOUND_RESOURCE_ID_Drawer : EditorID_Drawer<SOUND_RESOURCE>
	{
		protected override EditorID_Selector<SOUND_RESOURCE> getSelector()
		{
			return SOUND_RESOURCE_ID_Selector.Instance;
		}
	}

	public class SOUND_RESOURCE_ID_Selector : EditorID_Selector<SOUND_RESOURCE>
	{
		public new static EditorID_Selector<SOUND_RESOURCE> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<SOUND_RESOURCE_ID_Selector>("Select SOUND_RESOURCE_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.SOUND_RESOURCE.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("SOUND_RESOURCE", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.SOUND_RESOURCE.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("SOUND_RESOURCE", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.SOUND_RESOURCE.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.SOUND_RESOURCE.List) add(obj.sound_id);
		}
	}
}
