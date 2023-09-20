using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0649 // Disabled warnings related to serialized fields not assigned in this script but used in the editor.

namespace Devarc
{
    [System.Serializable]
    [ExcludeFromPresetAttribute]
    public class DEV_Settings : ScriptableObject
    {
        private static DEV_Settings sInstance;

        public static DEV_Settings Instance
        {
            get
            {
                if (DEV_Settings.sInstance == null)
                {
                    DEV_Settings.sInstance = Resources.Load<DEV_Settings>("DEV Settings");

#if UNITY_EDITOR
                    if (DEV_Settings.sInstance == null)
                    {
                        string dir = EditorUtility.OpenFolderPanel("Load png Textures", "", "");
                        string path = System.IO.Path.Combine(dir, "DEV Settings.asset");
                        if (path.Contains(Application.dataPath))
                        {
                            string subFoler = path.Substring(Application.dataPath.Length + 1);
                            string selectedFolder = System.IO.Path.Combine("Assets", subFoler);
                            DEV_Settings example = ScriptableObject.CreateInstance<DEV_Settings>();
                            AssetDatabase.CreateAsset(example, selectedFolder);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorUtility.FocusProjectWindow();
                            Selection.activeObject = example;
                        }
                        else
                        {
                        }
                    }
#endif
                }
                return DEV_Settings.sInstance;
            }
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Devwin/Create setting file")]
        static void ShowEditor()
        {
            var instance = DEV_Settings.Instance;
        }
#endif


        public SystemLanguage editorLanguage;


        [Serializable]
        public class RootPathData
        {
            public CString bundlePath;
            public CString builtinPath;
            public CString tableSubDirectory;
            public CString stringSubDirectory;
        }
        public RootPathData defaultDirectory = new RootPathData();

        public static string GetDefault_BundlePath()
        {
            return Instance.defaultDirectory.bundlePath;
        }


        public static string GetTable_BundlePath()
        {
            return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.tableSubDirectory}";
        }
        public static string GetTable_BuiltinPath()
        {
            return $"{Instance.defaultDirectory.builtinPath}/{Instance.defaultDirectory.tableSubDirectory}";
        }


        public static string GetStringTablePath_Bundle()
        {
            string subDir = Instance.editorLanguage.ToString();
            return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.stringSubDirectory}/{subDir}";
        }
        public static string GetStringTablePath_Builtin()
        {
            string subDir = Instance.editorLanguage.ToString();
            return $"{Instance.defaultDirectory.builtinPath}/{Instance.defaultDirectory.stringSubDirectory}/{subDir}";
        }

        private void OnDestroy()
        {
            if (sInstance == this)
            {
                sInstance = null;
            }
        }
    }
}

