// Copyright (c) 2021 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    public enum CHANNEL
    {
        BGM,
        EFFECT,
        UI,
        VOICE,
        MAX,
    }

    public abstract class SoundData
    {
        public abstract string sound_id { get; }
        public abstract string key { get; }
        public abstract string path { get; }
        public abstract float volume { get; }
        public abstract float cooltime { get; }
        public abstract bool loop { get; }
        public abstract bool isBundle { get; }
    }

    public class BundleSoundData : SoundData
    {
        public string addressKey;
        public SOUND_BUNDLE data;

        public override string sound_id => data.sound_id;
        public override string key => addressKey;
        public override string path => data.path;
        public override float volume => data.volume;
        public override float cooltime => data.cooltime;
        public override bool loop => data.loop;
        public override bool isBundle => false;
    }

    public class ResourceSoundData : SoundData
    {
        public SOUND_RESOURCE data;

        public override string sound_id => data.sound_id;
        public override string key => data.key;
        public override string path => data.path;
        public override float volume => data.volume;
        public override float cooltime => data.cooltime;
        public override bool loop => data.loop;
        public override bool isBundle => false;
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        public SoundChannel[] Channels => mChannels;
        SoundChannel[] mChannels = new SoundChannel[(int)CHANNEL.MAX];

        Dictionary<string, List<SoundData>> mSoundDatas = new Dictionary<string, List<SoundData>>();

        protected override void onAwake()
        {
            createChannel(CHANNEL.BGM, 2, false);
            createChannel(CHANNEL.EFFECT, 32, true);
            createChannel(CHANNEL.VOICE, 2, false);
            createChannel(CHANNEL.UI, 4, false);
        }

        protected override void onDestroy()
        {
        }


        public void LoadResource(string key = null)
        {
            foreach (var data in Table.SOUND_RESOURCE.List)
            {
                if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(data.key))
                {
                    // Load global sounds.
                    register(data);
                    AssetManager.Instance.LoadResourceAsset<AudioClip>(data.path);
                }
                else if (key == data.key)
                {
                    // Load specific sounds.
                    register(data);
                    AssetManager.Instance.LoadResourceAsset<AudioClip>(data.path);
                }
            }
        }


        public void UnloadResource(string key = null)
        {
            foreach (var data in Table.SOUND_RESOURCE.List)
            {
                if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(data.key))
                {
                    AssetManager.Instance.UnloadResourceAsset<AudioClip>(data.path);
                }
                else if (key == data.key)
                {
                    AssetManager.Instance.UnloadResourceAsset<AudioClip>(data.path);
                }
            }
            unRegister(key, false);
        }


        public IEnumerator LoadBundle(string addressKey, SystemLanguage lang = SystemLanguage.Unknown)
        {
            foreach (var data in Table.SOUND_BUNDLE.List)
            {
                register(data, addressKey);
            }

            if (lang == SystemLanguage.Unknown)
            {
                var handle = AssetManager.Instance.LoadBundleAssets<AudioClip>(addressKey);
                yield return handle;
            }
            else
            {
                var handle = AssetManager.Instance.LoadBundleAssets<AudioClip>(addressKey, lang);
                yield return handle;
            }
        }


        public void UnloadBundle(string key)
        {
            AssetManager.Instance.UnloadBundleAssets(key);
            unRegister(key, true);
        }


        void register(SOUND_BUNDLE data, string addressKey)
        {
            List<SoundData> list;
            if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
            {
                list = new List<SoundData>();
                mSoundDatas.Add(data.sound_id, list);
            }
            var obj = new BundleSoundData();
            obj.addressKey = addressKey;
            obj.data = data;
            list.Add(obj);
        }

        void register(SOUND_RESOURCE data)
        {
            List<SoundData> list;
            if (mSoundDatas.TryGetValue(data.sound_id, out list) == false)
            {
                list = new List<SoundData>();
                mSoundDatas.Add(data.sound_id, list);
            }
            var obj = new ResourceSoundData();
            obj.data = data;
            list.Add(obj);
        }


        void unRegister(string addressKey, bool isBundle)
        {
            foreach (var list in mSoundDatas.Values)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var obj = list[i];
                    if (obj.isBundle != isBundle)
                        continue;
                    if (obj.key != addressKey)
                        continue;

                    list.RemoveAt(i);
                }
            }
        }

        public bool IsPlaying(CHANNEL channel)
        {
            return mChannels[(int)channel].IsPlaying;
        }

        public int PlaySound(CHANNEL channel, int index)
        {
            {
                var tableData = Table.SOUND_BUNDLE.Get(index);
                if (tableData != null)
                    return PlaySound(channel, tableData.sound_id, 0, 0f, Vector3.zero);
            }
            {
                var tableData = Table.SOUND_RESOURCE.Get(index);
                if (tableData != null)
                    return PlaySound(channel, tableData.sound_id, 0, 0f, Vector3.zero);
            }
            return 0;
        }

        public int PlaySound(CHANNEL channel, string soundID)
        {
            return PlaySound(channel, soundID, 0, 0f, Vector3.zero);
        }

        public int PlaySound(CHANNEL channel, string soundID, Vector3 pos)
        {
            return PlaySound(channel, soundID, 0, 0f, pos);
        }

        public int PlaySound(CHANNEL channel, string soundID, int groupID, float fadeIn, Vector3 pos)
        {
            if (string.IsNullOrEmpty(soundID))
                return 0;

            List<SoundData> list = null;
            if (mSoundDatas.TryGetValue(soundID, out list) == false || list.Count == 0)
            {
                Debug.LogError($"[SoundManager] Cannot find sound_id: {soundID}");
                return 0;
            }

            var soundData = list[Random.Range(0, list.Count - 1)];
            var clip = AssetManager.Instance.GetAsset<AudioClip>(soundData.path);
            if (clip == null)
            {
                Debug.LogError($"[SoundManager] Cannot find audio clip: sound_id={soundID}, path={soundData.path}");
                return 0;
            }
            var obj = mChannels[(int)channel];
            if (obj.IsCooltime(groupID, soundData.sound_id))
            {
                return 0;
            }
            if (soundData.cooltime > 0f)
            {
                obj.StartCooltime(groupID, soundID, soundData.cooltime);
            }
            return obj.Play(groupID, clip, soundData.volume, soundData, 0f, fadeIn, pos);
        }


        public void FadeOut(CHANNEL channel, float _fadeOutTime)
        {
            mChannels[(int)channel].FadeOutAll(_fadeOutTime);
        }

        public void Stop(CHANNEL channel, int _soundSEQ)
        {
            mChannels[(int)channel].Stop(_soundSEQ);
        }

        public void StopAll(CHANNEL channel)
        {
            mChannels[(int)channel].StopAll();
        }

        void createChannel(CHANNEL _channel, int _pool, bool _3d)
        {
            GameObject obj = new GameObject(_channel.ToString());
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            SoundChannel compo = obj.AddComponent<SoundChannel>();
            compo.Init(_channel, _pool, _3d);
            mChannels[(int)_channel] = compo;
        }
    }
}