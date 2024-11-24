using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public partial class DownloadManager
    {
        public IEnumerator LoadBundles()
        {
#if UNITY_EDITOR
            yield return TableManager.Instance.LoadBundleTable("table-json", TableFormatType.JSON);
            yield return TableManager.Instance.LoadBundleString("lstring-json", TableFormatType.JSON, SystemLanguage.English);
#else
        yield return TableManager.Instance.LoadBundleTable("table-bin", TableFormatType.BIN);
        yield return TableManager.Instance.LoadBundleString("lstring-bin", TableFormatType.BIN, SystemLanguage.English);
#endif

            yield return EffectManager.Instance.LoadBundle("effect");
            yield return SoundManager.Instance.LoadBundle("sound");
            //yield return SoundManager.Instance.LoadBundleSounds("voice", SystemLanguage.English);

            foreach (var data in Table.CHARACTER.List)
            {
                Debug.Log(JsonUtility.ToJson(data));
            }

            foreach (var data in Table.SKILL.List)
            {
                Debug.Log(JsonUtility.ToJson(data));
            }

            foreach (var data in Table.SOUND_BUNDLE.List)
            {
                Debug.Log(JsonUtility.ToJson(data));
            }
        }


        public void UnloadBundles()
        {
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
    }
}
