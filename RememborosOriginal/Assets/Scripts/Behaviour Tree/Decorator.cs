using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decorator : remBehaviour
{
    //Base class for decorator, has one child, each decorator has a specific ability
    protected remBehaviour child;

    public Decorator(remBehaviour child)
    {
        this.child = child;
    } 

    public remBehaviour getChild()
    {
        return child;
    }

    public void setChild(remBehaviour child)
    {
        this.child = child;
    }

}
