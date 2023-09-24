using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Devarc
{
    public class BuildScript
    {
        [MenuItem("Tools/Devarc/Build/BuildTables")]
        public static void BuildTables()
        {
            Table.Initailize();
            TableManager.SaveTable();
            EditorUtility.DisplayDialog("Build binary tables", "Build completed.", "OK");
        }
    }
}