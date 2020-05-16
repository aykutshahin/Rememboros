using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    #region Variables
    // The position of Start and Goal nodes
    protected Vector2 StartNodePosition;
    protected Vector2 GoalNodePosition;

    //The position of obstacles
    protected List<Vector2> Obstacles;
    //Grid map
    public GridGraph map;

    //Founded path
    public List<Node> path;

    public GameObject gridTile;

    protected BoxCollider2D gridBox;

    protected Vector2 testPos;

    public Vector2 target;
    #endregion

    private void Awake()
    {
        testPos = target = Vector2.zero;
        Obstacles = new List<Vector2>();
        gridBox = gridTile.GetComponentInChildren<BoxCollider2D>();
        path = new List<Node>();
        map = new GridGraph(new Vector2(Mathf.FloorToInt(gridBox.bounds.min.x), Mathf.FloorToInt(gridBox.bounds.min.y)), new Vector2(Mathf.CeilToInt(gridBox.bounds.max.x), Mathf.CeilToInt(gridBox.bounds.max.y)));
    }
    private void Start()
    {
        Obstacles = map._obstacles;
    }
    private void Update()
    {
        testPos = new Vector2((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y));
        if(testPos != GoalNodePosition)
        {
            StartNodePosition.x = (int)Mathf.Floor(transform.position.x);
            StartNodePosition.y = (int)Mathf.Floor(transform.position.y);
        }

       GoalNodePosition.x = (int)Mathf.Floor(target.x);
       GoalNodePosition.y = (int)Mathf.Floor(target.y);

        // Find the path from StartNodePosition to GoalNodePosition
        path = AStar.FindPath(map, map.Grid[StartNodePosition], map.Grid[GoalNodePosition]);
    }


    // When Pathfinder GameObject is selected show the Gizmos
    private void OnDrawGizmosSelected()
    {
        if(gridTile != null)
        {
            // Draw a Cube on the Editor window for each Node of the Graph
            for (int y = (int)map.minCoords.y; y < (int)map.maxCoords.y; y++)
            {
                for (int x = (int)map.minCoords.x; x < (int)map.maxCoords.x; x++)
                {
                    Gizmos.DrawWireCube(new Vector2(x + 0.5f, y + 0.5f), new Vector2(1f, 1f));
                }
            }
        }

        if (path != null && map != null)
        {
            foreach (Node n in path)
            {
                // The Goal node is RED
                if (n.Position == GoalNodePosition)
                {
                    Gizmos.color = Color.yellow;
                }
                // The Start node is BLUE
                else if (n.Position == StartNodePosition)
                {
                    Gizmos.color = Color.blue;
                }
                // Every other node in the path is GREEN
                else
                {
                    Gizmos.color = Color.magenta;
                }
                Gizmos.DrawWireCube(n.Position + new Vector2(0.5f, 0.5f), new Vector2(1f, 1f));
            }

            foreach (Vector2 vec in Obstacles)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(new Vector2(vec.x + 0.5f, vec.y + 0.5f), new Vector2(1f, 1f));
            }

        }
    }
       
}
