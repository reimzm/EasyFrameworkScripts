using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace zm
{
    public delegate void EventManagerAction(object obj);

    public class Eventdata
    {
        public int MyType;

        public EventManagerAction unityAction;
    }

    public class EventManager : SingleBase<EventManager>
    {
        private Dictionary<int, List<Eventdata>> EventDictionary = new Dictionary<int, List<Eventdata>>();

        public void RegisterEvent(int eventType, EventManagerAction unityAction)
        {
            if (!EventDictionary.ContainsKey(eventType))
            {
                EventDictionary.Add(eventType, new List<Eventdata>());
            }

            Eventdata eventdata = new Eventdata()
            {
                MyType = eventType,
                unityAction = unityAction
            };

            EventDictionary[eventType].Add(eventdata);
        }

        public void UnRegisterEvent(int eventType, EventManagerAction eventManagerAction)
        {
            if (EventDictionary.ContainsKey(eventType))
            {
                foreach (var action in EventDictionary[eventType])
                {
                    if (action.unityAction == eventManagerAction)
                    {
                        var temp = action;
                        EventDictionary[eventType].Remove(temp);
                        break;
                    }
                }
            }
            else
                Debug.Log(string.Format("<color=red>没有注册事件{0}</color>", eventType.ToString()));
        }

        public void InvokeEvent(int eventType,object obj)
        {
            if(EventDictionary.ContainsKey(eventType))
            {
                foreach(var action in EventDictionary[eventType])
                {
                    action.unityAction.Invoke(obj);
                }
            }
            else
                Debug.Log(string.Format("<color=red>没有注册事件{0}</color>", eventType.ToString()));
        }
    }

}