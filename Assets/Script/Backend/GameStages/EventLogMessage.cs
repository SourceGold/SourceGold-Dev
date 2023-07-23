namespace Assets.Script.Backend
{
    public class EventLogMessage
    {
        public string Message;

        public EventLogMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
