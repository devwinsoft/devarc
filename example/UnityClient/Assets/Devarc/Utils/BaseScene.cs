using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseScene : MonoBehaviour
{
    public bool isDone => mDone;
    bool mDone = false;

    public abstract IEnumerator OnEnterScene();
    public abstract void OnLeaveScene();

    protected virtual void onAwake() { }
    protected virtual void onDestroy() { }

    private void Awake()
    {
        mDone = false;
        onAwake();
    }

    private IEnumerator Start()
    {
        mDone = false;
        if (SceneTransManager.Instance.Contains(this) == false)
        {
            SceneTransManager.Instance.Register(this);
            yield return OnEnterScene();
        }
        mDone = true;
    }

    private void OnDestroy()
    {
        onDestroy();
    }

}

