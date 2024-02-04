using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public class GameEventLogger : DataPersistence
    {
        // this need to be public to do default serilize before we found a solution
        // commented code are for using default serilize option
        //public List<EventLogMessage> EventLog { get; private set; }
        
        public ConcurrentQueue<EventLogMessage> EventLog { get; private set; }

        public  PlayerSaveInfo  PlayerStats = null;

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

        public static void LogEvent(string message, EventLogType eventLogType = EventLogType.UserEvent)
        {
            Instance.EventLog.Enqueue(new EventLogMessage(message, eventLogType));
            //Instance.EventLog.Add(new EventLogMessage(message, eventLogType));
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
