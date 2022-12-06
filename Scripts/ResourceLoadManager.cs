using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public class ResourceLoadManager : SingleBase<ResourceLoadManager>
    {

        private Canvas canvas;

        public Dictionary<string, object[]> LoadResourceDictionary = new Dictionary<string, object[]>();

        public void Init(string canvasName = "Canvas")
        {
            base.Init();
            var obj = GameObject.Find(canvasName);
            if (obj != null)
            {
                canvas = obj.GetComponent<Canvas>();
            }
        }

        public void LoadResource<T>(params string[] spritesResourcePaths) where T : UnityEngine.Object
        {
            foreach (var info in spritesResourcePaths)
            {
                var sprites = Resources.LoadAll<T>(info);
                LoadResourceDictionary.Add(info, sprites);
            }
        }

        public T GetLoadResource<T>(string spritesResourcePaths, string spriteName) where T : class
        {
            foreach (var sprite in LoadResourceDictionary[spritesResourcePaths])
            {
                var spritename = sprite.ToString().Replace(" (" + typeof(T).ToString() + ")", "");
                if (spritename == spriteName)
                {
                    T t = sprite as T;
                    return t;
                }
            }
            return default(T);
        }

        public T InstantiateResource<T>(string path, Transform parent = null) where T : Component
        {
            var obj = InstantiateResourceObject(path, parent);
            var t = obj.AddComponent<T>();
            return t;
        }

        public T InstantiateResourceToCanvas<T>(string path, Transform parent = null) where T : Component
        {
            if (parent == null) parent = canvas.transform;
            var obj = InstantiateResourceObject(path, parent);
            var t = obj.AddComponent<T>();
            return t;
        }

        public GameObject InstantiateResourceObjectToCanvas(string path, Transform parent = null)
        {
            if (parent == null) parent = canvas.transform;
            return InstantiateResourceObject(path, parent);
        }

        public GameObject InstantiateResourceObject(string path, Transform parent = null)
        {
            var res = Resources.Load<GameObject>(path);
            var obj = Instantiate(res, parent);
            return obj;
        }
    }

}