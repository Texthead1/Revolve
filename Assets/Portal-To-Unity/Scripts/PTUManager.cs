using System;
using UnityEngine;

namespace PortalToUnity
{
    public enum LogPriority
    {
        High,
        Normal,
        Low,
        Debug
    }

    public class PTUManager : MonoBehaviour
    {
        [SerializeField] private LoggingLevel _loggingLevel;

        public static PTUManager Instance;
        private static LoggingLevel LogLevel => Instance != null ? Instance._loggingLevel : LoggingLevel.None;

        private enum LoggingLevel
        {
            None = -1,
            Strict,
            Regular,
            Verbose,
            Debug
        }

        void Awake()
        {
            Instance = this;
            SkylanderDatabase.Initialize();
            PortalDatabase.Initialize();
            VillainDatabase.Initialize();
            Cryptography.CheckForSalt();
        }

        async void Start()
        {
            await DigitalPortalManager.Initialize();
        }

        public static bool Log(string message, LogPriority logPriority)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;
            Debug.Log(message);
            return true;
        }

        public static bool LogWarning(string message, LogPriority logPriority)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;
            Debug.LogWarning(message);
            return true;
        }

        public static bool LogError(string message, LogPriority logPriority)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;
            Debug.LogError(message);
            return true;
        }

        public static bool LogException(Exception exception, LogPriority logPriority)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;
            Debug.LogException(exception);
            return true;
        }

        public static bool LogMulti(LogPriority logPriority, params string[] messages)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;

            foreach (string message in messages)
                Debug.Log(message);

            return true;
        }

        public static bool LogWarningMulti(LogPriority logPriority, params string[] messages)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;

            foreach (string message in messages)
                Debug.LogWarning(message);

            return true;
        }

        public static bool LogErrorMulti(LogPriority logPriority, params string[] messages)
        {
            if (LogLevel < (LoggingLevel)logPriority)
                return false;

            foreach (string message in messages)
                Debug.LogError(message);

            return true;
        }
    }
}