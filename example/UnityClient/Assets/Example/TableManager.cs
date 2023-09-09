using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


public class TableManager : MonoSingleton<TableManager>
{
    public void LoadResources()
    {
        // Load tables.
        AssetManager.Instance.LoadResource_Assets<TextAsset>("Tables");

        // Load sounds.
        SoundManager.Instance.LoadResources("SOUND@builtin");
    }


    public IEnumerator LoadBundles()
    {
        // Load tables.
        yield return AssetManager.Instance.LoadBundle_Assets<TextAsset>("table");
        GameTable.CHARACTER.LoadFile("CHARACTER");
        GameTable.SKILL.LoadFile("SKILL");


        // Load effects.
        yield return AssetManager.Instance.LoadBundle_Prefabs("effect");


        // Load sounds.
        yield return SoundManager.Instance.LoadBundles("SOUND", "sound");
    }
}
