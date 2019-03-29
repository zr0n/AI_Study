using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class Point : MonoBehaviour
    {
        public float x = 0f, y = 0f;
        public Color colorPositive = Color.green;
        public Color colorNegative = Color.red;
        public Color colorUndefined = Color.gray;
        public SpriteRenderer spriteRenderer;

        public int label = 1;
        public int correctLabel = 1;

        public static Point Spawn(GameObject template, float x = 0f, float y = 0f)
        {
            GameObject go;
            if (template)
                go = GameObject.Instantiate(template);
            else
                go = GameObject.Instantiate<GameObject>(new GameObject());

            var point =  go.GetComponent<Point>();

            if (!point)
                return null;

            point.x = x;
            point.y = y;
            point.AddToScreen();

            return point;
        }
       

        public void AddToScreen()
        {
            Vector3 screenPosition = new Vector3(x, y, Camera.main.nearClipPlane);
            transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
            UpdateSprite();
        }

        public void UpdateSprite()
        {
            if (label > 0)
                spriteRenderer.color = colorPositive;
            else if (label < 0)
                spriteRenderer.color = colorNegative;
            else
                spriteRenderer.color = colorUndefined;
        }
    }

}