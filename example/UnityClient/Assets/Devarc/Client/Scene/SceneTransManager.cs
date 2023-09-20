using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;
using UnityEngine.SceneManagement;

public class SceneTransManager : MonoSingleton<SceneTransManager>
{
    List<BaseScene> mCurrentScenes = new List<BaseScene>();

    protected override void onAwake()
    {
    }

    public void Register(BaseScene scene)
    {
        mCurrentScenes.Add(scene);
    }


    public bool Contains(BaseScene scene)
    {
        return mCurrentScenes.Contains(scene);
    }


    public void LoadScene(string sceneName, float waitTime = 0f)
    {
        StartCoroutine(loadScene(sceneName, waitTime));
    }


    IEnumerator loadScene(string sceneName, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        foreach (BaseScene scene in mCurrentScenes)
        {
            scene.OnLeaveScene();
        }
        mCurrentScenes.Clear();

        var handle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (handle == null)
        {
            Debug.LogError($"[SceneTransManager::ChangeScene] Cannot find scene: name={sceneName}");
            yield break;
        }
        yield return handle;

        bool isDone = false;
        while (isDone == false)
        {
            foreach (BaseScene scene in mCurrentScenes)
            {
                isDone = scene.isDone;
            }
            yield return null;
        }
    }
}
