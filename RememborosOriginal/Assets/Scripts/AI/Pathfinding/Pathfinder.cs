using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    #region Variables

    [SerializeField] private Camera mainCamera = null;

    // The position of Start and Goal nodes
    public Vector2 StartNodePosition;
    public Vector2 GoalNodePosition;

    //The position of obstacles
    public List<Vector2> Obstacles;
    //Grid map
    public GridGraph map;
    //Founded path
    public List<Node> path;

    #endregion

    private void Update()
    {
        path = new List<Node>();
        map = new GridGraph(CameraScript.GraphWidth, CameraScript.GraphHeight);

       StartNodePosition.x = (int)Mathf.Floor(transform.position.x);
       StartNodePosition.y = (int)Mathf.Floor(transform.position.y);
        GoalNodePosition.x = (int)Mathf.Floor(GetComponent<GY_Crow>().target.x);
        GoalNodePosition.y = (int)Mathf.Floor(GetComponent<GY_Crow>().target.y);

        map._obstacles = Obstacles;

        // Find the path from StartNodePosition to GoalNodePosition
        path = AStar.FindPath(map, map.Grid[StartNodePosition], map.Grid[GoalNodePosition]);
    }


    // When Pathfinder GameObject is selected show the Gizmos
    private void OnDrawGizmos()
    {
        // Draw a Cube on the Editor window for each Node of the Graph
        for (int y = (int)CameraScript.GetCameraLowerBounds().y; y < CameraScript.GraphHeight; y++)
        {
            for (int x = (int)CameraScript.GetCameraLowerBounds().x; x < CameraScript.GraphWidth; x++)
            {
                Gizmos.DrawWireCube(new Vector2(x + 0.5f, y + 0.5f), new Vector2(1f, 1f));
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
