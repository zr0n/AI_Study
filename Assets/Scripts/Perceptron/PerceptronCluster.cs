using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{

    public class PerceptronCluster : MonoBehaviour
    {
        public bool randomizeInputs = true;
        public int perceptronsNum = 100;
        public float[] inputs = { 0f, 0f };
        public GameObject pointTemplate;
        public LineRenderer line;
        public bool autoTick = false;
        public int applicationFrameRate = 60;

        public List<Point> points = new List<Point>();
        public List<Perceptron> perceptrons = new List<Perceptron>();
        // Start is called before the first frame update
        void Start()
        {
            if (randomizeInputs)
                RandomizeInputs();


            SpawnPerceptrons();
            DrawLine();
            Application.targetFrameRate = applicationFrameRate;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) || autoTick)
                Tick();
        }
        void DrawLine()
        {
            if (line)
            {
                Vector3 lineStart = new Vector3(0f, 0f, Camera.main.nearClipPlane);
                Vector3 lineEnd = new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane);

                lineStart = Camera.main.ScreenToWorldPoint(lineStart);
                lineEnd = Camera.main.ScreenToWorldPoint(lineEnd);

                line.SetPosition(0, lineStart);
                line.SetPosition(1, lineEnd);
            }
        }
        void SpawnPerceptrons()
        {
            for(int i = 0; i< perceptronsNum; i++)
            {
                Vector3 randomScreenPosition = new Vector3();
                randomScreenPosition.x = Random.Range(0f, Screen.width);
                randomScreenPosition.y = Random.Range(0f, Screen.height);
                Perceptron perceptron = new Perceptron(new float[] { randomScreenPosition.x, randomScreenPosition.y });
                int guess = perceptron.Guess();

                Point point = Point.Spawn(pointTemplate, randomScreenPosition.x, randomScreenPosition.y);
                point.label = guess;
                points.Add(point);
                perceptrons.Add(perceptron);
                point.AddToScreen();

            }
        }
        void RandomizeInputs()
        {
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = Random.Range(-1f, 1f);
        }

        void Tick()
        {
            for(int i = 0; i < points.Count; i++)
            {
                perceptrons[i].Train(points[i].correctLabel);
                int guess = perceptrons[i].Guess();
                points[i].label = guess;
                points[i].AddToScreen();
            }
        }
    }

}