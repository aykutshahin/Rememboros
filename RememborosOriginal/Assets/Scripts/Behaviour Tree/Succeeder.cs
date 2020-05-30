using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succeeder : Decorator
{
    public Succeeder(remBehaviour child) : base(child)
    {
    }

    public override void onInitialize()
    {
        status = Status.RUNNING;
    }

    public override void onTerminate(Status status)
    {
        //NOTHING
    }

    //Returns SUCCESS no matter what.
    public override Status update()
    {
        child.tick();
        return Status.SUCCESS;
    }


}
