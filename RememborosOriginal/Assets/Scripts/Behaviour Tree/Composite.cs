using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Composite : remBehaviour
{
    //Holds children in a sequental data
    protected List<remBehaviour> children = new List<remBehaviour>();

    public virtual void addChild(remBehaviour child)
    {
        children.Add(child);
    }

    public virtual void removeChild(remBehaviour child) 
    {
        children.Remove(child);
    }

    public virtual void clearChildren()
    {
        children.Clear();
        children.Capacity = 0;
    }
}
