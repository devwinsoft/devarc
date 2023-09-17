using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Devarc
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        internal class BundleData
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

        Dictionary<Type, Dictionary<string, UnityEngine.Object>> mBundleAssets = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();
        Dictionary<string, GameObject> mBundlePrefabs = new Dictionary<string, GameObject>();

        Dictionary<Type, Dictionary<string, UnityEngine.Object>> mResourceAssets = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();
        Dictionary<string, GameObject> mResourcePrefabs = new Dictionary<string, GameObject>();


#if UNITY_EDITOR
        public static GameObject[] LoadPrefabs_Database(string searchDir)
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


        public static T[] LoadPrefabs_Database<T>(string fileName, string searchDir) where T : MonoBehaviour
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


        public static T[] LoadAssets_Database<T>(string fileName, string searchDir) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            var list = AssetDatabase.FindAssets($"t:{typeof(T).Name} {fileName}", new string[] { searchDir });
            foreach (var guid in list)
            {
                var tempPath = AssetDatabase.GUIDToAssetPath(guid);
                var compo = AssetDatabase.LoadAssetAtPath<T>(tempPath);
                if (compo != null)
                    result.Add(compo);
            }
            return result.ToArray();
        }
#endif


        public void UnloadAsset_Resource<T>(string fileName) where T : UnityEngine.Object
        {
            string name = fileName.ToLower();
            Type type = typeof(T);

            Dictionary<string, UnityEngine.Object> list = null;
            if (mResourceAssets.TryGetValue(type, out list) == false)
            {
                list = new Dictionary<string, UnityEngine.Object>();
                mResourceAssets.Add(type, list);
            }

            UnityEngine.Object obj = null;
            if (list.TryGetValue(name, out obj))
            {
                list.Remove(name);
                Resources.UnloadAsset(obj);
            }
        }


        public void UnloadAssets_Bundle(string key, System.Action<string> callback = null)
        {
            BundleData bundleData;
            if (mBundles.TryGetValue(key, out bundleData))
            {
                foreach (var name in bundleData.list)
                {
                    foreach (var list in mBundleAssets.Values)
                    {
                        list.Remove(name);
                        callback?.Invoke(name);
                    }
                }
                bundleData.list.Clear();
                Addressables.Release(bundleData.handle);
            }
            mBundles.Remove(key);
        }


        public IEnumerator LoadAsset_Bundle<T>(string key, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetAsync<T>(key);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                var obj = task.Result;
                registerAsset_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }
            createBundleData(key, task);
        }

        public AsyncOperationHandle<IList<T>> LoadAssets_Bundle<T>(string key, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetsAsync<T>(key, (obj) =>
            {
                registerAsset_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            });
            createBundleData(key, task);
            return task;
        }


        public AsyncOperationHandle<IList<T>> LoadAssets_Bundle<T>(string key, SystemLanguage lang, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            List<string> keys = new List<string> { key, lang.ToString() };
            var task = Addressables.LoadAssetsAsync<T>(keys, (obj) =>
            {
                registerAsset_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }, Addressables.MergeMode.Intersection);
            createBundleData(key, task);
            return task;
        }


        public IEnumerator LoadPrefab_Bundle(string key, System.Action<GameObject> callback = null)
        {
            var task = Addressables.LoadAssetAsync<GameObject>(key);
            createBundleData(key, task);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = task.Result;
                registerPrefab_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }
        }


        public AsyncOperationHandle<IList<GameObject>> LoadPrefabs_Bundle(string key, System.Action<GameObject> callback = null)
        {
            var task = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                registerPrefab_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            });
            createBundleData(key, task);
            return task;
        }

        public AsyncOperationHandle<IList<GameObject>> LoadPrefabs_Bundle(string key, SystemLanguage lang, System.Action<GameObject> callback = null)
        {
            List<string> keys = new List<string> { key, lang.ToString() };
            var task = Addressables.LoadAssetsAsync<GameObject>(keys, (obj) =>
            {
                registerPrefab_Bundle(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }, Addressables.MergeMode.Intersection);

            createBundleData(key, task);
            return task;
        }


        public IEnumerator LoadPrefabs_Bundle<T>(string key, System.Action<T> callback = null) where T : MonoBehaviour
        {
            var task = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                registerPrefab_Bundle(obj);
                getBundleData(key)?.Add(obj.name);

                T compo = obj.GetComponent<T>();
                if (compo != null)
                {
                    callback?.Invoke(compo);
                }
            });
            yield return task;
            createBundleData(key, task);
        }


        public T LoadAsset_Resource<T>(string filePath) where T : UnityEngine.Object
        {
            Type type = typeof(T);
            var resPath = GetResourcePath(filePath);
            var obj = Resources.Load<T>(resPath);
            if (obj != null)
            {
                registerAsset_Resource<T>(obj);
            }
            return obj;
        }


        public T[] LoadAssets_Resource<T>(string searchDir) where T : UnityEngine.Object
        {
            var resPath = GetResourcePath(searchDir);
            T[] result = Resources.LoadAll<T>(resPath);
            foreach (var obj in result)
            {
                registerAsset_Resource<T>(obj);
            }
            return result;
        }


        public GameObject LoadPrefab_Resource(string filePath)
        {
            Addressables.CleanBundleCache();

            var resPath = GetResourcePath(filePath);
            var obj = Resources.Load<GameObject>(resPath);
            if (obj == null)
                return null;
            registerPrefab_Bundle(obj);
            return obj;
        }


        public T LoadPrefab_Resource<T>(string searchDir) where T : UnityEngine.MonoBehaviour
        {
            var resPath = GetResourcePath(searchDir);
            var obj = Resources.Load<GameObject>(resPath);
            if (obj == null)
                return null;
            registerPrefab_Bundle(obj);
            return obj.GetComponent<T>();
        }


        public GameObject[] LoadPrefabs_Resource(string searchDir)
        {
            GameObject[] list = Resources.LoadAll<GameObject>(searchDir);
            foreach (GameObject obj in list)
            {
                registerPrefab_Bundle(obj);
            }
            return list;
        }


        public T[] LoadPrefabs_Resource<T>(string searchDir) where T : MonoBehaviour
        {
            List<T> result = new List<T>();
            GameObject[] list = Resources.LoadAll<GameObject>(searchDir);
            foreach (GameObject obj in list)
            {
                registerPrefab_Bundle(obj);

                T compo = obj.GetComponent<T>();
                if (compo != null)
                {
                    result.Add(compo);
                }
            }
            return result.ToArray();
        }


        public T GetAsset<T>(string path) where T : UnityEngine.Object
        {
            Type type = typeof(T);
            Dictionary<string, UnityEngine.Object> list = null;
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            UnityEngine.Object obj = null;

            if (mResourceAssets.TryGetValue(type, out list))
            {
                if (list.TryGetValue(name, out obj))
                    return obj as T;
            }

            if (mBundleAssets.TryGetValue(type, out list))
            {
                if (list.TryGetValue(name, out obj))
                    return obj as T;
            }

            return null;
        }


        public GameObject GetPrefab(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            GameObject obj = null;
            if (mBundlePrefabs.TryGetValue(name, out obj))
            {
                return obj;
            }
            if (mResourcePrefabs.TryGetValue (name, out obj))
            {
                return obj;
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


        void registerAsset_Bundle<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return;

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
                return;
            }
            list.Add(name, obj);
        }


        void registerAsset_Resource<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return;

            Type type = typeof(T);
            Dictionary<string, UnityEngine.Object> list = null;
            if (mResourceAssets.TryGetValue(type, out list) == false)
            {
                list = new Dictionary<string, UnityEngine.Object>();
                mResourceAssets.Add(type, list);
            }

            string name = obj.name.ToLower();
            if (list.ContainsKey(name))
            {
                Debug.LogError($"[AssetManager::registerAsset_Resource] Already exist asset: type={type.Name}, name={name}");
                return;
            }
            list.Add(name, obj);
        }


        void registerPrefab_Bundle(GameObject obj)
        {
            if (obj == null)
                return;

            string name = obj.name.ToLower();
            if (mBundlePrefabs.ContainsKey(name))
            {
                Debug.LogError($"[AssetManager::registerPrefab_Bundle] Already exist prefab: name={name}");
                return;
            }
            mBundlePrefabs.Add(obj.name, obj);
        }


        void registerPrefab_Resource(GameObject obj)
        {
            if (obj == null)
                return;

            string name = obj.name.ToLower();
            if (mResourcePrefabs.ContainsKey(name))
            {
                Debug.LogError($"[AssetManager::registerPrefab_Resource] Already exist prefab: name={name}");
                return;
            }
            mResourcePrefabs.Add(name, obj);
        }
    }
}
