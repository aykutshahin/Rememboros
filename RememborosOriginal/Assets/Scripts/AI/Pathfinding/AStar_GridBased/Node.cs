using System;
using UnityEngine;

public class Node
{
    public Vector2 Position;
    public float heuristicValue;

    public Node(float x, float y)
    {
        Position = new Vector2(x, y);
    }
}