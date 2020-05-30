using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Composite
{
    public override void onInitialize()
    {
        status = Status.RUNNING;
    }

    public override void onTerminate(Status status)
    {
        //NOTHING
    }

    public override Status update()
    {
        //Keep going until a child behavior says it’s running or the list ends.
        foreach (remBehaviour rb in children)
        {
            Status s = rb.tick();
            //If child succeeds or keeps running, do the same 
            if (s != Status.FAILURE)
                return s;
        }
        //If traversed through all of them then return failure.
        return Status.FAILURE;
    }
}
