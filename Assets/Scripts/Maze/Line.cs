using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Line : MonoBehaviour
    {

        public LineRenderer lineRenderer;
        public Vector2 a, b;
        int size = 1;
        // Start is called before the first frame update
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void Draw()
        {
            if (lineRenderer == null)
            {
                Debug.Log("Null line renderer");
                return;

            }
            lineRenderer.SetPosition(0, a);
            lineRenderer.SetPosition(1, b);
        }


    }

}