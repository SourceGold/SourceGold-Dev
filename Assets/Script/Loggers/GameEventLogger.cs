using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Loggers
{
    public class GameEventLogger : DataPersistence
    {
        // this need to be public to do default serilize before we found a solution
        // commented code are for using default serilize option
        //public List<EventLogMessage> EventLog { get; private set; }

        public ConcurrentQueue<EventLogMessage> EventLog { get; private set; }

        private static GameEventLogger _instance;

        public static GameEventLogger Instance
        {
            get
            {
                return _instance ?? (_instance = new GameEventLogger(true));
            }
        }

        public GameEventLogger()
        {
            EventLog = new ConcurrentQueue<EventLogMessage>();
            //EventLog = new List<EventLogMessage>();
        }

        public GameEventLogger(bool autoRegister) : base(autoRegister)
        {
            EventLog = new ConcurrentQueue<EventLogMessage>();
            //EventLog = new List<EventLogMessage>();
        }

        public static void LogEvent(string message, EventLogLevel eventLogLevel, string eventLogType = "", bool enableConsoleLogging = false)
        {
            Instance.EventLog.Enqueue(new EventLogMessage(message, eventLogLevel, eventLogType));
            if (enableConsoleLogging )
            {
                switch (eventLogType)
                {
                    case EventLogType.WarningEventLogType: 
                        Debug.LogWarning(message); 
                        break;
                    case EventLogType.UserErrorEventLogType:
                    case EventLogType.GameErrorEventLogType:
                        Debug.LogError(message); 
                        break;
                    case EventLogType.InfoEventLogType:
                    default:
                        Debug.Log(message);
                        break;
                }
            }
            //Instance.EventLog.Add(new EventLogMessage(message, eventLogType));
        }

        public static void LogDebugEvent(string message, string eventLogType = "", bool enableConsoleLogging = false)
        {
            LogEvent(message, EventLogLevel.DebugEvent, eventLogType, enableConsoleLogging);
        }

        public override void Restart()
        {
            LogEvent("Scene Restarted", EventLogLevel.GameEvent);
        }

        public override void LoadData(string fileName)
        {
            var logs = DataPersistenceManager.LoadDataFile<List<EventLogMessage>>(fileName);
            var newQueue = new ConcurrentQueue<EventLogMessage>();
            foreach (var log in logs)
            {
                if (log.Level == EventLogLevel.UserEvent)
                {
                    newQueue.Enqueue(log);
                }
            }
            while (EventLog.TryDequeue(out var log))
            {
                newQueue.Enqueue(log);
            }
            EventLog = newQueue;
        }

        public override void SaveData(string fileName)
        {
            DataPersistenceManager.SaveDataFile(fileName, EventLog);
        }

        public override string GetSaveFileName()
        {
            return nameof(EventLog);
        }
    }
}
