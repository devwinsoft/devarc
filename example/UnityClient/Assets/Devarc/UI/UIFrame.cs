using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIFrame : MonoBehaviour
{
    public abstract void Clear();
    protected abstract void onInit();

    public void Init()
    {
        onInit();
    }
}
