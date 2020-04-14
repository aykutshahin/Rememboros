using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Returns the best path as a List of Nodes
    /// </summary>
    /// Static type used to call FindPath function in Pathfinder script like AStar.FindPath(.....).
    public static List<Node> FindPath(GridGraph graph, Node start, Node goal)
    {
        //Open-list is a priority queue that stores Nodes and theirs heuristic values, purpose of using it getting neighbours of our current node.
        SimplePriorityQueue<Node> openList = new SimplePriorityQueue<Node>();

        //Dictionary used to check current node which node came from.
        Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();

        //Dictionary used to check nodes that have been visited in order to prevent infinity while loop.
        Dictionary<Node, Node> pseudoClosedList = new Dictionary<Node, Node>();

        //Purpose of using closedList is to store our path.
        List<Node> closedList = new List<Node>();

        //Start came from start :))
        came_from.Add(start, start);

        //Added start node to openlist and set heuristic value to zero.
        openList.Enqueue(start, 0);

        Node current = new Node(0, 0);

        //Until open list is empty
        while (openList.Count > 0)
        {
            //Current node is node from openlist that has lowest heuristic value.
            current = openList.Dequeue();

            if (current == goal)
            {
                break; // Exit, path founded
            }
            foreach (Node neighbour in graph.GetNeighbours(current))
            {
                if (!pseudoClosedList.ContainsKey(neighbour))
                {
                    pseudoClosedList[neighbour] = neighbour; // Assign neigbour visited
                    came_from[neighbour] = current; // Assign this neighbour came from current node
                    neighbour.heuristicValue = Heuristic(neighbour, goal); // Assign neigbours heuristic value
                    openList.Enqueue(neighbour, neighbour.heuristicValue); //Add this neighbour to openlist
                }
            }
        }
        while(current != start)
        {
            closedList.Add(current);
            current = came_from[current];
        }
        closedList.Reverse();
        return closedList;
    }

    /// <summary>
    /// Getting heuristic value, manhattan distance algorithm used.
    /// </summary>
    public static float Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.Position.x - b.Position.x) + Mathf.Abs(a.Position.y - b.Position.y);
    }
}
