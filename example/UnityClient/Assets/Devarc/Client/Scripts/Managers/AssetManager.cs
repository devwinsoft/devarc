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

        Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();
        Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
        Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
        Dictionary<string, TextAsset> mTextAssets = new Dictionary<string, TextAsset>();


#if UNITY_EDITOR
        public static GameObject[] LoadDatabase_Prefabs(string searchDir)
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


        public static T[] LoadDatabase_Prefabs<T>(string fileName, string searchDir) where T : MonoBehaviour
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


        public static T[] LoadDatabase_Assets<T>(string fileName, string searchDir) where T : UnityEngine.Object
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


        public void UnLoadResource<T>(string fileName) where T : UnityEngine.Object
        {
            string name = fileName.ToLower();
            Type type = typeof(T);
            if (type == typeof(Texture))
            {
                mTextures.Remove(name);
            }
            else if (type == typeof(TextAsset))
            {
                mTextAssets.Remove(name);
            }
            else if (type == typeof(AudioClip))
            {
                mAudioClips.Remove(name);
            }
            else if (type == typeof(Sprite))
            {
                mSprites.Remove(name);
            }

            mTextures.Remove(name);
            mTextAssets.Remove(name);
            mAudioClips.Remove(name);
            mSprites.Remove(name);
            mPrefabs.Remove(name);
        }


        public void UnLoadAssets(string key)
        {
            BundleData bundleData;
            if (mBundles.TryGetValue(key, out bundleData))
            {
                foreach (var fileName in bundleData.list)
                {
                    mTextures.Remove(fileName);
                    mTextAssets.Remove(fileName);
                    mAudioClips.Remove(fileName);
                    mSprites.Remove(fileName);
                    mPrefabs.Remove(fileName);
                }
                bundleData.list.Clear();
                Addressables.Release(bundleData.handle);
            }
            mBundles.Remove(key);
        }


        public IEnumerator LoadBundle_Asset<T>(string key, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetAsync<T>(key);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                var obj = task.Result;
                registerAsset(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }
            createBundleData(key, task);
        }

        public IEnumerator LoadBundle_Assets<T>(string key, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            var task = Addressables.LoadAssetsAsync<T>(key, (obj) =>
            {
                registerAsset(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            });
            yield return task;
            createBundleData(key, task);
        }


        public IEnumerator LoadBundle_AudioClip(string key, System.Action<AudioClip> callback = null)
        {
            var task = Addressables.LoadAssetAsync<AudioClip>(key);
            yield return task;

            var obj = task.Result;
            if (obj != null)
            {
                registerAsset<AudioClip>(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }
            createBundleData(key, task);
        }




        public IEnumerator LoadBundle_Prefab(string key, System.Action<GameObject> callback = null)
        {
            var task = Addressables.LoadAssetAsync<GameObject>(key);
            yield return task;
            if (task.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = task.Result;
                registerPrefab(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            }
            createBundleData(key, task);
        }


        public IEnumerator LoadBundle_Prefabs(string key, System.Action<GameObject> callback = null)
        {
            var task = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                registerPrefab(obj);
                getBundleData(key)?.Add(obj.name);
                callback?.Invoke(obj);
            });
            yield return task;
            createBundleData(key, task);
        }


        public IEnumerator LoadBundle_Prefabs<T>(string key, System.Action<T> callback = null) where T : MonoBehaviour
        {
            var task = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                registerPrefab(obj);
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


        public IEnumerator LoadBundle_Sprites(string key)
        {
            var task = Addressables.LoadAssetsAsync<Sprite>(key, (obj) =>
            {
                mSprites.Add(obj.name.ToLower(), obj);
            });
            yield return task;
            createBundleData(key, task);
        }

        public IEnumerator LoadBundle_Textures(string key)
        {
            var task = Addressables.LoadAssetsAsync<Texture>(key, (obj) =>
            {
                mTextures.Add(obj.name.ToLower(), obj);
            });
            yield return task;
            createBundleData(key, task);
        }

        public IEnumerator LoadBundle_TextAssets(string key, System.Action<TextAsset> callback = null)
        {
            var task = Addressables.LoadAssetsAsync<TextAsset>(key, (obj) =>
            {
                mTextAssets.Add(obj.name.ToLower(), obj);
                callback?.Invoke(obj);
            });
            yield return task;
            createBundleData(key, task);
        }


        public T LoadResource_Asset<T>(string filePath) where T : UnityEngine.Object
        {
            Type type = typeof(T);
            var resPath = GetResourcePath(filePath);
            var obj = Resources.Load<T>(resPath);
            if (obj != null)
            {
                registerAsset<T>(obj);
            }
            return obj;
        }


        public T[] LoadResource_Assets<T>(string searchDir) where T : UnityEngine.Object
        {
            var resPath = GetResourcePath(searchDir);
            T[] result = Resources.LoadAll<T>(resPath);
            foreach (var obj in result)
            {
                registerAsset<T>(obj);
            }
            return result;
        }


        public GameObject LoadResource_Prefab(string filePath)
        {
            Addressables.CleanBundleCache();

            var resPath = GetResourcePath(filePath);
            var obj = Resources.Load<GameObject>(resPath);
            if (obj == null)
                return null;
            registerPrefab(obj);
            return obj;
        }


        public T LoadResource_Prefab<T>(string searchDir) where T : UnityEngine.MonoBehaviour
        {
            var resPath = GetResourcePath(searchDir);
            var obj = Resources.Load<GameObject>(resPath);
            if (obj == null)
                return null;
            registerPrefab(obj);
            return obj.GetComponent<T>();
        }


        public GameObject[] LoadResource_Prefabs(string searchDir)
        {
            GameObject[] list = Resources.LoadAll<GameObject>(searchDir);
            foreach (GameObject obj in list)
            {
                registerPrefab(obj);
            }
            return list;
        }


        public T[] LoadResource_Prefabs<T>(string searchDir) where T : MonoBehaviour
        {
            List<T> result = new List<T>();
            GameObject[] list = Resources.LoadAll<GameObject>(searchDir);
            foreach (GameObject obj in list)
            {
                registerPrefab(obj);

                T compo = obj.GetComponent<T>();
                if (compo != null)
                {
                    result.Add(compo);
                }
            }
            return result.ToArray();
        }


        public AudioClip GetAudioClip(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            AudioClip obj = null;
            mAudioClips.TryGetValue(name, out obj);
            return obj;
        }


        public GameObject GetPrefab(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            GameObject obj = null;
            mPrefabs.TryGetValue(name, out obj);
            return obj;
        }


        public Sprite GetSprite(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            Sprite obj = null;
            mSprites.TryGetValue(name, out obj);
            return obj;
        }


        public TextAsset GetTextAsset(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            TextAsset obj = null;
            mTextAssets.TryGetValue(name, out obj);
            return obj;
        }


        public Texture GetTexture(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLower();
            Texture obj = null;
            mTextures.TryGetValue(name, out obj);
            return obj;
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


        void registerAsset<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return;

            Type type = typeof(T);
            if (type == typeof(Texture))
            {
                mTextures.Add(obj.name.ToLower(), obj as Texture);
            }
            else if (type == typeof(TextAsset))
            {
                mTextAssets.Add(obj.name.ToLower(), obj as TextAsset);
            }
            else if (type == typeof(AudioClip))
            {
                mAudioClips.Add(obj.name.ToLower(), obj as AudioClip);
            }
            else if (type == typeof(Sprite))
            {
                mSprites.Add(obj.name.ToLower(), obj as Sprite);
            }
        }


        public void registerPrefab(GameObject prefab)
        {
            GameObject temp;
            if (mPrefabs.TryGetValue(prefab.name, out temp))
            {
                if (prefab == temp)
                    Debug.LogError($"[AssetManager] Duplicate object name:{prefab.name}");
            }
            else
            {
                mPrefabs.Add(prefab.name, prefab);
            }
        }


        public T GetAsset<T>(string path) where T : UnityEngine.Object
        {
            Type type = typeof(T);
            if (type == typeof(Texture))
            {
                return GetTexture(path) as T;
            }
            else if (type == typeof(TextAsset))
            {
                return GetTextAsset(path) as T;
            }
            else if (type == typeof(AudioClip))
            {
                return GetAudioClip(path) as T;
            }
            else if (type == typeof(Sprite))
            {
                return GetSprite(path) as T;
            }
            else
            {
                return null;
            }
        }

    }
}
