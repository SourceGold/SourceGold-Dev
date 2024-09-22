namespace Assets.Script.Loggers
{
    public class InfoLogger
    {
        public void LogInfo(string message, bool enableConsoleLogging = true)
        {
            GameEventLogger.LogDebugEvent(message, EventLogType.InfoEventLogType, enableConsoleLogging);
        }
    }
}
