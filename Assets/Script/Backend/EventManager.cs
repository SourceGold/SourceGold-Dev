using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Backend
{
    public class EventManager : MonoBehaviour
    {

        private Dictionary<string, UnityEvent> eventDictionary;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, UnityEvent>();
            }
        }

        public static void StartListening(GameEventType eventType, UnityAction listener)
        {
             if (TryGetEvent(eventType, out UnityEvent thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                instance.eventDictionary.Add(eventType.ToString(), thisEvent);
            }
            GameEventLogger.LogEvent($"Listener: {listener} started listening event: {eventType}", EventLogType.GameEvent);
        }

        public static void StopListening(GameEventType eventType, UnityAction listener)
        {
            if (eventManager == null) return;
            if (TryGetEvent(eventType, out UnityEvent thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
            GameEventLogger.LogEvent($"Listener: {listener} stopped listening event: {eventType}", EventLogType.GameEvent);
        }

        public static void TriggerEvent(GameEventType eventType)
        {
            GameEventLogger.LogEvent($"Event Triggered: {eventType}");
            if (TryGetEvent(eventType, out UnityEvent thisEvent))
            {
                thisEvent.Invoke();
            }
        }

        private static bool TryGetEvent(GameEventType eventType, out UnityEvent thisEvent)
        {
            return instance.eventDictionary.TryGetValue(eventType.ToString(), out thisEvent);
        }
    }
}
