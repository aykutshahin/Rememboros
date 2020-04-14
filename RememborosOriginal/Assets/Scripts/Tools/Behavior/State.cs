using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        public List<Transition> transitions = new List<Transition>();
        public void Tick()
        {

        }
    }
}

