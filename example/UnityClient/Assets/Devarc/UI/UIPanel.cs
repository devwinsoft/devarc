using Devarc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public RectTransform rectTransform => mRectTransform;
    RectTransform mRectTransform;

    public virtual void Clear()
    {
    }

    protected virtual void onAwake()
    {
    }

    private void Awake()
    {
        mRectTransform = gameObject.SafeGetComponent<RectTransform>();
        onAwake();
    }
}
