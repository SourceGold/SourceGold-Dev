using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Backend
{
    public class EventLogger
    {
        protected ConcurrentQueue<EventLogMessage> EventLog{ get; set; }

        public EventLogger()
        {
            EventLog = new ConcurrentQueue<EventLogMessage>();
        }

        public void LogEvent(string message)
        {
            EventLog.Enqueue(new EventLogMessage(message));
        }
    }
}
