using System;
using System.Collections.Generic;
using System.Threading;

namespace Devarc
{
    public class MainThreadDispatcher
    {
        private static readonly Thread mainThread = Thread.CurrentThread;
        private readonly Queue<DelayedAction> mDelayedActions = new();
        public bool IsMainThread => Thread.CurrentThread == mainThread;


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

        private class DelayedAction
        {
            public Action<object[]> action;
            public object[] args;
        }
    }
}