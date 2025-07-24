using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(ITEM_RELIC_ID))]
	public class ITEM_RELIC_ID_Drawer : EditorID_Drawer<ITEM_RELIC>
	{
		protected override EditorID_Selector<ITEM_RELIC> getSelector()
		{
			return ITEM_RELIC_ID_Selector.Instance;
		}
	}

	public class ITEM_RELIC_ID_Selector : EditorID_Selector<ITEM_RELIC>
	{
		public new static EditorID_Selector<ITEM_RELIC> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<ITEM_RELIC_ID_Selector>("Select ITEM_RELIC_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.ITEM_RELIC.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_RELIC", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.ITEM_RELIC.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("ITEM_RELIC", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.ITEM_RELIC.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.ITEM_RELIC.List) add(obj.item_id);
		}
	}
}
