using System;

namespace Assets.Script.Backend
{
    public class EventLogMessage
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public EventLogType Type { get; set; }

        public EventLogMessage() { }

        public EventLogMessage(string message, EventLogType eventLogType)
        {
            Message = message;
            DateTime = DateTime.Now;
            Type = eventLogType;
        }

        public override string ToString()
        {
            return $"Event Type: {Type}, Event Time: {DateTime}, Event Message: {Message}";
        }
    }
}
