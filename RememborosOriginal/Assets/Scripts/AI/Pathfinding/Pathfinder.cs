using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    #region Variables

    // GridGraph's dimensions
    public int GraphWidth;
    public int GraphHeight;

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
        // Initialize a new GridGraph of a given width and height
        GraphWidth = Mathf.CeilToInt(mainCamera.orthographicSize) * 4 + (int)CameraScript.GetCameraLowerBounds().x;
        GraphHeight = Mathf.CeilToInt(mainCamera.orthographicSize) * 2 + (int)CameraScript.GetCameraLowerBounds().y;

        path = new List<Node>();
        map = new GridGraph(GraphWidth, GraphHeight);

        map._obstacles = Obstacles;

        int x1 = (int)StartNodePosition.x;
        int y1 = (int)StartNodePosition.y;
        int x2 = (int)GoalNodePosition.x;
        int y2 = (int)GoalNodePosition.y;

        // Find the path from StartNodePosition to GoalNodePosition
        path = AStar.FindPath(map, map.Grid[x1, y1], map.Grid[x2, y2]);
    }


    // When Pathfinder GameObject is selected show the Gizmos
    private void OnDrawGizmos()
    {
        if (path != null && map != null)
        {
            // Draw a Cube on the Editor window for each Node of the Graph
            for (int y = (int)CameraScript.GetCameraLowerBounds().y; y < GraphHeight; y++)
            {
                for (int x = (int)CameraScript.GetCameraLowerBounds().x; x < GraphWidth; x++)
                {
                    Gizmos.DrawWireCube(new Vector2(x + 0.5f, y + 0.5f), new Vector2(1f, 1f));
                }
            }

            foreach (Node n in path)
            {
                // The Goal node is RED
                if (n.Position == GoalNodePosition)
                {
                    Gizmos.color = Color.red;
                }
                // The Start node is BLUE
                else if (n.Position == StartNodePosition)
                {
                    Gizmos.color = Color.blue;
                }
                // Every other node in the path is GREEN
                else
                {
                    Gizmos.color = Color.black;
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
