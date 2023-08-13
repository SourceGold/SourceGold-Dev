using System.Collections.Concurrent;

namespace Assets.Script.Backend
{
    public class GameEventLogger
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

        public static void LogEvent(string message)
        {
            Instance.EventLog.Enqueue(new EventLogMessage(message));
        }
    }
}
