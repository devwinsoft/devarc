using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


namespace Devarc
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        Dictionary<string, BaseEffect> mPrefabs = new Dictionary<string, BaseEffect>();
        SimplePool<BaseEffect> mPool = new SimplePool<BaseEffect>();

        protected override void onAwake()
        {
            mPool.InitRoot(transform);
        }

        protected override void onDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            mPool.Clear();
        }


        public IEnumerator LoadBundle(string addressKey)
        {
            var handle = AssetManager.Instance.LoadBundleAssets<GameObject>(addressKey);
            yield return handle;

            if (handle.IsValid())
            {
                foreach (var obj in handle.Result)
                {
                    var compo = obj.GetComponent<BaseEffect>();
                    if (compo == null)
                        continue;
                    mPrefabs.TryAdd(obj.name, compo);
                }
            }
        }


        public void UnloadBundle(string addressKey)
        {
            var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
            foreach (var name in removeList)
            {
                mPrefabs.Remove(name);
                mPool.Remove(name);
            }
        }


        public BaseEffect CreateEffect(EffectPlayData data, Vector3 worldPos, bool flipX = false)
        {
            if (data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            BaseEffect prefab = null; 
            if (mPrefabs.TryGetValue(data.EffectID, out prefab) == false)
            {
                Debug.LogError($"[EffectManager::CreateEffect] Cannot find prefab: effect_id={data.EffectID.Value}");
                return null;
            }

            Vector3 offset = data.Offset;
            if (flipX)
            {
                offset.x = -offset.x;
            }

            BaseEffect obj = mPool.Pop(prefab.gameObject, mPool.Root, worldPos + offset);
            obj.Play(data.WaitTime);
            return obj;
        }


        public BaseEffect CreateEffect(EffectPlayData data, Transform attachTr, bool flipX = false)
        {
            if (data == null || data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            BaseEffect prefab = null;
            if (mPrefabs.TryGetValue(data.EffectID, out prefab) == false)
            {
                Debug.LogError($"[EffectManager::CreateEffect] Cannot find prefab: effect_id={data.EffectID.Value}");
                return null;
            }

            Vector3 offset = data.Offset;
            if (flipX)
            {
                offset.x = -offset.x;
            }

            BaseEffect obj = mPool.Pop(prefab.gameObject, attachTr, offset);
            if (obj == null)
            {
                Debug.LogErrorFormat("Cannot find effect_id: {0}", data.EffectID.Value);
                return null;
            }

            obj.Play(data.WaitTime);
            return obj;
        }


        public void Remove(BaseEffect _effect)
        {
            if (_effect == null)
            {
                return;
            }
            mPool.Push(_effect);
        }
    }

} // end of namespace
