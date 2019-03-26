using System.Collections;
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

    
}
