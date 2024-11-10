using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(PROJECTILE_ID))]
	public class PROJECTILE_ID_Drawer : EditorID_Drawer<PROJECTILE>
	{
		protected override EditorID_Selector<PROJECTILE> getSelector()
		{
			return PROJECTILE_ID_Selector.Instance;
		}
	}

	public class PROJECTILE_ID_Selector : EditorID_Selector<PROJECTILE>
	{
		public new static EditorID_Selector<PROJECTILE> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<PROJECTILE_ID_Selector>("Select PROJECTILE_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.PROJECTILE.Clear();
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("PROJECTILE", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))
			{
				Table.PROJECTILE.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.FindAssets<TextAsset>("PROJECTILE", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))
			{
				Table.PROJECTILE.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.PROJECTILE.List) add(obj.projectile_id);
		}
	}
}
