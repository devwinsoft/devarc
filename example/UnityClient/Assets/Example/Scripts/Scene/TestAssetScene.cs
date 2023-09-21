using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Devarc;


public class TestAssetScene : BaseScene
{
    public UIDebugLog debugLog;
    public CHARACTER_ID charID;
    public SKILL_ID skillID;
    public SOUND_ID soundID;
    public EffectPlayData effectData;
    public STRING_ID stringID;


    protected override void onAwake()
    {
        if (!AppManager.IsCreated())
        {
            AppManager.Create("AppManager");
        }
        Debug.Log("TestAssetScene::onAwake");
    }


    public override IEnumerator OnEnterScene()
    {
        AppManager.Instance.LoadResources(SystemLanguage.Korean);
        yield return AppManager.Instance.LoadLocalBundles(SystemLanguage.Korean);
    }


    public override void OnLeaveScene()
    {
        AppManager.Instance.UnloadResources();
        AppManager.Instance.UnloadBundles();
    }


    public void OnClick_Download()
    {
        StartCoroutine(download());
    }

    IEnumerator download()
    {
        long totalSize = 0;
        Dictionary<string, long> patchList = null;
        yield return DownloadManager.Instance.GetPatchList((_size, _list) =>
        {
            totalSize = _size;
            patchList = _list;
        });

        Debug.LogFormat("Start to download contents: {0:N0} kb", (float)totalSize / 1000f);
        bool success = false;
        yield return DownloadManager.Instance.Download(patchList, (result, process) =>
        {
            success = (result == AsyncOperationStatus.Succeeded);
        });

        Debug.Log($"Download completed: success={success}");
        if (success)
        {
            EffectManager.Instance.Clear();
            yield return EffectManager.Instance.LoadBundle("effect");
            yield return SoundManager.Instance.LoadBundle("sound");
            //yield return SoundManager.Instance.LoadBundleSounds("voice", lang);
        }
    }


    public void OnClick_PlayEffect()
    {
        EffectManager.Instance.CreateEffect(effectData, Vector3.zero);
    }


    public void OnClick_PlaySound()
    {
        SoundManager.Instance.PlaySound(CHANNEL.EFFECT, soundID);
    }


    public void OnClick_GotoNetworkTest()
    {
        SceneTransManager.Instance.LoadScene("TestNetworkScene");
    }
}
