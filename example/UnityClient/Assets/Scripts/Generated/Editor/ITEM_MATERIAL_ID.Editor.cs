using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(ITEM_MATERIAL_ID))]
	public class ITEM_MATERIAL_ID_Drawer : EditorID_Drawer<ITEM_MATERIAL>
	{
		protected override EditorID_Selector<ITEM_MATERIAL> getSelector()
		{
			return ITEM_MATERIAL_ID_Selector.Instance;
		}
	}

	public class ITEM_MATERIAL_ID_Selector : EditorID_Selector<ITEM_MATERIAL>
	{
		public new static EditorID_Selector<ITEM_MATERIAL> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<ITEM_MATERIAL_ID_Selector>("Select ITEM_MATERIAL_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.ITEM_MATERIAL.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_MATERIAL", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.ITEM_MATERIAL.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_MATERIAL", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.ITEM_MATERIAL.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.ITEM_MATERIAL.List) add(obj.item_id);
		}
	}
}
