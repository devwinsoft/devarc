using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseScene : MonoBehaviour
{
    protected virtual void onAwake() { }
    protected virtual void onStart() { }

    private void Awake()
    {
        onAwake();
    }

    private void Start()
    {
        onStart();
    }
}

