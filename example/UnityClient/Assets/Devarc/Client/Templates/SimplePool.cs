//
// Copyright (c) 2021 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
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
using System.IO;
using UnityEngine;

namespace Devarc
{
    public interface ISimpleObject
    {
        void OnPoolEvent_Pop();
        void OnPoolEvent_Push();
    }

    public class SimplePool<T> where T : MonoBehaviour, ISimpleObject
    {
        public Transform Root => mRoot;
        Transform mRoot = null;

        Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, List<T>> mPool = new Dictionary<string, List<T>>();

        public void Clear()
        {
            foreach (var list in mPool.Values)
            {
                foreach (var obj in list)
                {
                    GameObject.Destroy(obj);
                }
                list.Clear();
            }
            mPool.Clear();
            mPrefabs.Clear();
        }


        public void InitRoot(Transform root)
        {
            mRoot = root;
        }


        public T Pop(string fileName, Transform attachTr)
        {
            GameObject prefab = getPrefab(fileName);
            if (prefab == null)
            {
                return null;
            }
            return Pop(prefab, attachTr, prefab.transform.localPosition);
        }

        public T Pop(string fileName, Transform attachTr, Vector3 localPos)
        {
            GameObject prefab = getPrefab(fileName);
            if (prefab == null)
            {
                return null;
            }
            return Pop(prefab, attachTr, localPos);
        }

        public T Pop(GameObject prefab, Transform attachTr, Vector3 localPos)
        {
            T compo;
            List<T> list;
            string name = prefab.name;
            if (mPool.TryGetValue(name, out list) == false)
            {
                list = new List<T>();
                mPool.Add(name, list);
            }

            if (list.Count > 0)
            {
                compo = list[0];
                list.RemoveAt(0);
            }
            else
            {
                // Instantiate
                GameObject obj = GameObject.Instantiate<GameObject>(prefab, localPos, Quaternion.identity, attachTr);
                compo = obj.GetComponent<T>();
                if (compo == null)
                {
                    Debug.LogErrorFormat("[SimplePool<{0}>::Pop()] type={0}", typeof(T));
                    GameObject.Destroy(obj);
                    return null;
                }
            }

            compo.transform.SetParent(attachTr);
            compo.transform.localPosition = localPos;
            compo.transform.localRotation = prefab.transform.localRotation;
            compo.transform.localScale = prefab.transform.localScale;
            compo.gameObject.name = prefab.name;
            compo.gameObject.SetActive(true);

            compo.OnPoolEvent_Pop();
            return compo;
        }


        public void Push(T _obj)
        {
            if (_obj == null)
            {
                Debug.LogErrorFormat("[SimplePool::Push] Pooling object is null: type={0}", typeof(T));
                return;
            }
            _obj.transform.SetParent(mRoot);
            _obj.OnPoolEvent_Push();

            List<T> list;
            if (mPool.TryGetValue(_obj.name, out list) == false)
            {
                list = new List<T>();
                mPool.Add(_obj.name, list);
            }
            _obj.gameObject.SetActive(false);
            list.Add(_obj);
        }


        GameObject getPrefab(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            GameObject prefab;
            if (mPrefabs.TryGetValue(name, out prefab) == false)
            {
                prefab = AssetManager.Instance.GetAsset<GameObject>(fileName);
                if (prefab == null)
                {
                    Debug.LogErrorFormat("[SimplePool::Pop] Cannot find prefab: path={0}", fileName);
                    return null;
                }
                mPrefabs.Add(name, prefab);
            }
            return prefab;
        }
    }

}
