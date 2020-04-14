using System.Collections.Generic;
using UnityEngine;

public class GridGraph
{
    public int Width;
    public int Height;

    public Dictionary<Vector2,Node> Grid;
    //public Dictionary<Vector2, Node> Grid;

    public List<Vector2> _obstacles;
    /// <summary>
    /// Constructer of GridGraph script to assign variables to
    /// </summary>
    /// <param name="w">Grid's width value</param>
    /// <param name="h">Grid's height value</param>
    public GridGraph(int w, int h)
    {
        Grid = new Dictionary<Vector2, Node>();
        Width = w;
        Height = h;

        for (int x = (int)CameraScript.GetCameraLowerBounds().x; x < w; x++)
        {
            for (int y = (int)CameraScript.GetCameraLowerBounds().y; y < h; y++)
            {
                Node a = new Node(x, y);
                Vector2 av = new Vector2(x, y);
                Grid.Add(av,a);
            }
        }
    }

    /// <summary>
    /// Checks whether the neighbouring Node is within camera bounds or not 
    /// </summary>
    public bool IsNodeInside(Vector2 vec)
    {
        if (vec.x >= CameraScript.GetCameraLowerBounds().x && vec.x < this.Width &&
            vec.y >= CameraScript.GetCameraLowerBounds().y && vec.y < this.Height)
            return true;
        else
            return false;
    }

    public bool IsObstacle(Vector2 vec)
    {
        if(_obstacles.Contains(vec))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a List of neighbouring Nodes
    /// </summary>
    public List<Node> GetNeighbours(Node n)
    {
        List<Node> neighbours = new List<Node>();

        List<Vector2> directions = new List<Vector2>()
        {
            // 8-way direction
            new Vector2( -1, 0 ), // left
            new Vector2(-1, 1 ),  // top-left
            new Vector2( 0, 1 ),  // top
            new Vector2( 1, 1 ),  // top-right,
            new Vector2( 1, 0 ),  // right
            new Vector2( 1, -1 ), // bottom-right
            new Vector2( 0, -1 ), // bottom
            new Vector2( -1, -1 ) // bottom-left
        };

        foreach (Vector2 v in directions)
        {
            Vector2 newVector = v + n.Position;
            if (IsNodeInside(newVector) && !IsObstacle(newVector))
            {
                neighbours.Add(Grid[newVector]);
            }
        }

        return neighbours;
    }
}
