namespace Assets.Script.Loggers
{
    public class ErrorLogger
    {
        public void LogGameError(string message, bool enableConsoleLogging = true)
        {
            GameEventLogger.LogDebugEvent(message, EventLogType.GameErrorEventLogType, enableConsoleLogging);
        }

        public void LogUserError(string message, bool enableConsoleLogging = true)
        {
            GameEventLogger.LogDebugEvent(message, EventLogType.UserErrorEventLogType, enableConsoleLogging);
        }

        public void LogGameWarning(string message, bool enableConsoleLogging = true)
        {
            GameEventLogger.LogDebugEvent(message, EventLogType.WarningEventLogType, enableConsoleLogging);
        }
    }
}
