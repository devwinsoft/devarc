using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(ITEM_EQUIP_ID))]
	public class ITEM_EQUIP_ID_Drawer : EditorID_Drawer<ITEM_EQUIP>
	{
		protected override EditorID_Selector<ITEM_EQUIP> getSelector()
		{
			return ITEM_EQUIP_ID_Selector.Instance;
		}
	}

	public class ITEM_EQUIP_ID_Selector : EditorID_Selector<ITEM_EQUIP>
	{
		public new static EditorID_Selector<ITEM_EQUIP> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<ITEM_EQUIP_ID_Selector>("Select ITEM_EQUIP_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.ITEM_EQUIP.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_EQUIP", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.ITEM_EQUIP.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_EQUIP", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.ITEM_EQUIP.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.ITEM_EQUIP.List) add(obj.item_id);
		}
	}
}
