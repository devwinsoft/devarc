using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.AddressableAssets;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(EFFECT_ID))]
    public class EFFECT_ID_Drawer : EditorID_Drawer<BaseEffectPlay>
    {
        protected override EditorID_Selector<BaseEffectPlay> getSelector()
        {
            return EFFECT_ID_Selector.Instance;
        }
    }

    public class EFFECT_ID_Selector : EditorID_Selector<BaseEffectPlay>
    {
        public new static EditorID_Selector<BaseEffectPlay> Instance
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
            foreach (var obj in AssetManager.FindPrefabs<BaseEffectPlay>("*", DEV_Settings.GetDefault_BundlePath()))
            {
                add(obj.name);
            }
        }
    }
}

