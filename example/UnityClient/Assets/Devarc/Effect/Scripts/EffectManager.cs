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
            mPool.Clear();
        }

        public IEnumerator Preload(string addressable)
        {
            yield return AssetManager.Instance.LoadPrefabs_Bundle<BaseEffect>(addressable, (obj) =>
            {
                mPrefabs.Add(obj.name, obj);
            });
        }

        public BaseEffect CreateEffect(BaseEffectData data, Vector3 worldPos, bool flipX = false)
        {
            if (data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            BaseEffect prefab = null;
            if (mPrefabs.TryGetValue(data.EffectID, out prefab) == false)
            {
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


        public BaseEffect CreateEffect(BaseEffectData data, Transform attachTr, bool flipX = false)
        {
            if (data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            BaseEffect prefab = null;
            if (mPrefabs.TryGetValue(data.EffectID, out prefab) == false)
            {
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

        public void Push(BaseEffect _effect)
        {
            if (_effect == null)
            {
                return;
            }
            mPool.Push(_effect);
        }
    }

} // end of namespace
