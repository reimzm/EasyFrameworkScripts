using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public class LoadResourceManager : SingleBase<LoadResourceManager>
    {
        private Canvas canvas;

        public void Init(string canvasName = "Canvas")
        {
            base.Init();
            var obj = GameObject.Find(canvasName);
            if (obj != null)
            {
                canvas = obj.GetComponent<Canvas>();
            }
        }

        public T InstantiateResource<T>(string path, Transform parent = null) where T : Component
        {
            var obj = InstantiateResource(path, parent);
            var t = obj.AddComponent<T>();
            return t;
        }

        public T InstantiateResourceToCanvas<T>(string path, Transform parent = null) where T : Component
        {
            if (parent == null) parent = canvas.transform;
            var obj = InstantiateResource(path, parent);
            var t = obj.AddComponent<T>();
            return t;
        }

        public GameObject InstantiateResourceToCanvas(string path, Transform parent = null)
        {
            if (parent == null) parent = canvas.transform;
            return InstantiateResource(path, parent);
        }

        public GameObject InstantiateResource(string path, Transform parent = null)
        {
            var res = Resources.Load<GameObject>(path);
            var obj = Instantiate(res, parent);
            return obj;
        }
    }

}