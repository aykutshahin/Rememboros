using System.Collections.Generic;
using UnityEngine;

public class GridGraph
{
    public Vector2  maxCoords;
    public Vector2  minCoords;

    public Dictionary<Vector2,Node> Grid;
    //public Dictionary<Vector2, Node> Grid;

    public List<Vector2> _obstacles;

    Collider2D[] hitObstacles;

    /// <summary>
    /// Constructer of GridGraph script to assign variables to
    /// </summary>
    /// <param name="w">Grid's width value</param>
    /// <param name="h">Grid's height value</param>
    public GridGraph(Vector2 minCoord, Vector2 maxCoord)
    {
        _obstacles = new List<Vector2>();
        Grid = new Dictionary<Vector2, Node>();
        maxCoords = maxCoord;
        minCoords = minCoord;

        for (int x = (int)minCoord.x; x < (int)maxCoord.x; x++)
        {
            for (int y = (int)minCoord.y; y < (int)maxCoord.y; y++)
            {
                Node a = new Node(x, y);
                Vector2 av = new Vector2(x, y);
                Grid.Add(av,a);
                hitObstacles = Physics2D.OverlapCircleAll(av + new Vector2(0.5f, 0.5f), 0.2f, LayerMask.GetMask("Ground"));
                if(hitObstacles.Length > 0)
                {
                    _obstacles.Add(av);
                }
            }
        }

    }

    /// <summary>
    /// Checks whether the neighbouring Node is within camera bounds or not 
    /// </summary>
    public bool IsNodeInside(Vector2 vec)
    {
        if (vec.x >= minCoords.x && vec.x < maxCoords.x &&
            vec.y >= minCoords.y && vec.y < maxCoords.y)
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
