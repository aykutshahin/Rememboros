using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Decorator
{
    public Inverter(remBehaviour child) : base(child)
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

    //If child's status is SUCCESS or FAILURE it inverts it.
    public override Status update()
    {
        child.tick();
        status = child.getStatus(); 

        if(status == Status.SUCCESS)
        {
            status = Status.FAILURE;

        }else if(status == Status.FAILURE)
        {
            status = Status.SUCCESS;
        }

        return status;
    }

}
