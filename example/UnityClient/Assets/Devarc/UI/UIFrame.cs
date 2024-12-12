using Devarc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIFrame : MonoBehaviour
{
    public abstract void Clear();
    public abstract void Init(UICanvas canvas);
}

public abstract class UIFrame<T> : UIFrame where T : UICanvas
{
    protected abstract void onInit();

    protected T mCanvas;

    public sealed override void Init(UICanvas canvas)
    {
        mCanvas = canvas as T;
        onInit();
    }
}