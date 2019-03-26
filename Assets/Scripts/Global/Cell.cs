using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Cell
    {
        public int x, y;

        public Vector2 Position
        {
            get
            {
                return new Vector2(this.x, this.y); ;
            }
        }

        public bool[] walls = { true, true, true, true }; //top, right, bottom, left

        float cellSize;
        public bool visited;
        MazeGenerator maze;
        AStar aStar;
        GameObject currentSprite;
        List<Line> wallsRendered = new List<Line>();

        const int TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3;

        Line lineTemplate;

        /* A Star Search Variables */
        public int gScore = int.MaxValue;
        public int fScore = int.MaxValue;
        public Cell cameFrom;


        public Cell(int x, int y, MazeGenerator maze)
        {
            this.cellSize = maze.cellSize;
            this.x = x;
            this.y = y;
            
            this.maze = maze;
            //RandomWalls();
            lineTemplate = maze.lineTemplate;
            DrawWalls();
            maze.DrawBackground(this);


        }
        public Cell(int x, int y, AStar aStar)
        {
            this.cellSize = aStar.cellSize;
            this.x = x;
            this.y = y;
            this.aStar = aStar;
            lineTemplate = aStar.lineTemplate;
            DrawWalls();
            aStar.DrawBackground(this);


        }
       
        public void Redraw()
        {
            ClearAllLines();
            DrawWalls();
        }
        public void ClearAllLines()
        {
            foreach (Line line in wallsRendered)
            {
                if (line && line.gameObject)
                    GameObject.Destroy(line.gameObject);
            }
        }
        void RandomWalls()
        {
            for (int i = 0; i < 4; i++)
            {
                walls[i] = Random.Range(1, 10) % 2 == 0;
            }
        }

        public void SwapSprite(GameObject newSprite)
        {
            if (currentSprite != null)
                GameObject.Destroy(currentSprite);

            currentSprite = newSprite;
        }


        public int GetCellIndex(int i, int j)
        {
            if (maze != null)
                return maze.GetCellIndex(j, i);
            else if (aStar != null)
                return aStar.GetCellIndex(j, i);
            else
                return -1;
        }

        public void RemoveWallBetween(Cell neighborCell)
        {
            Cell a = this;
            Cell b = neighborCell;

            int i = a.x - b.x;
            int j = a.y - b.y;
            if (i == -1)
            {
                a.walls[LEFT] = false;
                b.walls[RIGHT] = false;
            }
            else if (i == 1)
            {
                a.walls[RIGHT] = false;
                b.walls[LEFT] = false;
            }

            else if (j == -1)
            {
                a.walls[TOP] = false;
                b.walls[BOTTOM] = false;
            }
            else if (j == 1)
            {
                a.walls[BOTTOM] = false;
                b.walls[TOP] = false;
            }
            Debug.Log("i: " + i + " j: " + j);
            a.Redraw();
            b.Redraw();
        }

        public List<Cell> GetNeighbors()
        {
            List<Cell> allNeighbors = new List<Cell>();
            List<Cell> unvisitedNeighbors = new List<Cell>();

            int iTop = GetCellIndex(x, y + 1);
            int iRight = GetCellIndex(x + 1, y);
            int iBottom = GetCellIndex(x, y - 1);
            int iLeft = GetCellIndex(x - 1, y);

            if (maze)
            {
                if (iTop > -1)
                    allNeighbors.Add(maze.grid[iTop]);
                if (iRight > -1)
                    allNeighbors.Add(maze.grid[iRight]);
                if (iBottom > -1)
                    allNeighbors.Add(maze.grid[iBottom]);
                if (iLeft > -1)
                    allNeighbors.Add(maze.grid[iLeft]);
            }
            


            foreach (var neighbor in allNeighbors)
            {
                if (!neighbor.visited)
                {
                    unvisitedNeighbors.Add(neighbor);
                    if (maze)
                        maze.DrawBackground(neighbor, neighbor.visited ? CellStatus.Visited : CellStatus.Empty);
                    
                }
            }

            return unvisitedNeighbors;
        }

        public List<Cell> GetNeighborsAStar()
        {
            int iTop = GetCellIndex(x, y + 1);
            int iRight = GetCellIndex(x + 1, y);
            int iBottom = GetCellIndex(x, y - 1);
            int iLeft = GetCellIndex(x - 1, y);

            List<Cell> allNeighbors = new List<Cell>();
            List<Cell> validNeighbors = new List<Cell>();
            if (aStar)
            {
                if (iTop > -1)
                    allNeighbors.Add(aStar.grid[iTop]);
                if (iRight > -1)
                    allNeighbors.Add(aStar.grid[iRight]);
                if (iBottom > -1)
                    allNeighbors.Add(aStar.grid[iBottom]);
                if (iLeft > -1)
                    allNeighbors.Add(aStar.grid[iLeft]);
            }

            foreach(Cell neighbor in allNeighbors)
            {
                if (!neighbor.visited)
                {
                    validNeighbors.Add(neighbor);
                }
            }
            return validNeighbors;
        }

        public void DrawWalls()
        {
            if (walls[0])
            {
                Line newLine = GameObject.Instantiate<Line>(lineTemplate);
                newLine.a = new Vector2((x * cellSize), (y * cellSize) + cellSize);
                newLine.b = new Vector2((x * cellSize) + cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
                wallsRendered.Add(newLine);
            }
            if (walls[1])
            {
                Line newLine = GameObject.Instantiate<Line>(lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
                wallsRendered.Add(newLine);

            }
            if (walls[2])
            {
                Line newLine = GameObject.Instantiate<Line>(lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize + cellSize, (y * cellSize));
                newLine.Draw();
                wallsRendered.Add(newLine);

            }
            if (walls[3])
            {
                Line newLine = GameObject.Instantiate<Line>(lineTemplate);
                newLine.a = new Vector2((x * cellSize) + cellSize, (y * cellSize));
                newLine.b = new Vector2((x * cellSize) + cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
                wallsRendered.Add(newLine);
            }
        }




    }

    public enum CellStatus
    {
        Empty,
        Occupied,
        Visited,
        Possible
    }


}