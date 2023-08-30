using System;

namespace Assets.Script.Backend
{
    public class EventLogMessage
    {
        public string Message;
        public DateTime DateTime;

        public EventLogMessage(string message)
        {
            Message = message;
            DateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
