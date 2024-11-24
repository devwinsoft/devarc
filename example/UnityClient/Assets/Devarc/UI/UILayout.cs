using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class UILayout : PrefabSingleton<UILayout>
    {
        public abstract void Clear();
        public abstract void Init();

        private void Start()
        {
            if (UIManager.IsCreated())
            {
                UIManager.Instance.RegisterLayout(this);
            }
        }

        private void OnDestroy()
        {
            if (UIManager.IsCreated())
            {
                UIManager.Instance.RemoveLayout(this);
            }
        }
    }
}
