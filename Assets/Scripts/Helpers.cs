using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class Helpers
    {
        public static Vector2 convBlockID_XY(int blockID, Vector2 levelSize)
        {
            return new Vector2(Mathf.FloorToInt(blockID / levelSize.y), Mathf.FloorToInt(blockID % levelSize.y));
        }

        public static int convXY_BlockID(Vector2 coords, Vector2 levelSize)
        {
            return (int)coords.y + ((int)coords.x * (int)levelSize.y);
        }

        public static Vector2 CalcBlockDistance(int fromID, int toID, Vector2 levelSize)
        {
            return convBlockID_XY(toID, levelSize) - convBlockID_XY(fromID, levelSize);
        }

        public static void AddEventTrigger(EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType)
        {
            // Create a nee TriggerEvent and add a listener
            EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
            trigger.AddListener((eventData) => action()); // you can capture and pass the event data to the listener

            // Create and initialise EventTrigger.Entry using the created TriggerEvent
            EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

            // Add the EventTrigger.Entry to delegates list on the EventTrigger
            eventTrigger.triggers.Add(entry);
        }
    }
}
