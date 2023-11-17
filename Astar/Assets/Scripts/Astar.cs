using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Unity.IO.LowLevel.Unsafe;

public class Astar
{
    /// <summary>
    /// Returns a list of positions from startPos to endPos on the given grid using the A* algorithm.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Utils.PriorityQueue<Cell, int> frontier = new();
        Dictionary<Cell, Cell> came_from = new();
        Dictionary<Cell, int> cost = new();

        Cell startCell = MazeGeneration.instance.GetCellForVector2(startPos);
        Cell endCell = MazeGeneration.instance.GetCellForVector2(endPos);
        frontier.Enqueue(startCell, 0);
        came_from.Add(startCell, null);
        cost.Add(startCell, 0);

        while (frontier.Count > 0)
        {
            Cell current = frontier.Dequeue();
            if (current == endCell)
            {
                Debug.Log($"Cells evaluated: {came_from.Count}");
                break;
            }
            foreach (Cell neighbor in current.GetNeighbours(grid))
            {
                Vector2 direction = neighbor.gridPosition - current.gridPosition;
                int neighborCost = cost[current] + 1;
                if (!current.HasWall(GetWallFromVector2(direction)) && (!cost.ContainsKey(neighbor) || neighborCost < cost[neighbor]))
                {
                    if (!cost.TryAdd(neighbor, neighborCost))
                    {
                        cost[neighbor] = neighborCost;
                    }
                    frontier.Enqueue(neighbor, neighborCost + Heuristic(endCell.gridPosition, neighbor.gridPosition));
                    if (!came_from.TryAdd(neighbor, current))
                    {
                        came_from[neighbor] = current;
                    }
                }
            }
        }
        if (!came_from.ContainsKey(endCell))
        {
            return null;
        }
        
        Cell backtrackCell = endCell;
        Stack<Vector2Int> pathStack = new();
        List<Vector2Int> path = new();
        while (backtrackCell != startCell)
        {
            pathStack.Push(backtrackCell.gridPosition);
            backtrackCell = came_from[backtrackCell];
        }
        while (pathStack.Count > 0)
        {
            path.Add(pathStack.Pop());
        }
        return path;
    }

    public int Heuristic(Vector2Int start, Vector2Int end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    public Wall GetWallFromVector2(Vector2 vector)
    {
        if (vector == Vector2.down)
        {
            return Wall.DOWN;
        }
        if (vector == Vector2.up)
        {
            return Wall.UP;
        }
        if (vector == Vector2.left)
        {
            return Wall.LEFT;
        }
        else
        {
            return Wall.RIGHT;
        }
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
