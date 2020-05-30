using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : Sequencer
{
    //Add condition to the start of the list
    public void addCondition(Condition condition)
    {   
        children.Insert(0, condition);
    }

    //Add action to the end of the list
    public void addAction(Action action){
        children.Add(action);
    } 

}
