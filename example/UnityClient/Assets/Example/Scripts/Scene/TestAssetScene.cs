using System.Collections;
using UnityEngine;
using Devarc;


public class TestAssetScene : BaseScene
{
    public UIDebugLog debugLog;
    public CHARACTER_ID charID;
    public SKILL_ID skillID;
    public SOUND_ID soundID;
    public EffectPlayData effectData1;
    public EffectPlayData effectData2;
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
        yield return null;

        // Load resouce assets...
        TableManager.Instance.LoadResourceTable();
        TableManager.Instance.LoadResourceString(SystemLanguage.Korean);
        SoundManager.Instance.LoadResource();
    }


    public override void OnLeaveScene()
    {
        // Unload resource assets...
        TableManager.Instance.UnloadResourceTable();
        TableManager.Instance.UnloadResourceString();
        SoundManager.Instance.UnloadResource();

        // Unload bundle assets...
        AppManager.Instance.UnloadBundles();
    }


    public void OnClick_Download()
    {
        DownloadManager.Instance.BeginPatch();
    }

    public void OnClick_PlayEffect1()
    {
        EffectManager.Instance.CreateEffect(effectData1.EffectID, transform, effectData1.offset, effectData1.rotation, EFFECT_ATTACH_TYPE.World);
    }

    public void OnClick_PlayEffect2()
    {
        EffectManager.Instance.CreateEffect(effectData2.EffectID, transform, effectData2.offset, effectData2.rotation, EFFECT_ATTACH_TYPE.World);
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
