using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class ObjectPool
    {
        private static Dictionary<System.Type, List<GameObject>> pools = new Dictionary<System.Type, List<GameObject>>(); 


        public static T Spawn<T>(GameObject template) where T: MonoBehaviour{
            GameObject go = GameObject.Instantiate(template);
            return go.GetComponent<T>();
        }

        public static T GetFromPool<T>(GameObject template) where T: MonoBehaviour 
        {
            if (!pools.ContainsKey(typeof(T)))
                pools[typeof(T)] = new List<GameObject>();

            List<GameObject> pool = pools[typeof(T)];

            GameObject result = null;
            if(pool.Count > 0)
            {
                result = pool[0];
                result.SetActive(true);
                pool.RemoveAt(0);
            }
            else
            {
                result = Spawn<T>(template).gameObject;
            }
            return result ? result.GetComponent<T>() : null;
        }

        public static void AddToPool<T>(T objectToRecycle) where T: MonoBehaviour
        {
            if (!pools.ContainsKey(typeof(T)))
                pools[typeof(T)] = new List<GameObject>();

            pools[typeof(T)].Add(objectToRecycle.gameObject);
            objectToRecycle.gameObject.SetActive(false);
        }

        public static void DebugPool(string message = "")
        {
            if (message.Length > 0)
                Debug.Log(message);
            foreach(System.Type key in pools.Keys)
            {
                Debug.Log("Key " + key.Name);

                foreach(GameObject go in pools[key])
                {
                    Debug.Log(go.name);
                }

            }
        }
    }

}