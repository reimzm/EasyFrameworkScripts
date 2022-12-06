using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public class SingleBase<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    instance = obj.AddComponent<T>();
                }
                return instance;
            }
        }



        public virtual void Init()
        {
        }
    }
}
