using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Collectable : MonoBehaviour
    {
        public float healthIncrement = 0;
        public float intervalCacheLoop = .5f;
        public bool isLoopOwner = false;

        public static List<Collectable> allCollectables {
            get
            {
                return _allCollectableCache;
            }
        }

        protected static List<Collectable> _allCollectableCache = new List<Collectable>();

        private static bool startedCacheLoop = false;
        private static GameObject loopOwner = null;

        void Start()
        {
            if (!startedCacheLoop && isLoopOwner)
            {
                
                StartSingletonLoop();
            }
        }
        void StartSingletonLoop()
        {
            startedCacheLoop = true; //static
            loopOwner = this.gameObject; //static
            StartCoroutine(CacheCollectables());
            isLoopOwner = true;
        }
        public void Consume()
        {
            if (loopOwner != gameObject)
                GameObject.Destroy(gameObject);
        }
        IEnumerator CacheCollectables()
        {
            while (true)
            {
                _allCollectableCache = new List<Collectable>(GameObject.FindObjectsOfType<Collectable>());
                yield return new WaitForSeconds(intervalCacheLoop);
            }
        }
    }

}