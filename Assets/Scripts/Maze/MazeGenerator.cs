﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class MazeGenerator : MonoBehaviour
    {
        public int rows = 10, cols = 10;
        public Line lineTemplate;

        public float cellSize = 100;
        public int frameRate = 1;

        public GameObject cellOccupied;
        public GameObject cellVisited;
        public GameObject cellEmpty;
        public GameObject cellPossible;

        public Vector2 spriteScale = new Vector2(10, 10);

        public float spritePositionFactor = 1;
        public Vector2 spriteGridOffset = new Vector2(5f, 5f);


        public List<Cell> grid = new List<Cell>();

        Cell currentCell;

        List<Cell> stack = new List<Cell>();
        // Start is called before the first frame update
        void Start()
        {
            SetupGrid();
            Application.targetFrameRate = frameRate;
            StartCoroutine(Tick());

        }

        // Update is called once per frame
        IEnumerator Tick()
        {
            MoveAround();

            yield return new WaitForSeconds(1f / frameRate);
            StartCoroutine(Tick());
            yield return null;
        }

        void MoveAround()
        {
            List<Cell> neighbors = currentCell.GetNeighbors();
            Cell movingTo;
            if (neighbors.Count > 0)
            {
                movingTo = neighbors[Random.Range(0, neighbors.Count)];
                stack.Add(movingTo);
            }
            else if(stack.Count > 0)
            {
                int lastIndex = stack.Count - 1;
                movingTo = stack[lastIndex];
                stack.RemoveAt(lastIndex);
            }
            else
            {
                Debug.Log("Finished");
                StopAllCoroutines();
                return;
            }
            
            movingTo.visited = true;

            Debug.Log("Moving from " + currentCell.x + "," + currentCell.y + " to " + movingTo.x + "," + movingTo.y);

            DrawBackground(currentCell, CellStatus.Visited);

            currentCell.RemoveWallBetween(movingTo);

            var neighborsNew = movingTo.GetNeighbors();

            foreach (var neighbor in neighborsNew)
                DrawBackground(neighbor, CellStatus.Possible);

            currentCell = movingTo;

            DrawBackground(currentCell, CellStatus.Occupied);
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
            grid[0].visited = true;
            currentCell = grid[0];

        }

        public int GetCellIndex(int x, int y)
        {
            if (x < 0 || y < 0 || x >= cols || y >= rows)
                return -1;

            return (x + y * rows);
        }


        public void DrawBackground(Cell cell, CellStatus status = CellStatus.Empty)
        {
            GameObject objectToSpawn = GetSpriteToSpawn(status);
            if (objectToSpawn == null)
                return;

            GameObject spawnedObject = Instantiate(objectToSpawn);

            Vector3 worldPosition = new Vector3();
            worldPosition.x = (cell.x * spritePositionFactor) + spriteGridOffset.x;
            worldPosition.y = (cell.y * spritePositionFactor) + spriteGridOffset.y;

            spawnedObject.transform.position = worldPosition;
            spawnedObject.transform.localScale = spriteScale;

            cell.SwapSprite(spawnedObject);

        }
        GameObject GetSpriteToSpawn(CellStatus status = CellStatus.Empty)
        {
            switch (status)
            {
                case CellStatus.Visited:
                    return cellVisited;
                case CellStatus.Occupied:
                    return cellOccupied;
                case CellStatus.Possible:
                    return cellPossible;
                default:
                    return cellEmpty;
            }
        }
    }

    public class Cell
    {
        public int x, y;
        public bool[] walls = {true, true, true, true}; //top, right, bottom, left
       
        float cellSize;
        public bool visited;
        MazeGenerator maze;
        GameObject currentSprite;
        List<Line> wallsRendered = new List<Line>();
        const int TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3;

        public Cell(int x, int y, MazeGenerator maze)
        {
            this.cellSize = maze.cellSize;
            this.x = x;
            this.y = y;
            this.maze = maze;
            //RandomWalls();
            Draw();
            maze.DrawBackground(this);

            
        }
        public void Redraw()
        {
            ClearAllLines();
            Draw();
        }
        public void ClearAllLines()
        {
            foreach(Line line in wallsRendered)
            {
                if(line && line.gameObject)
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
            if(currentSprite != null)
                GameObject.Destroy(currentSprite);

            currentSprite = newSprite;
        }


        public int GetCellIndex(int i, int j)
        {
            if (maze == null)
                return -1;
            return maze.GetCellIndex(j, i);
        }

        public void RemoveWallBetween(Cell neighborCell)
        {
            Cell a = this;
            Cell b = neighborCell;

            int i = a.x - b.x;
            int j = a.y - b.y;
            if(i == -1)
            {
                a.walls[LEFT] = false;
                b.walls[RIGHT] = false;
            }
            else if(i == 1)
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
            Debug.Log("i: " + i +" j: " + j);
            a.Redraw();
            b.Redraw();
        }

        public List<Cell> GetNeighbors()
        {
            List<Cell> allNeighbors = new List<Cell>();
            List<Cell> unvisitedNeighbors = new List<Cell>();

            int iTop = GetCellIndex     ( x     , y + 1 );
            int iRight = GetCellIndex   ( x + 1 , y     );
            int iBottom = GetCellIndex  ( x     , y - 1 );
            int iLeft = GetCellIndex    ( x - 1 , y     );


            if (iTop > -1)
                allNeighbors.Add(maze.grid[iTop]);
            if (iRight > -1)
                allNeighbors.Add(maze.grid[iRight]);
            if (iBottom > -1)
                allNeighbors.Add(maze.grid[iBottom]);
            if (iLeft > -1)
                allNeighbors.Add(maze.grid[iLeft]);


            foreach (var neighbor in allNeighbors)
            {
                if (!neighbor.visited)
                {
                    unvisitedNeighbors.Add(neighbor);
                    maze.DrawBackground(neighbor, neighbor.visited ? CellStatus.Visited : CellStatus.Empty);
                }


            }

            return unvisitedNeighbors;
        }

        public void Draw()
        {
            if (walls[0])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2((x * cellSize), (y * cellSize) + cellSize);
                newLine.b = new Vector2((x * cellSize) + cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
                wallsRendered.Add(newLine);
            }
            if (walls[1])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize, (y * cellSize) + cellSize);
                newLine.Draw();
                wallsRendered.Add(newLine);

            }
            if (walls[2])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
                newLine.a = new Vector2(x * cellSize, y * cellSize);
                newLine.b = new Vector2(x * cellSize + cellSize, (y * cellSize));
                newLine.Draw();
                wallsRendered.Add(newLine);

            }
            if (walls[3])
            {
                Line newLine = GameObject.Instantiate<Line>(maze.lineTemplate);
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
