using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


public class DataManager : MonoSingleton<DataManager>
{
    public void LoadResources()
    {
        // Load tables.
        AssetManager.Instance.LoadAssets_Resource<TextAsset>("Tables");

        // Load sounds.
        SoundManager.Instance.LoadResourceSounds("SOUND@builtin");
    }


    public IEnumerator LoadTables()
    {
        yield return AssetManager.Instance.LoadAssets_Bundle<TextAsset>("table");
        Table.CHARACTER.LoadJson(loadTextAsset("CHARACTER"));
        Table.SKILL.LoadJson(loadTextAsset("SKILL"));
    }


    string loadTextAsset(string fileName)
    {
        var textAsset = AssetManager.Instance.GetAsset<TextAsset>(fileName);
        if (textAsset == null)
        {
            Debug.LogError($"[Table::LoadFile] Cannot find TextAsset: {fileName}");
            return string.Empty;
        }
        return textAsset.text;
    }
}
