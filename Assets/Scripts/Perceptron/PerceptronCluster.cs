using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{

    public class PerceptronCluster : MonoBehaviour
    {
        public int perceptronsNum = 100;
        public GameObject pointTemplate;
        public LineRenderer line;
        public LineRenderer lineCorrect;
        public bool autoTick = false;
        public int applicationFrameRate = 60;
        public float a = 1, b = 1;

        public List<Point> points = new List<Point>();
        public List<Perceptron> perceptrons = new List<Perceptron>();
        Perceptron perceptron = null;
        Vector3 start, end;
        // Start is called before the first frame update
        void Start()
        {


            SpawnPoints();
            DrawCorrectLine();
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
                Vector3 lineStart = new Vector3(start.x, perceptron.GetY(start.x), Camera.main.nearClipPlane);
                Vector3 lineEnd = new Vector3(Screen.width, perceptron.GetY(Screen.width), Camera.main.nearClipPlane);


                Debug.Log("Line Start " + lineStart);
                Debug.Log("Line End " + lineEnd.ToString());
                line.SetPosition(0, Camera.main.ScreenToWorldPoint(lineStart));
                line.SetPosition(1, Camera.main.ScreenToWorldPoint(lineEnd));
            }
        }
        void SpawnPoints()
        {
            perceptron = new Perceptron();
            for(int i = 0; i< perceptronsNum; i++)
            {
                Vector3 randomScreenPosition = new Vector3();
                randomScreenPosition.x = Random.Range(0f, Screen.width);
                randomScreenPosition.y = Random.Range(0f, Screen.height);
                int guess = perceptron.Guess(new float []{ randomScreenPosition.x, randomScreenPosition.y });

                Point point = Point.Spawn(pointTemplate, randomScreenPosition.x, randomScreenPosition.y);
                point.label = guess;
                points.Add(point);
                point.AddToScreen();

            }
            perceptrons.Add(perceptron);
        }
    

        void Tick()
        {
            for(int i = 0; i < points.Count; i++)
            {
                //perceptrons[i].Train(points[i].correctLabel);
                float[] inputs = new float[] { points[i].x, points[i].y, b };
                perceptron.Train(inputs, points[i].correctLabel);
                int guess = perceptron.Guess(inputs);
                points[i].label = guess;
                float lineY = F(points[i].x);
                if (lineY < points[i].y)
                    points[i].correctLabel = 1;
                else
                    points[i].correctLabel = -1;

                points[i].AddToScreen();
            }

            DrawLine();
        }

        float F(float x)
        {
            return a * x + b;
        }

        void DrawCorrectLine()
        {
            start = new Vector3();
            start.x = 0 ;
            start.y = F(start.x);

            end = new Vector3();
            end.x = Screen.width;
            end.y = F(end.x);

            start.z = end.z = Camera.main.nearClipPlane;


            lineCorrect.SetPosition(0, Camera.main.ScreenToWorldPoint(start));
            lineCorrect.SetPosition(1, Camera.main.ScreenToWorldPoint(end));
        }
    }

}