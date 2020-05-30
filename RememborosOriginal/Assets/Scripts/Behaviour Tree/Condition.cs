using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : remBehaviour
{
    //The check is run once immediately and the condition terminates.
    public abstract bool instantCheck();
    //Keep checking a condition over time, and keep running every frame as long as it is True. If it becomes False, then exit with a FAILURE code.
    public abstract bool monitorCheck();
}
