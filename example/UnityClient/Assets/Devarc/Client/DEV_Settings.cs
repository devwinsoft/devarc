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
        [MenuItem("Tools/Devarc/Create setting file")]
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
            public CString resourcePath;
            public CString tableBinDirectory;
            public CString tableJsonDirectory;
            public CString stringBinDirectory;
            public CString stringJsonDirectory;
        }
        public RootPathData defaultDirectory = new RootPathData();


        public const string AuthorizationURI = "https://accounts.google.com/o/oauth2/v2/auth";
        public const string UserinfoURL = "https://www.googleapis.com/oauth2/v2/userinfo";
        public const string RevocationURI = "https://oauth2.googleapis.com/revoke";

        [Serializable]
        public class GoogleWebData
        {
            public CString client_id;
            public string login_uri;
            public string redirect_uri;
            public string code_uri;
            public string signin_uri;
            public string[] scopes;
        }
        public GoogleWebData googleWebData = new GoogleWebData();

        public static string GetDefault_BundlePath()
        {
            return Instance.defaultDirectory.bundlePath;
        }


        public static string GetTablePath(bool isBundle, TableFormatType formatType)
        {
            if (isBundle)
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.tableBinDirectory}";
                    default:
                        return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.tableJsonDirectory}";
                }
            }
            else
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        return $"{Instance.defaultDirectory.resourcePath}/{Instance.defaultDirectory.tableBinDirectory}";
                    default:
                        return $"{Instance.defaultDirectory.resourcePath}/{Instance.defaultDirectory.tableJsonDirectory}";
                }
            }
        }


        public static string GetStringPath(SystemLanguage lang, bool isBundle, TableFormatType formatType)
        {
            if (isBundle)
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.stringBinDirectory}/{lang}";
                    default:
                        return $"{Instance.defaultDirectory.bundlePath}/{Instance.defaultDirectory.stringJsonDirectory}/{lang}";
                }
            }
            else
            {
                switch (formatType)
                {
                    case TableFormatType.BIN:
                        return $"{Instance.defaultDirectory.resourcePath}/{Instance.defaultDirectory.stringBinDirectory}/{lang}";
                    default:
                        return $"{Instance.defaultDirectory.resourcePath}/{Instance.defaultDirectory.stringJsonDirectory}/{lang}";
                }
            }
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

