using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public class GameEventLogger : DataPersistence
    {
        protected ConcurrentQueue<EventLogMessage> EventLog { get; set; }

        private static GameEventLogger _instance;

        public static GameEventLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameEventLogger();
                }

                return _instance;
            }
        }

        public GameEventLogger()
        {
            EventLog = new ConcurrentQueue<EventLogMessage>();
        }

        public static void LogEvent(string message, EventLogType eventLogType = EventLogType.UserEvent)
        {
            Instance.EventLog.Enqueue(new EventLogMessage(message, eventLogType));
        }

        public override void LoadData(string fileName)
        {
            var logs = DataPersistenceManager.LoadDataFile<List<EventLogMessage>>(fileName);
            var newQueue = new ConcurrentQueue<EventLogMessage>();
            foreach (var log in logs)
            {
                if (log.Type == EventLogType.UserEvent)
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
