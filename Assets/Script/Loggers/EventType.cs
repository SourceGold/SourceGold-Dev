namespace Assets.Script.Loggers
{
    public enum EventLogLevel
    {
        _,
        SystemEvent,
        UserEvent,
        GameEvent,
        DebugEvent,
    }

    public static class EventLogType
    {
        public const string InfoEventLogType = "Information";

        public const string GameErrorEventLogType = "GameError";
        public const string UserErrorEventLogType = "UserError";
        public const string WarningEventLogType = "Warning";
    }
}
