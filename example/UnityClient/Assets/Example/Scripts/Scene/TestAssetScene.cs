using System.Collections;
using UnityEngine;
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
        // Load resouce assets...
        Table.Initailize();
        TableManager.Instance.LoadResourceTable();
        TableManager.Instance.LoadResourceString(SystemLanguage.Korean);
        SoundManager.Instance.LoadResource();

        // Load bundle assets...
        yield return TableManager.Instance.LoadBundleTable("table");
        yield return TableManager.Instance.LoadBundleString("lstring", SystemLanguage.Korean);
    }


    public override void OnLeaveScene()
    {
        // Unload resource assets...
        TableManager.Instance.UnloadResourceTable();
        TableManager.Instance.UnloadResourceString();
        SoundManager.Instance.UnloadResource();

        // Unload bundle assets...
        TableManager.Instance.UnloadBundleString("lstring");
        TableManager.Instance.UnloadBundleTable("table");
        EffectManager.Instance.UnloadBundle("effect");
        SoundManager.Instance.UnloadBundle("sound");
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
