using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class BaseTrigger<KEY>
    {
        public delegate bool FUNCTION(System.Object[] args);

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

        public void ClearAll()
        {
            mTriggers.Clear();
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
    }
}

