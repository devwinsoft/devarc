using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(UNIT_MONSTER_ID))]
	public class UNIT_MONSTER_ID_Drawer : EditorID_Drawer<UNIT_MONSTER>
	{
		protected override EditorID_Selector<UNIT_MONSTER> getSelector()
		{
			return UNIT_MONSTER_ID_Selector.Instance;
		}
	}

	public class UNIT_MONSTER_ID_Selector : EditorID_Selector<UNIT_MONSTER>
	{
		public new static EditorID_Selector<UNIT_MONSTER> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<UNIT_MONSTER_ID_Selector>("Select UNIT_MONSTER_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.UNIT_MONSTER.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_MONSTER", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.UNIT_MONSTER.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("UNIT_MONSTER", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.UNIT_MONSTER.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.UNIT_MONSTER.List) add($"{obj.unit_id}:{obj.name_id}");
		}
	}
}
