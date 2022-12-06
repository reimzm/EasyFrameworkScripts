using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public static class InstanceCreationTool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="only">是否只能创建一个</param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T CreactObject<T>(Transform parent = null, GameObject obj = null, bool only = true) where T : MonoBehaviour
        {
            if (only)
            {
                var tobj = GameObject.FindObjectOfType<T>();
                if (tobj != null)
                {
                    return tobj;
                }
            }

            if (obj == null)
            {
                obj = new GameObject();
                obj.name = typeof(T).ToString();
                obj.transform.parent = parent;
            }
            var t = obj.AddComponent<T>();
            return t;
        }



        public static void AddDontDestryOnload(this MonoBehaviour monoBehaviour)
        {
            var ddols = monoBehaviour.gameObject.GetComponent<DontDestroyOnLoadScript>();
            if (ddols == null)
                monoBehaviour.gameObject.AddComponent<DontDestroyOnLoadScript>();
        }
    }
}


