using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Decorator
{
    protected int counter = 0;
    protected int limit = 1;

    public Repeater(remBehaviour child) : base(child)
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

    //Ticks the child limit number of times, if child returns SUCCESS all the time then returns SUCCESS 
    //For the first time child returns RUNNING or FAILURE it returns RUNNING or FAILURE
    public override Status update()
    {
        counter = 0; 
        while (true)
        {
            child.tick();
            if (child.getStatus() == Status.RUNNING)
                break;
            if (child.getStatus() == Status.FAILURE)
                return Status.FAILURE;
            if (++counter == limit)
                return Status.SUCCESS;
        }
        return Status.RUNNING;
    }

    public int getLimit()
    {
        return limit;
    } 

    public void setLimit(int limit)
    {
        this.limit = limit;
    }
}
