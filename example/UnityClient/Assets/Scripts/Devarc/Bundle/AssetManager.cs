using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.CodeDom;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Devarc
{
    public class AssetManager : SingletonManager<AssetManager>
    {
        Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();
        Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
        Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
        Dictionary<string, TextAsset> mTexts = new Dictionary<string, TextAsset>();

        protected override void onAwake()
        {
        }

        protected override void onDestroy()
        {
        }

        public static GameObject LoadPrefabAtPath(string path)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                return Instance.GetPrefab(name);
            }
            else
            {
                string editorPath = Path.Combine("Assets", path);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(editorPath + ".prefab");
                return obj;
            }
#else
            string name = Path.GetFileNameWithoutExtension(path);
            return Instance.GetPrefab(name);
#endif
        }


        public static T LoadAssetAtPath<T>(string path, bool bundle = true) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (bundle)
                {
                    Type type = typeof(T);
                    string name = Path.GetFileNameWithoutExtension(path);
                    if (type == typeof(GameObject))
                    {
                        return Instance.GetPrefab(name) as T;
                    }
                    else if (type == typeof(Texture))
                    {
                        return Instance.GetTexture(name) as T;
                    }
                    else if (type == typeof(TextAsset))
                    {
                        return Instance.GetTextAsset(name) as T;
                    }
                    else if (type == typeof(AudioClip))
                    {
                        return Instance.GetAudioClip(name) as T;
                    }
                    else if (type == typeof(Sprite))
                    {
                        return Instance.GetSprite(name) as T;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    T obj = Resources.Load<T>(path);
                    return obj;
                }
            }
            else
            {
                if (bundle)
                {
                    var editorPath = Path.Combine("Assets", path);
                    var extensions = getExtensions(typeof(T));
                    foreach (var ext in extensions)
                    {
                        T obj = AssetDatabase.LoadAssetAtPath<T>(editorPath + ext);
                        if (obj != null)
                        {
                            return obj;
                        }
                    }
                    return null;
                }
                else
                {
                    T obj = Resources.Load<T>(path);
                    return obj;
                }
            }
#else
            if (bundle)
            {
                Type type = typeof(T);
                string name = Path.GetFileNameWithoutExtension(path);
                if (type == typeof(GameObject))
                {
                    return Instance.GetPrefab(name) as T;
                }
                else if (type == typeof(Texture))
                {
                    return Instance.GetTexture(name) as T;
                }
                else if (type == typeof(TextAsset))
                {
                    return Instance.GetTextAsset(name) as T;
                }
                else if (type == typeof(AudioClip))
                {
                    return Instance.GetAudioClip(name) as T;
                }
                else if (type == typeof(Sprite))
                {
                    return Instance.GetSprite(name) as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                T obj = Resources.Load<T>(path);
                return obj;
            }
#endif
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


        public IEnumerator LoadAudioClip(string key)
        {
            var handle = Addressables.LoadAssetsAsync<AudioClip>(key, (obj) =>
            {
                mAudioClips.Add(obj.name, obj);
            });
            yield return handle;
        }

        public IEnumerator LoadPrefabs(string key)
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                mPrefabs.Add(obj.name, obj);
            });
            yield return handle;
        }

        IEnumerator LoadSprites(string key)
        {
            var handle = Addressables.LoadAssetsAsync<Sprite>(key, (obj) =>
            {
                mSprites.Add(obj.name, obj);
            });
            yield return handle;
        }

        public IEnumerator LoadTextures(string key)
        {
            var handle = Addressables.LoadAssetsAsync<Texture>(key, (obj) =>
            {
                mTextures.Add(obj.name, obj);
            });
            yield return handle;
        }

        public IEnumerator LoadTextAssets(string key)
        {
            var handle = Addressables.LoadAssetsAsync<TextAsset>(key, (obj) =>
            {
                mTexts.Add(obj.name, obj);
            });
            yield return handle;
        }


        public AudioClip GetAudioClip(string name)
        {
            AudioClip obj = null;
            mAudioClips.TryGetValue(name, out obj);
            return obj;
        }

        public GameObject GetPrefab(string name)
        {
            GameObject obj = null;
            if (mPrefabs.TryGetValue(name, out obj))
                return obj;
            if (mPrefabs.TryGetValue(Path.GetFileNameWithoutExtension(name), out obj))
                return obj;
            return obj;
        }

        public Sprite GetSprite(string name)
        {
            Sprite obj = null;
            mSprites.TryGetValue(name, out obj);
            return obj;
        }

        public TextAsset GetTextAsset(string name)
        {
            TextAsset obj = null;
            if (mTexts.TryGetValue(name, out obj))
                return obj;
            if (mTexts.TryGetValue(System.IO.Path.GetFileNameWithoutExtension(name), out obj))
                return obj;
            return null;
        }

        public Texture GetTexture(string name)
        {
            Texture obj = null;
            mTextures.TryGetValue(name, out obj);
            return obj;
        }
    }
}
