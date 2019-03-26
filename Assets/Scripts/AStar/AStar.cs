using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class AStar : MonoBehaviour
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

        public List<Vector2> blockers = new List<Vector2>();


        public List<Cell> grid = new List<Cell>();

        List<Cell> closedSet = new List<Cell>();
        List<Cell> openSet = new List<Cell>();
        List<Cell> pathFound = new List<Cell>();
        Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();


        int lowestScore = int.MaxValue;

        Cell from;
        Cell to;

        bool foundPath;
        
        

        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = frameRate;
            SetupGrid();
            from = grid[0];
            to = grid[grid.Count - 1];
            openSet.Add(from);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (foundPath)
                return;

            if (openSet.Count > 0)
            {
                Cell current = GetLowestScoreOpen();

                DrawBackground(current, CellStatus.Occupied);

                if (current == to)
                {
                    foundPath = true;
                    pathFound = ReconstructPath(current);
                    Debug.Log("Found Path");
                    DrawPathFound();
                    return;
                }
                openSet.Remove(current);
                closedSet.Add(current);

                List<Cell> neighbors = current.GetNeighborsAStar();
                foreach (Cell neighbor in neighbors)
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    int tentativeScore = current.gScore + DistanceBetween(current, neighbor);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else if (tentativeScore >= neighbor.gScore)
                        continue;

                    cameFrom[neighbor] = current;
                    neighbor.gScore = tentativeScore;
                    neighbor.fScore = neighbor.gScore + DistanceBetween(neighbor, to);
                }

            }
        }
        Cell GetLowestScoreOpen()
        {
            int lowest = int.MaxValue;
            Cell lCell = null;
            foreach(Cell cell in openSet)
            {
                if (cell.fScore <= lowest)
                {
                    lowest = cell.fScore;
                    lCell = cell;
                }
            }
            return lCell;
        }
        List<Cell> ReconstructPath(Cell current)
        {
            List<Cell> totalPath = new List<Cell>{ current };

            while(cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            return totalPath;
        }


        public static AStar Build(GameObject template)
        {
            return GameObject.Instantiate(template).GetComponent<AStar>();
        }

        void SetupGrid()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Vector2 pos = new Vector2(j, i);
                    Cell newCell = new Cell(i, j, this);
                    newCell.visited = blockers.Contains(pos);
                    DrawBackground(newCell, newCell.visited ? CellStatus.Visited : CellStatus.Empty);
                    grid.Add(newCell);
                }
            }
            grid[0].visited = true;

        }
        int DistanceBetween(Cell a, Cell b)
        {
            return Mathf.FloorToInt((a.Position - b.Position).magnitude);
        }
        public void DrawPathFound()
        {
            foreach(Cell cell in grid)
            {
                if (pathFound.Contains(cell))
                {
                    DrawBackground(cell, CellStatus.Occupied);
                }
                else if(!cell.visited)
                {
                    DrawBackground(cell);
                }
            }
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

        public int GetCellIndex(int x, int y)
        {
            if (x < 0 || y < 0 || x >= cols || y >= rows)
                return -1;

            return (x + y * rows);
        }
    }

}