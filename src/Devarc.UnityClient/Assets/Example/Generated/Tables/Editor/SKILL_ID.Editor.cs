using UnityEngine;
using UnityEditor;

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

		protected override void reload()
		{
			var textAsset = AssetManager.LoadAssetAtPath<TextAsset>("Example/Bundles/Tables/SKILL");
			if (textAsset == null) return;
			Table.SKILL.Clear();
			Table.SKILL.LoadJson(textAsset.text);
			foreach (var obj in Table.SKILL.List) add(obj.skill_id);
		}
	}
}
