using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public abstract class SimpleFsmController<STATE, FSM, OWNER> : MonoBehaviour
        where STATE : struct, IConvertible
        where FSM : SimpleFsmObject<STATE, OWNER>
        where OWNER : MonoBehaviour 
    {
        protected abstract void onInit();

        public OWNER Owner => mOwner;
        OWNER mOwner = null;

        public STATE CurrentStateType => mCurrentState != null ? mCurrentState.State : default(STATE);
        public FSM CurrentState => mCurrentState;
        FSM mCurrentState = null;
        Dictionary<STATE, FSM> mStates = new Dictionary<STATE, FSM>();

        class TRANS
        {
            public STATE state;
            public object[] args;
        }
        List<TRANS> mChangingStates = new List<TRANS>();
        bool mIsChanging = false;

        void LateUpdate()
        {
            mCurrentState?.Tick();
        }

        public virtual void Clear()
        {
            mCurrentState = null;
            mIsChanging = false;
        }

        public void Init(OWNER owner)
        {
            mOwner = owner;
            onInit();
        }

        public FSM Get(STATE state)
        {
            FSM obj = null;
            mStates.TryGetValue(state, out obj);
            return obj;
        }

        public void ChangeState(STATE state, params object[] args)
        {
            TRANS data = new TRANS();
            data.state = state;
            data.args = args;
            mChangingStates.Add(data);
            updateState();
        }

        protected void registerState(FSM state)
        {
            if (mStates.TryAdd(state.State, state))
            {
                state.Init(mOwner);
            }
        }

        void updateState()
        {
            if (mIsChanging)
                return;

            mIsChanging = true;
            while (mChangingStates.Count > 0)
            {
                int i = mChangingStates.Count - 1;
                var data = mChangingStates[i];
                mChangingStates.RemoveAt(i);

                FSM fsm = Get(data.state);
                if (fsm == null)
                {
                    Debug.LogError($"Cannot find FSM: key={data.state}");
                    continue;
                }

                if (mCurrentState != null)
                {
                    mCurrentState.Exit();
                }
                mCurrentState = fsm;
                mCurrentState.Enter(data.args);
            }
            mIsChanging = false;
        }

    }
}
