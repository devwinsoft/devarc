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
    public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get { return mInstance; }
        }
        static T mInstance;

        public static T Create(Transform _parent)
        {
            GameObject obj = new GameObject(typeof(T).Name);
            obj.transform.parent = _parent;
            return obj.AddComponent<T>();
        }

        public static void Destroy()
        {
            if (mInstance != null)
            {
                GameObject.Destroy(mInstance.gameObject);
            }
        }

        protected abstract void onAwake();
        protected virtual void onStart() { }
        protected virtual void onUpdate() { }
        protected virtual void onLateUpdate() { }
        protected abstract void onDestroy();


        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            mInstance = this as T;
            onAwake();
        }
        private void OnDestroy()
        {
            onDestroy();
            mInstance = null;
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