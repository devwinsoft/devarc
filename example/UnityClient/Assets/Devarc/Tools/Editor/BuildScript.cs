using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Devarc
{
    public class BuildScript
    {
        [MenuItem("Tools/Devarc/Build/Build Binary Tables")]
        public static void BuildForTables()
        {
            Table.Initailize();
            TableManager.SaveTable();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Build binary tables", "Work completed: Build binary tables.", "OK");
        }

        [MenuItem("Tools/Devarc/Build/Build Android")]
        public static void BuildForAndroid()
        {
            PlayerSettings.Android.keyaliasPass = "1232456";
            PlayerSettings.Android.keyaliasPass = "123456";
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            var buildOption = new BuildPlayerOptions();
            buildOption.locationPathName = "";
            BuildPipeline.BuildPlayer(buildOption); ;
        }
    }
}