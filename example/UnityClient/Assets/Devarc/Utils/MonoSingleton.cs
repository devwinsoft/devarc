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
using UnityEngine;

namespace Devarc
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    mInstance = obj.AddComponent<T>();
                    return mInstance;
                }
                return mInstance;
            }
        }

        public static bool IsCreated() => mInstance != null;

        static T mInstance;
        protected Transform mTransform;

        public static T Create()
        {
            if (IsCreated())
            {
                return mInstance;
            }
            GameObject obj = new GameObject(typeof(T).Name);
            mInstance = obj.AddComponent<T>();
            return mInstance;
        }

        public static T Create(string resourcePath)
        {
            if (IsCreated())
            {
                return mInstance;
            }

            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                Debug.LogError($"[{typeof(T).Name}::Create] Cannot find prefab: resourcePath={resourcePath}");
                return null;
            }
            var obj = Instantiate(prefab);
            var compo = obj.GetComponent<T>();
            return compo;
        }

        public static void Destroy()
        {
            if (IsCreated() == false)
                return;
            Destroy(mInstance.gameObject);
        }


        protected virtual void onAwake() { }
        protected virtual void onStart() { }
        protected virtual void onUpdate() { }
        protected virtual void onLateUpdate() { }
        protected virtual void onDestroy() { }


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            mInstance = this as T;
            mTransform = transform;
            onAwake();
        }

        private void OnDestroy()
        {
            onDestroy();
            if (mInstance == this)
            {
                mInstance = null;
            }
        }

        private void Start()
        {
            onStart();
        }

        private void Update()
        {
            onUpdate();
        }

        private void LateUpdate()
        {
            onLateUpdate();
        }
    }

}