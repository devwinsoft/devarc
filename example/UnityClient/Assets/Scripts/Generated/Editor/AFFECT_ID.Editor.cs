using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(AFFECT_ID))]
	public class AFFECT_ID_Drawer : EditorID_Drawer<AFFECT>
	{
		protected override EditorID_Selector<AFFECT> getSelector()
		{
			return AFFECT_ID_Selector.Instance;
		}
	}

	public class AFFECT_ID_Selector : EditorID_Selector<AFFECT>
	{
		public new static EditorID_Selector<AFFECT> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<AFFECT_ID_Selector>("Select AFFECT_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.AFFECT.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("AFFECT", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.AFFECT.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("AFFECT", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.AFFECT.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.AFFECT.List) add(obj.affect_id);
		}
	}
}
