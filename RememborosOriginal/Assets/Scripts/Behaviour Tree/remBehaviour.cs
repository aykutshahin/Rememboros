using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class remBehaviour : MonoBehaviour
{
    //Called once, immediately before update
    public abstract void onInitialize();
    //Called once each time the behavior tree is updated, until it signals it has terminated according to its returning status
    public abstract Status update();
    //Called once, immediately after the previous update signals it’s no longer running 
    //If there is thing to do you would do it according to status, if it's FAILURE or SUCCESS
    public abstract void onTerminate(Status status);

    //Initializes with INVALID value
    protected Status status = Status.INVALID;

    //Wraps and handles all other functions at a single place
    public virtual Status tick()
    {
        if (status != Status.RUNNING)
            onInitialize();

        status = update();

        if (status != Status.RUNNING)
            onTerminate(status);

        return status;
    } 

    public Status getStatus()
    {
        return status;
    } 

    public void setStatus(Status status)
    {
        this.status = status;
    }
}
