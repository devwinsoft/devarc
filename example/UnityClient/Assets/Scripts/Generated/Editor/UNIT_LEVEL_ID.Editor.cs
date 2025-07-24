using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(UNIT_LEVEL_ID))]
	public class UNIT_LEVEL_ID_Drawer : EditorID_Drawer<UNIT_LEVEL>
	{
		protected override EditorID_Selector<UNIT_LEVEL> getSelector()
		{
			return UNIT_LEVEL_ID_Selector.Instance;
		}
	}

	public class UNIT_LEVEL_ID_Selector : EditorID_Selector<UNIT_LEVEL>
	{
		public new static EditorID_Selector<UNIT_LEVEL> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<UNIT_LEVEL_ID_Selector>("Select UNIT_LEVEL_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.UNIT_LEVEL.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_LEVEL", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.UNIT_LEVEL.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_LEVEL", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.UNIT_LEVEL.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.UNIT_LEVEL.List) add(obj.index);
		}
	}
}
