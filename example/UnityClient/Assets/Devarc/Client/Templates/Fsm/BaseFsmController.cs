using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Devarc
{
    public abstract class BaseFsmData
    {
    }

    public abstract class BaseFsmController<OWNER, FSM, DATA> : BaseController<OWNER>
        where OWNER : BaseObject 
        where FSM : BaseFsmObject<OWNER, DATA>
        where DATA : BaseFsmData
    {
        public abstract FSM Get(DATA data);

        FSM mCurrentState = null;
        List<DATA> mChangingStates = new List<DATA>();
        bool mIsChanging = false;

        public override void Clear()
        {
            mCurrentState = null;
            mIsChanging = false;
        }

        public void ChangeFSM(DATA data)
        {
            mChangingStates.Add(data);
            updateFSM();
        }

        void updateFSM()
        {
            if (mIsChanging)
                return;

            mIsChanging = true;
            while (mChangingStates.Count > 0)
            {
                int i = mChangingStates.Count - 1;
                var data = mChangingStates[i];
                mChangingStates.RemoveAt(i);

                FSM fsm = Get(data);
                if (fsm == null)
                {
                    Debug.LogError($"Cannot find FSM: key={data.ToString()}");
                    continue;
                }

                if (mCurrentState != null)
                {
                    mCurrentState.Exit();
                }
                mCurrentState = fsm;
                mCurrentState.Enter(data);
            }
            mIsChanging = false;
        }

    }
}
