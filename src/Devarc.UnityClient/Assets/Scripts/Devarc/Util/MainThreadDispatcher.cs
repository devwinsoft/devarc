using System;
using System.Collections.Generic;
using System.Threading;


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


        public void AddWork(Action<object[]> work, params object[] args)
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

        public void DoWork()
        {
            while (mDelayedActions.Count > 0)
            {
                var delayedAction = mDelayedActions.Dequeue();
                delayedAction.action.Invoke(delayedAction.args);
            }
        }
    }
}

