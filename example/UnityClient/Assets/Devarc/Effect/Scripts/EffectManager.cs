using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


namespace Devarc
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        SimplePool<BaseEffect> mPool = new SimplePool<BaseEffect>();

        protected override void onAwake()
        {
            mPool.InitRoot(transform);
        }

        protected override void onDestroy()
        {
            mPool.Clear();
        }

        public BaseEffect CreateEffect(EffectPlayData data, Vector3 worldPos, bool flipX = false)
        {
            if (data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            GameObject prefab = AssetManager.Instance.GetAsset<GameObject>(data.EffectID);
            if (prefab == null)
            {
                Debug.LogError($"[EffectManager] Cannot play effect: effect_id={data.EffectID.Value}");
                return null;
            }

            Vector3 offset = data.Offset;
            if (flipX)
            {
                offset.x = -offset.x;
            }

            BaseEffect obj = mPool.Pop(prefab, mPool.Root, worldPos + offset);
            obj.Play(data.WaitTime);
            return obj;
        }


        public BaseEffect CreateEffect(EffectPlayData data, Transform attachTr, bool flipX = false)
        {
            if (data == null || data.EffectID == null || data.EffectID.IsValid == false)
            {
                return null;
            }

            GameObject prefab = AssetManager.Instance.GetAsset<GameObject>(data.EffectID);
            if (prefab == null)
            {
                return null;
            }

            Vector3 offset = data.Offset;
            if (flipX)
            {
                offset.x = -offset.x;
            }

            BaseEffect obj = mPool.Pop(prefab, attachTr, offset);
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
