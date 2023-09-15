using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Devarc
{
    public class MainThreadDispatcher
    {
        public bool IsMainThread => Thread.CurrentThread == mainThread;
        static Thread mainThread = Thread.CurrentThread;

        class DelayedAction
        {
            public Action<object[]> action;
            public object[] args;
        }
        Queue<DelayedAction> mDelayedActions = new Queue<DelayedAction>();


        public void Invoke(Action<object[]> work, params object[] args)
        {
            if (IsMainThread)
            {
                work.Invoke(args);
            }
            else
            {
                var delayedAction = new DelayedAction();
                delayedAction.action = work;
                delayedAction.args = args;
                mDelayedActions.Enqueue(delayedAction);
            }
        }


        public void MainThreadTick()
        {
            while (mDelayedActions.Count > 0)
            {
                var delayedAction = mDelayedActions.Dequeue();
                delayedAction.action.Invoke(delayedAction.args);
            }
        }
    }
}

