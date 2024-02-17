using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Devarc
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        class BundleData
        {
            public string addressable;
            public AsyncOperationHandle handle;
            public HashSet<string> list = new HashSet<string>();

            public void Add(string fileName)
            {
                string key = fileName.ToLower();
                list.Add(key);
            }

            public void Remove(string fileName)
            {
                string key = fileName.ToLower();
                list.Remove(key);
            }
        }
        Dictionary<string, BundleData> mBundles = new Dictionary<string, BundleData>();


        abstract class ResData
        {
            public string dir;
            public abstract UnityEngine.Object get();
        }

        class ResData<T> : ResData where T : UnityEngine.Object
        {
            public T obj;
            public override UnityEngine.Object get() => obj;
        }

        Dictionary<Type, Dictionary<string, UnityEngine.Object>> mBundleAssets = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();
        Dictionary<Type, Dictionary<string, ResData>> mResourceAssets = new Dictionary<Type, Dictionary<string, ResData>>();
        Dictionary<string, SceneInstance> mBundleScenes = new Dictionary<string, SceneInstance>();

#if UNITY_EDITOR
        public static GameObject[] FindPrefabs(string searchDir)
        {
            List<GameObject> result = new List<GameObject>();
            var list = AssetDatabase.FindAssets("t:GameObject", new string[] { searchDir });
            foreach (var guid in list)
            {
                var tempPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(tempPath);
                result.Add(obj);
            }
            return result.ToArray();
        }


        public static T[] FindPrefabs<T>(string fileName, string searchDir) where T : MonoBehaviour
        {
            List<T> result = new List<T>();
            var list = AssetDatabase.FindAssets($"t:GameObject {fileName}", new string[] { searchDir });
            foreach (var guid in list)
            {
                var tempPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(tempPath);
                var compo = obj.GetComponent<T>();
                if (compo != null)
                    result.Add(compo);
            }
            return result.ToArray();
        }


        public static T[] FindAssets<T>(string fileName, string searchDir) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            var list = AssetDatabase.FindAssets($"t:{typeof(T).Name} {fileName}", new string[] { searchDir });
            foreach (var guid in list)
            {
                var tempPath = AssetDatabase.GUIDToAssetPath(guid);
                var compo = AssetDatabase.LoadAssetAtPath<T>(tempPath);
                if (compo != null && compo.name == fileName)
                    result.Add(compo);
            }
            return result.ToArray();
        }
#endif


        public void UnloadResourceAsset<T>(string fileName) where T : UnityEngine.Object
        {
            string name = Path.GetFileName(fileName).ToLower();
            Type type = typeof(T);
            Dictionary<string, ResData> list = null;

            if (mResourceAssets.TryGetValue(type, out list) == false)
            {
                list = new Dictionary<string, ResData>();
                mResourceAssets.Add(type, list);
            }

            ResData data = null;
            if (list.TryGetValue(name, out data))
            {
                Resources.UnloadAsset(data.get());
                list.Remove(name);
            }
        }


        public IEnumerable<string> UnloadResourceAssets<T>(string searchDir)
        {
            Type type = typeof(T);
            string resDir = searchDir.ToLower();
            Dictionary<string, ResData> list = null;
            List<string> deleteNames = new List<string>();

            if (mResourceAssets.TryGetValue(type, out list) == false)
            {
                return deleteNames;
            }

            foreach (var data in list.Values)
            {
                if (data.dir.Contains(resDir))
                {
                    deleteNames.Add(data.get().name.ToLower());
                }
            }
            foreach (var name in deleteNames)
            {
                list.Remove(name);
            }
            return deleteNames;
        }


        public IEnumerable<string> UnloadBundleAssets(string key)
        {
            List<string> result = new List<string>();
            BundleData bundleData;
            if (mBundles.TryGetValue(key, out bundleData))
            {
                foreach (var name in bundleData.list)
                {
                    foreach (var list in mBundleAssets.Values)
                    {
                        result.Add(name);
                        list.Remove(name);
                    }
                }
                bundleData.list.Clear();
                Addressables.Release(bundleData.handle);
            }
            mBundles.Remove(key);
            return result;
        }


        public IEnumerator LoadBundleAsset<T>(string key) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetAsync<T>(key);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                var obj = task.Result;
                if (registerAsset_Bundle(obj))
                    getBundleData(key)?.Add(obj.name);
            }
            createBundleData(key, task);
        }

        public AsyncOperationHandle<IList<T>> LoadBundleAssets<T>(string key) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetsAsync<T>(key, (obj) =>
            {
                if (registerAsset_Bundle(obj))
                    getBundleData(key)?.Add(obj.name);
            });
            createBundleData(key, task);
            return task;
        }


        public AsyncOperationHandle<IList<T>> LoadBundleAssets<T>(string key, SystemLanguage lang) where T : UnityEngine.Object
        {
            List<string> keys = new List<string> { key, lang.ToString() };
            var task = Addressables.LoadAssetsAsync<T>(keys, (obj) =>
            {
                if (registerAsset_Bundle(obj))
                    getBundleData(key)?.Add(obj.name);
            }, Addressables.MergeMode.Intersection);
            createBundleData(key, task);
            return task;
        }

        public IEnumerator LoadBundleScene(string key, LoadSceneMode mode)
        {
            AsyncOperationHandle<SceneInstance> task = Addressables.LoadSceneAsync(key, mode, true);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                if (mBundleScenes.TryAdd(key, task.Result) == false)
                {
                    Debug.LogError($"[AssetManager::LoadBundleScene] key={key}");
                }
            }
        }

        public IEnumerator UnLoadBundleScene(string key)
        {
            SceneInstance obj;
            if (mBundleScenes.TryGetValue(key, out obj))
            {
                yield return Addressables.UnloadSceneAsync(obj, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }
        }

        public T LoadResourceAsset<T>(string filePath, SystemLanguage lang = SystemLanguage.Unknown) where T : UnityEngine.Object
        {
            var resPath = GetResourcePath(filePath);
            string resDir = Path.GetDirectoryName(filePath);
            if (lang == SystemLanguage.Unknown)
            {
                resDir = resDir.ToLower();
            }
            else
            {
                resDir = Path.Combine(resDir, lang.ToString()).ToLower();
            }

            Type type = typeof(T);
            var obj = Resources.Load<T>(resPath);
            if (obj != null)
            {
                registerAsset_Resource<T>(obj, resDir);
            }
            return obj;
        }


        public T[] LoadResourceAssets<T>(string searchDir, SystemLanguage lang = SystemLanguage.Unknown) where T : UnityEngine.Object
        {
            string resDir = null;
            if (lang == SystemLanguage.Unknown)
            {
                resDir = searchDir.ToLower();
            }
            else
            {
                resDir = Path.Combine(searchDir, lang.ToString()).ToLower();
            }

            T[] result = Resources.LoadAll<T>(resDir);
            foreach (var obj in result)
            {
                registerAsset_Resource<T>(obj, resDir);
            }
            return result;
        }


        public T GetAsset<T>(string fileName) where T : UnityEngine.Object
        {
            Type type = typeof(T);
            string name = Path.GetFileNameWithoutExtension(fileName).ToLower();

            // serach bundle
            {
                Dictionary<string, UnityEngine.Object> list = null;
                UnityEngine.Object obj = null;
                if (mBundleAssets.TryGetValue(type, out list))
                {
                    if (list.TryGetValue(name, out obj))
                        return obj as T;
                }
            }

            // search resource
            {
                Dictionary<string, ResData> list = null;
                ResData data = null;
                if (mResourceAssets.TryGetValue(type, out list))
                {
                    if (list.TryGetValue(name, out data))
                        return data.get() as T;
                }
            }
            return null;
        }


        public static string GetResourcePath(string path)
        {
            string ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
            {
                return path.ToLower();
            }
            else
            {
                string val = path.Substring(0, path.Length - ext.Length - 1);
                return val.ToLower();
            }
        }

        public static string[] GetResourcePathList<T>(string path)
        {
            string resPath = GetResourcePath(path);
            string ext = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(ext))
            {
                return new string[] { resPath };
            }

            var extList = new List<string>();
            extList.AddRange(getExtensions(typeof(T)));
            var result = new string[extList.Count];
            for (int i = 0; i < extList.Count; i++)
            {
                result[i] = $"{resPath}{extList[i]}";
            }
            return result;
        }


        static string[] getExtensions(Type type)
        {
            if (type == typeof(GameObject))
            {
                return new string[] { ".prefab" };
            }
            else if (type == typeof(Texture))
            {
                return new string[] { ".png", ".tga", "jpg" };
            }
            else if (type == typeof(TextAsset))
            {
                return new string[] { ".json", "cvs", ".txt" };
            }
            else if (type == typeof(AudioClip))
            {
                return new string[] { ".ogg", "mp4", ".wav" };
            }
            else if (type == typeof(Sprite))
            {
                return new string[] { "png", ".tga" };
            }
            else
            {
                return new string[0];
            }
        }


        BundleData createBundleData(string addressable, AsyncOperationHandle handle)
        {
            BundleData data = null;
            if (mBundles.TryGetValue(addressable, out data) == false)
            {
                data = new BundleData();
                data.addressable = addressable;
                data.handle = handle;
                mBundles.Add(addressable, data);
            }
            return data;
        }


        BundleData getBundleData(string addressable)
        {
            BundleData data = null;
            mBundles.TryGetValue(addressable, out data);
            return data;
        }


        bool registerAsset_Bundle<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return false;

            Type type = typeof(T);
            Dictionary<string, UnityEngine.Object> list = null;
            if (mBundleAssets.TryGetValue(type, out list) == false)
            {
                list = new Dictionary<string, UnityEngine.Object>();
                mBundleAssets.Add(type, list);
            }

            string name = obj.name.ToLower();
            if (list.ContainsKey(name))
            {
                Debug.LogError($"[AssetManager::registerAsset_Bundle] Already exist asset: type={type.Name}, name={name}");
                return false;
            }
            list.Add(name, obj);
            return true;
        }


        void registerAsset_Resource<T>(T obj, string dir) where T : UnityEngine.Object
        {
            if (obj == null)
                return;

            Type type = typeof(T);
            Dictionary<string, ResData> list = null;
            if (mResourceAssets.TryGetValue(type, out list) == false)
            {
                list = new Dictionary<string, ResData>();
                mResourceAssets.Add(type, list);
            }

            string name = obj.name.ToLower();
            if (list.ContainsKey(name))
            {
                Debug.LogError($"[AssetManager::registerAsset_Resource] Already exist asset: type={type.Name}, name={name}");
                return;
            }

            ResData<T> data = new ResData<T>();
            data.obj = obj;
            data.dir = dir;
            list.Add(name, data);
        }


        public T CreateObject<T>(string prefabName, Transform attachTr = null) where T : MonoBehaviour
        {
            if (string.IsNullOrEmpty(prefabName))
                return null;

            GameObject prefab = GetAsset<GameObject>(prefabName);
            if (prefab == null)
            {
                Debug.LogError($"[AssetManager::CreateObject] Cannot find prefab: name={prefabName}");
                return null;
            }
            GameObject obj = GameObject.Instantiate(prefab, attachTr);
            T compo = obj.GetComponent<T>();
            if (compo == null)
            {
                Debug.LogError($"[AssetManager::CreateObject] Component is not attached: name={prefabName}, type={typeof(T).Name}");
                return null;
            }
            obj.name = prefabName;
            return compo;
        }

        public T GetPrefabComponent<T>(string prefabName) where T : MonoBehaviour
        {
            if (string.IsNullOrEmpty(prefabName))
                return null;

            GameObject prefab = GetAsset<GameObject>(prefabName);
            if (prefab == null)
            {
                Debug.LogError($"[AssetManager::GetPrefabComponent] Cannot find prefab: name={prefabName}");
                return null;
            }
            T compo = prefab.GetComponent<T>();
            return compo;
        }
    }
}
