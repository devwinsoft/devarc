using System;

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

        public static event Action<LogTypeEx, string> OnLogMessage;

        public static void Dispatch()
        {
            sDispatcher.MainThreadTick();
        }

        public static void LogError(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Error, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Error, (string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogErrorFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Error, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Error, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }



        public static void LogAssert(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Assert, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Assert, (string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogAssertFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Assert, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Assert, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }



        public static void LogWarning(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Warning, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Warning, (string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogWarningFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Warning, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Warning, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }



        public static void Log(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Log, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Log, (string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Log, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Log, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }


        public static void LogException(object message)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Exception, message.ToString());
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Exception, (string)tempArgs[0]);
                }), message);
            }
        }

        public static void LogExceptionFormat(string message, params object[] args)
        {
            if (sDispatcher.IsMainThread)
            {
                OnLogMessage?.Invoke(LogTypeEx.Exception, string.Format(message, args));
            }
            else
            {
                sDispatcher.Invoke((Action<object[]>)((tempArgs) =>
                {
                    OnLogMessage?.Invoke(LogTypeEx.Exception, string.Format((string)tempArgs[0], (object[])tempArgs[1]));
                }), message, args);
            }
        }
    }
}
