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
using System.Collections.Generic;
using UnityEngine;


namespace Devarc
{
    public class SoundChannel : MonoBehaviour
    {
        public bool IsMuted => mMuted;
        bool mMuted = false;

        CHANNEL mChannel;
        bool mIs3d;
        int mNextSEQ = 1;

        List<SoundPlay> mPool = new List<SoundPlay>();
        List<SoundPlay> mPlayList = new List<SoundPlay>();
        Dictionary<int, SoundPlay> mPlayDict = new Dictionary<int, SoundPlay>();
        Dictionary<int, Dictionary<string, float>> mCooltimes = new Dictionary<int, Dictionary<string, float>>();

        public bool IsPlaying
        {
            get
            {
                foreach (var play in mPlayList)
                {
                    switch (play.State)
                    {
                        case SoundPlay.SOUND_PLAY_STATE.PLAY:
                        case SoundPlay.SOUND_PLAY_STATE.WAIT:
                            return true;
                        default:
                            break;
                    }
                }
                return false;
            }
        }

        public void Clear()
        {
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay data = mPlayList[i];
                data.mAudio.Stop();
                Destroy(data.gameObject);
            }
            mPlayList.Clear();
            mPlayDict.Clear();
        }

        public void Init(CHANNEL _channel, int _playCount, bool _3d)
        {
            mChannel = _channel;
            mIs3d = _3d;
            mNextSEQ = (int)mChannel * 1000;

            for (int i = 0; i < _playCount; i++)
            {
                GameObject obj = new GameObject("SoundPlay");
                SoundPlay compo = obj.AddComponent<SoundPlay>();
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                compo.mAudio = obj.AddComponent<AudioSource>();
                compo.mAudio.playOnAwake = false;
                compo.mAudio.rolloffMode = AudioRolloffMode.Linear;
                if (_3d)
                {
                    compo.mAudio.dopplerLevel = 0f;
                    compo.mAudio.spatialBlend = 1f;
                    compo.mAudio.minDistance = 30f;
                    compo.mAudio.maxDistance = 100f;
                }
                mPool.Add(compo);
            }
        }


        public void StartCooltime(int group, string soundID, float cooltime)
        {
            Dictionary<string, float> list = null;
            if (mCooltimes.TryGetValue(group, out list) == false)
            {
                list = new Dictionary<string, float>();
                mCooltimes.Add(group, list);
            }
            list[soundID] = Time.realtimeSinceStartup + cooltime;
        }

        public bool IsCooltime(int group, string soundID)
        {
            Dictionary<string, float> list = null;
            if (mCooltimes.TryGetValue(group, out list) == false)
            {
                return false;
            }

            float releaseTime = 0f;
            if (list.TryGetValue(soundID, out releaseTime) == false)
            {
                return false;
            }
            return releaseTime > Time.realtimeSinceStartup;
        }

        public void StopCooltime(int group, string soundID)
        {
            Dictionary<string, float> list = null;
            if (mCooltimes.TryGetValue(group, out list) == false)
            {
                return;
            }

            float releaseTime = 0f;
            if (list.TryGetValue(soundID, out releaseTime) == false)
            {
                return;
            }
            list.Remove(soundID);
        }


        public int Play(int groupID, AudioClip clip, float volumn, SoundData soundData, float wait, float fadingTime, Vector3 pos)
        {
            SoundPlay obj = null;
            if (mPool.Count > 0)
            {
                obj = mPool[0];
                mPool.RemoveAt(0);
            }
            else
            {
                obj = mPlayList[0];
                stop(obj);
                if (mPool.Count > 0)
                {
                    obj = mPool[0];
                    mPool.RemoveAt(0);
                }
            }
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);
            obj.Init(generateSoundSEQ(), soundData.sound_id, groupID, clip, volumn, soundData.loop, mMuted, wait, fadingTime);
            if (mIs3d)
            {
                obj.mAudio.minDistance = soundData.area_close;
                obj.mAudio.maxDistance = soundData.area_far;
            }
            mPlayList.Add(obj);
            mPlayDict.Add(obj.SoundSEQ, obj);
            return obj.SoundSEQ;
        }

        public void FadeOutAll(float fadeOut)
        {
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay sound = mPlayList[i];
                if (fadeOut <= 0f)
                {
                    stop(sound);
                }
                else
                {
                    sound.FadeOut(fadeOut);
                }
            }
        }

        public void Stop(int _soundSEQ)
        {
            SoundPlay obj;
            if (mPlayDict.TryGetValue(_soundSEQ, out obj))
            {
                stop(obj);
            }
        }

        public void StopAll()
        {
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay sound = mPlayList[i];
                stop(sound);
            }
        }

        public void Mute(bool muted)
        {
            mMuted = muted;
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay sound = mPlayList[i];
                sound.Mute(muted);
            }
        }

        private void Update()
        {
            for (int i = mPlayList.Count - 1; i >= 0; i--)
            {
                SoundPlay sound = mPlayList[i];
                if (sound.Tick(Time.unscaledDeltaTime))
                {
                    stop(sound);
                }
            }
        }

        int generateSoundSEQ()
        {
            do
            {
                mNextSEQ++;
                if (mNextSEQ >= 10000)
                {
                    mNextSEQ = (int)mChannel * 1000;
                }
            } while (mPlayDict.ContainsKey(mNextSEQ));
            return mNextSEQ;
        }

        void stop(SoundPlay obj)
        {
            int soundSEQ = obj.SoundSEQ;
            obj.Stop();

            mPlayList.Remove(obj);
            mPlayDict.Remove(soundSEQ);
            obj.gameObject.SetActive(false);
            mPool.Add(obj);

            StopCooltime(obj.GroupID, obj.SoundID);
        }
    }
}