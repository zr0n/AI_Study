using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class MazeGenerator : MonoBehaviour
    {
        public int rows = 10, cols = 10;
        public Line lineTemplate;

        public int cellSize = 100;

        public List<Cell> grid = new List<Cell>();
        // Start is called before the first frame update
        void Start()
        {
            SetupGrid();

        }

        // Update is called once per frame
        void Update()
        {

        }

       
        void SetupGrid()
        {
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    Cell newCell = new Cell(i, j, this);
                    grid.Add(newCell);
                    

                }
            }

        }
    }

    public class Cell
    {
        public int x, y;
        public bool[] walls = {true, true, true, true}; //top, right, bottom, left
        int cellSize;
        MazeGenerator maze;

        public Cell(int x, int y, MazeGenerator maze)
        {
            this.cellSize = maze.cellSize;
            this.x = x;
            this.y = y;
            this.maze = maze;
            Draw();
        }

        public void PushLine()
        {
        }

        public void Draw()
        {
            if (walls[0])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2((x * cellSize), (y * cellSize) + cellSize);
                newLine.b = new Vector2((x * cellSize) + cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
            }
            if (walls[1])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
            }
            if (walls[2])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize + cellSize, (y * cellSize));
                newLine.Draw();
            }
            if (walls[3])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2((x * cellSize) + cellSize, (y * cellSize));
                newLine.b = new Vector2((x * cellSize) + cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
            }
        }


    }

}
