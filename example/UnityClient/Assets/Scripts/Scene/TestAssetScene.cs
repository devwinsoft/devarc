using System.Collections;
using UnityEngine;
using Devarc;
using System.Net.Http.Headers;

public class TestAssetScene : BaseScene
{
    public UIDebugLog debugLog;
    public SKILL_ID skillID;
    public SOUND_ID soundID;
    public EffectDataPlay effectData1;
    public EffectDataPlay effectData2;

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
        DownloadManager.Instance.UnloadBundles();
    }


    public void OnClick_Download()
    {
        DownloadManager.Instance.Patch((info) =>
        {
            Debug.LogFormat("Start to download contents: {0:N0} kb", (float)(info.totalSize / 1000f));
            DownloadManager.Instance.Download(
                (progress) =>
                {
                },
                () =>
                {
                    Debug.Log($"Download completed.");
                    StartCoroutine(DownloadManager.Instance.LoadBundles());
                });
        });
    }

    public void OnClick_PlayEffect1()
    {
        EffectManager.Instance.CreateEffect(effectData1.EffectID, transform, effectData1.offset, effectData1.euler, EFFECT_ATTACH_TYPE.World);
    }

    public void OnClick_PlayEffect2()
    {
        EffectManager.Instance.CreateEffect(effectData2.EffectID, transform, effectData2.offset, effectData2.euler, EFFECT_ATTACH_TYPE.World);
    }

    public void OnClick_PlaySound()
    {
        SoundManager.Instance.PlaySound(CHANNEL.EFFECT, soundID);
    }

    public void OnClick_Mute()
    {
        if (SoundManager.Instance.IsMuted(CHANNEL.EFFECT))
        {
            SoundManager.Instance.Mute(CHANNEL.EFFECT, false);
        }
        else
        {
            SoundManager.Instance.Mute(CHANNEL.EFFECT, true);
        }
    }

    public void OnClick_GotoNetworkTest()
    {
        SceneTransManager.Instance.LoadScene("TestNetworkScene");
    }
}
