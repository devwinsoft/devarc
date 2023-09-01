using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Devarc
{
    public class AssetManager : SingletonManager<AssetManager>
    {
        Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();
        Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
        Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
        Dictionary<string, TextAsset> mTextAssets = new Dictionary<string, TextAsset>();

        protected override void onAwake()
        {
        }

        protected override void onDestroy()
        {
        }


        IEnumerator LoadAudioClip(string key)
        {
            var handle = Addressables.LoadAssetsAsync<AudioClip>(key, (obj) =>
            {
                mAudioClips.Add(obj.name, obj);
            });
            yield return handle;
        }

        IEnumerator LoadPrefabs(string key)
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>(key, (obj) =>
            {
                mPrefabs.Add(obj.name, obj);
            });
            yield return handle;
        }

        IEnumerator LoadSprites(string key, float ppu = 100)
        {
            var handle = Addressables.LoadAssetsAsync<Texture2D>(key, (obj) =>
            {
                Sprite sprite = Sprite.Create(obj, new Rect(0, 0, obj.width, obj.height), new Vector2(0.5f, 0.5f), ppu);
                mSprites.Add(obj.name, sprite);
            });
            yield return handle;
        }

        IEnumerator LoadTextures(string key)
        {
            var handle = Addressables.LoadAssetsAsync<Texture>(key, (obj) =>
            {
                mTextures.Add(obj.name, obj);
            });
            yield return handle;
        }


        IEnumerator LoadTextAssets(string key)
        {
            var handle = Addressables.LoadAssetsAsync<TextAsset>(key, (obj) =>
            {
                mTextAssets.Add(obj.name, obj);
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
            if (mTextAssets.TryGetValue(name, out obj))
                return obj;
            if (mTextAssets.TryGetValue(System.IO.Path.GetFileNameWithoutExtension(name), out obj))
                return obj;
            return null;
        }
    }
}
