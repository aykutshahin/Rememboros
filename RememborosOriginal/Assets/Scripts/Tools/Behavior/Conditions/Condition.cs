using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckCondition(StateManager state);
    }
}

