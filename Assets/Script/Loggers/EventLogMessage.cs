using System;

namespace Assets.Script.Loggers
{
    public class EventLogMessage
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public EventLogLevel Level { get; set; }
        public string LogType { get; set; }

        public EventLogMessage() { }

        public EventLogMessage(string message, EventLogLevel eventLogLevel, string eventLogType)
        {
            Message = message;
            DateTime = DateTime.Now;
            Level = eventLogLevel;
            LogType = eventLogType;
        }

        public override string ToString()
        {
            return $"Event {nameof(Level)}: {Level}, Log Type {LogType}, Event Time: {DateTime}, Event {Message}: {Message}";
        }
    }
}
