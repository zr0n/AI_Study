using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{

    public class Spawner : MonoBehaviour
    {
        [SerializeField] GameObject template;
        [SerializeField] float intervalMin = 1f;
        [SerializeField] float intervalMax = 0f;
        [SerializeField] bool startSpawning = false;
        [SerializeField] string name = "Spawner";
        [SerializeField] Vector3 MaxDistanceFromOrigin = new Vector3(30f, 30f, 30f);
        [SerializeField] Vector3 MinDistanceFromOrigin = Vector3.zero;


        static int instancesCount = 0;
        int id = 0;
        // Start is called before the first frame update
        void Start()
        {
            instancesCount++;
            id = instancesCount;
            StartCoroutine(SpawnLoop());
        }

        
        IEnumerator SpawnLoop()
        {
            if (startSpawning)
                Spawn();

            while (true)
            {
                yield return new WaitForSeconds(Random.Range(intervalMin, intervalMax));
                Spawn();
            }
        }
        void Spawn()
        {
            if (!template)
                Debug.LogError("No template assigned to " + FullName());
            else
            {
                Debug.Log(FullName() + ": spawning a " + template.name);
                GameObject obj = GameObject.Instantiate(template);
                obj.transform.position = RandomPosition();
            }

        }
        string FullName()
        {
            return name + "-" + id;
        }
        Vector3 RandomPosition()
        {
            Vector3 offset = new Vector3(
                Random.Range(MinDistanceFromOrigin.x, MaxDistanceFromOrigin.x),
                Random.Range(MinDistanceFromOrigin.y, MaxDistanceFromOrigin.y),
                Random.Range(MinDistanceFromOrigin.z, MaxDistanceFromOrigin.z)
            );

            return transform.position + offset;
        }
    }

}