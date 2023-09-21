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
        DownloadManager.Instance.BeginPatch();
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
