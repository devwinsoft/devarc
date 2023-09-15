using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public enum LogTypeEx
    {
        Error,
        Assert,
        Warning,
        Log,
        Exception
    }

    public class Debugging
    {
        static MainThreadDispatcher sDispatcher = new MainThreadDispatcher();

        public static event Action<string> OnLog;
        public static event Action<object> OnLogWarning;
        public static event Action<object> OnLogError;
        public static event Action<bool, string> OnAssert;
        public static event Action<Exception> OnLogException;

        public static void Dispatch()
        {
            sDispatcher.MainThreadTick();
        }

        public static void LogError(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogError?.Invoke(message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogError?.Invoke((string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogErrorFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogError?.Invoke(string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogError?.Invoke(string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }



        public static void LogAssert(bool condition, string message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnAssert?.Invoke(condition, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnAssert?.Invoke(condition, (string)tempArgs[0]);
                }), condition, message);
            }
        }

        public static void LogAssertFormat(bool condition, string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnAssert?.Invoke(condition, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnAssert?.Invoke(condition, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), condition, message, args);
            }
        }



        public static void LogWarning(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogWarning?.Invoke(message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogWarning?.Invoke((string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogWarningFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogWarning?.Invoke(string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogWarning?.Invoke(string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }



        public static void Log(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLog?.Invoke(message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLog?.Invoke((string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLog?.Invoke(string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLog?.Invoke(string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }


        public static void LogException(Exception ex)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogException?.Invoke(ex);
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogException?.Invoke((Exception)tempArgs[0]);
                }), ex);
            }
        }

    }
}
