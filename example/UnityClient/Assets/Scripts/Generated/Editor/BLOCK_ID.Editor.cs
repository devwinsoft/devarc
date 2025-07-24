using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(BLOCK_ID))]
	public class BLOCK_ID_Drawer : EditorID_Drawer<BLOCK>
	{
		protected override EditorID_Selector<BLOCK> getSelector()
		{
			return BLOCK_ID_Selector.Instance;
		}
	}

	public class BLOCK_ID_Selector : EditorID_Selector<BLOCK>
	{
		public new static EditorID_Selector<BLOCK> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<BLOCK_ID_Selector>("Select BLOCK_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.BLOCK.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("BLOCK", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.BLOCK.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("BLOCK", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.BLOCK.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.BLOCK.List) add(obj.block_id);
		}
	}
}
