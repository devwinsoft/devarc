using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class UILayout : PrefabSingleton<UILayout>
    {
        public abstract void Clear();
        public abstract void Init();

        public Canvas canvas => mCanvas;
        Canvas mCanvas;

        private void Awake()
        {
            mCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            if (UIManager.IsCreated())
            {
                switch (mCanvas.renderMode)
                {
                    case RenderMode.ScreenSpaceCamera:
                        mCanvas.worldCamera = UIManager.Instance.uiCamera;
                        break;
                    default:
                        break;
                }
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
