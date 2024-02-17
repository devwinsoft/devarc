using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


namespace Devarc
{
    public enum EFFECT_ATTACH_TYPE
    {
        World,
        Ground,
        Unit,
    }


    public class EffectManager : MonoSingleton<EffectManager>
    {
        SimplePool<BaseEffect> mPool = new SimplePool<BaseEffect>();

        protected override void onAwake()
        {
            mPool.SetRoot(transform);
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
        }


        public void UnloadBundle(string addressKey)
        {
            var removeList = AssetManager.Instance.UnloadBundleAssets(addressKey);
            foreach (var name in removeList)
            {
                mPool.Remove(name);
            }
        }


        public BaseEffect CreateEffect(EFFECT_ID effectID, Transform attachTr, Vector3 offset, Vector3 localRotation, EFFECT_ATTACH_TYPE attachType)
        {
            if (effectID == null || effectID.IsValid == false)
            {
                return null;
            }

            BaseEffect obj = mPool.Pop(effectID, attachTr, offset, Quaternion.Euler(localRotation));
            if (obj == null)
            {
                return null;
            }
            switch (attachType)
            {
                case EFFECT_ATTACH_TYPE.Ground:
                    Ray ray = new Ray(obj.transform.position + new Vector3(0f, 100f, 0f), Vector3.down);
                    RaycastHit hit;
                    if (Physics.SphereCast(ray, 0.01f, out hit, 1000f))
                    {
                        obj.transform.position = hit.point;
                        obj.transform.SetParent(mPool.Root, true);
                    }
                    break;
                case EFFECT_ATTACH_TYPE.Unit:
                    break;
                default:
                    obj.transform.SetParent(mPool.Root, true);
                    break;
            }
            obj.Play();
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
