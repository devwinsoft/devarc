using System;
using System.Collections.Generic;

namespace Devarc
{
    public class BaseTrigger<KEY> where KEY : struct, IConvertible
    {
        public delegate bool FUNCTION(System.Object[] args);
        public delegate void FUNCTION_TIMER(System.Object[] args);

        public class TRIGGER
        {
            public int instanceID;
            public KEY Key;
            public FUNCTION Callback;

            public TRIGGER(int _instanceID, KEY _key, FUNCTION _callback)
            {
                instanceID = _instanceID;
                Key = _key;
                Callback = _callback;
            }
        }
        List<TRIGGER> mTriggers = new List<TRIGGER>();

        public class TRIGGER_TIMER
        {
            public int instanceID;
            public float Timeout;
            public FUNCTION_TIMER Callback;
            public object[] Args;

            float mElapsedTime = 0f;

            public TRIGGER_TIMER(int _instanceID, float _timeout, FUNCTION_TIMER _callback, params object[] _args)
            {
                instanceID = _instanceID;
                Timeout = _timeout;
                Callback = _callback;
                Args = _args;

                mElapsedTime = 0f;
            }

            public bool Tick(float deltaTime)
            {
                mElapsedTime += deltaTime;
                if (mElapsedTime >= Timeout)
                {
                    mElapsedTime -= Timeout;
                    return true;
                }
                return false;
            }
        }
        List<TRIGGER_TIMER> mTimers = new List<TRIGGER_TIMER>();


        public void ClearAll()
        {
            mTimers.Clear();
        }

        public void Clear(int _instanceID)
        {
            for (int i = mTriggers.Count - 1; i >= 0; i--)
            {
                var data = mTriggers[i];
                if (data.instanceID == _instanceID)
                    mTriggers.RemoveAt(i);
            }
        }


        public void Register(int _instanceID, KEY _key, FUNCTION _callback)
        {
            mTriggers.Add(new TRIGGER(_instanceID, _key, _callback));
        }

        public void Notify(KEY _key, params System.Object[] args)
        {
            for (int i = mTriggers.Count - 1; i >= 0; i--)
            {
                if (i >= mTriggers.Count)
                {
                    continue;
                }

                TRIGGER data = mTriggers[i];
                if (string.Equals(_key, data.Key) == false)
                {
                    continue;
                }
                bool result = data.Callback(args);
                if (result && i < mTriggers.Count)
                {
                    mTriggers.RemoveAt(i);
                }
            }
        }


        public void RegisterTimer(int _instanceID, float _timeout, FUNCTION_TIMER _callback, params object[] args)
        {
            mTimers.Add(new TRIGGER_TIMER(_instanceID, _timeout, _callback, args));
        }

        public void Tick(float deltaTime)
        {
            for (int i = mTimers.Count - 1; i >= 0; i--)
            {
                if (i >= mTimers.Count)
                {
                    continue;
                }

                var data = mTimers[i];
                if (data.Tick(deltaTime) == false)
                {
                    continue;
                }
                data.Callback(data.Args);
                if (i < mTimers.Count)
                {
                    mTimers.RemoveAt(i);
                }
            }
        }
    }
}

