using UnityEngine;

namespace GameWorkstore.Patterns
{
    public enum DebugLevel
    {
        ERROR = 0,
        WARNING = 1,
        INFO = 2,
    }

    public static class DebugMessege
    {
        private static DebugLevel CurrentLogLevel = DebugLevel.ERROR;

        public static void SetLogLevel(DebugLevel level)
        {
            CurrentLogLevel = level;
        }

        //[Conditional("UNITY_EDITOR")]
        public static void Log(string msg, DebugLevel level)
        {
            if(CurrentLogLevel >= level)
            {
                switch (level)
                {
                    case DebugLevel.ERROR: Debug.LogError(msg); break;
                    case DebugLevel.WARNING: Debug.LogWarning(msg); break;
                    case DebugLevel.INFO: Debug.Log(msg); break;
                }
            }
        }
    }
}
