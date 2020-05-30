using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    //Hold the root of the tree it serves as a central point to use the tree.
    protected remBehaviour Root; 
    //Tree holds the data it needs in this object
    public DataContext context = new DataContext();

    public BehaviourTree(remBehaviour Root)
    {
        this.Root = Root;
    }

    //BT tick is to be called every frame (Update method in the class that uses BT)
    public void tick()
    {
        Root.tick();
    } 

    public void setRoot(remBehaviour Root)
    {
        this.Root = Root; 
    } 

    public remBehaviour getRoot()
    {
        return Root;
    }
}
