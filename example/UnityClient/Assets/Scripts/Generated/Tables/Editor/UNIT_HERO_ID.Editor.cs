using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(UNIT_HERO_ID))]
	public class UNIT_HERO_ID_Drawer : EditorID_Drawer<UNIT_HERO>
	{
		protected override EditorID_Selector<UNIT_HERO> getSelector()
		{
			return UNIT_HERO_ID_Selector.Instance;
		}
	}

	public class UNIT_HERO_ID_Selector : EditorID_Selector<UNIT_HERO>
	{
		public new static EditorID_Selector<UNIT_HERO> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<UNIT_HERO_ID_Selector>("Select UNIT_HERO_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.UNIT_HERO.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_HERO", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.UNIT_HERO.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_HERO", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.UNIT_HERO.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.UNIT_HERO.List) add($"{obj.unit_id}:{obj.name_id}");
		}
	}
}
