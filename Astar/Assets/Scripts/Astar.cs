using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Queue<Cell> frontier = new Queue<Cell>();
        Dictionary<Cell, Cell> came_from = new Dictionary<Cell, Cell>();

        Cell startCell = MazeGeneration.instance.GetCellForVector2(startPos);
        Cell endCell = MazeGeneration.instance.GetCellForVector2(endPos);
        frontier.Enqueue(startCell);
        came_from.Add(startCell, null);

        while (frontier.Count > 0)
        {
            Cell current = frontier.Dequeue();
            foreach (Cell neighbor in current.GetNeighbours(grid))
            {
                Vector2 direction = neighbor.gridPosition - current.gridPosition;
                if (!current.HasWall(GetWallFromVector2(direction)) && !came_from.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    came_from.Add(neighbor, current);
                }
            }
            if (current == endCell)
            {
                break;
            }
        }
        if (!came_from.ContainsKey(endCell))
        {
            return null;
        }

        Cell backtrackCell = endCell;
        Stack<Vector2Int> pathStack = new Stack<Vector2Int>();
        List<Vector2Int> path = new List<Vector2Int>();
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
