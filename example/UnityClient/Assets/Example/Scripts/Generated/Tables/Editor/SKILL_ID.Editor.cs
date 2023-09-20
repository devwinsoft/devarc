using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Devarc
{
	[CustomPropertyDrawer(typeof(SKILL_ID))]
	public class SKILL_ID_Drawer : EditorID_Drawer<SKILL>
	{
		protected override EditorID_Selector<SKILL> getSelector()
		{
			return SKILL_ID_Selector.Instance;
		}
	}

	public class SKILL_ID_Selector : EditorID_Selector<SKILL>
	{
		public new static EditorID_Selector<SKILL> Instance
		{
			get {
				if (msInstance != null) return msInstance;
				msInstance = ScriptableWizard.DisplayWizard<SKILL_ID_Selector>("Select SKILL_ID");
				return msInstance;
			}
		}

		public override void Reload()
		{
			Table.SKILL.Clear();
			foreach (var textAsset in AssetManager.LoadDatabaseAssets<TextAsset>("SKILL", DEV_Settings.GetTable_BundlePath()))
			{
				Table.SKILL.LoadJson(textAsset.text);
			}
			foreach (var textAsset in AssetManager.LoadDatabaseAssets<TextAsset>("SKILL", DEV_Settings.GetTable_BuiltinPath()))
			{
				Table.SKILL.LoadJson(textAsset.text);
			}
			foreach (var obj in Table.SKILL.List) add(obj.skill_id);
		}
	}
}
