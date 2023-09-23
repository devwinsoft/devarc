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
        // Load resouce assets...
        TableManager.Instance.LoadResourceTable();
        TableManager.Instance.LoadResourceString(SystemLanguage.Korean);
        SoundManager.Instance.LoadResource();

        // Load bundle assets...
#if UNITY_EDITOR
        yield return TableManager.Instance.LoadBundleTable("table-json", TableFormatType.JSON);
        yield return TableManager.Instance.LoadBundleString("lstring-json", TableFormatType.JSON, SystemLanguage.Korean);
#else
        yield return TableManager.Instance.LoadBundleTable("table-bin", TableFormatType.BIN);
        yield return TableManager.Instance.LoadBundleString("lstring-bin", TableFormatType.JSON, SystemLanguage.Korean);
#endif
    }


    public override void OnLeaveScene()
    {
        // Unload resource assets...
        TableManager.Instance.UnloadResourceTable();
        TableManager.Instance.UnloadResourceString();
        SoundManager.Instance.UnloadResource();

        // Unload bundle assets...
#if UNITY_EDITOR
        TableManager.Instance.UnloadBundleTable("table-json");
        TableManager.Instance.UnloadBundleString("lstring-json");
#else
        TableManager.Instance.UnloadBundleTable("table-bin");
        TableManager.Instance.UnloadBundleString("lstring-bin");
#endif
        EffectManager.Instance.UnloadBundle("effect");
        SoundManager.Instance.UnloadBundle("sound");
    }


    public void OnClick_Download()
    {
        DownloadManager.Instance.BeginPatch();
    }

    public void OnClick_PlayEffect1()
    {
        EffectManager.Instance.CreateEffect(effectData1, Vector3.zero);
    }

    public void OnClick_PlayEffect2()
    {
        EffectManager.Instance.CreateEffect(effectData2, Vector3.zero);
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
