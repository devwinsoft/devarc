using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.AddressableAssets;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(EFFECT_ID))]
    public class EFFECT_ID_Drawer : EditorID_Drawer<BaseEffect>
    {
        protected override EditorID_Selector<BaseEffect> getSelector()
        {
            return EFFECT_ID_Selector.Instance;
        }
    }

    public class EFFECT_ID_Selector : EditorID_Selector<BaseEffect>
    {
        public new static EditorID_Selector<BaseEffect> Instance
        {
            get
            {
                if (msInstance != null)
                {
                    return msInstance;
                }

                msInstance = ScriptableWizard.DisplayWizard<EFFECT_ID_Selector>("Select EFFECT_ID");
                return msInstance;
            }
        }

        public override void Reload()
        {
            foreach (var obj in AssetManager.LoadPrefabs_Database<BaseEffect>("*", DEV_Settings.GetDefault_BundlePath()))
            {
                add(obj.name);
            }
        }
    }
}

