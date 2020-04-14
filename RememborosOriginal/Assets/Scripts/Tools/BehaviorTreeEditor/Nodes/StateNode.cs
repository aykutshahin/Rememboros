using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rememboros;

namespace Rememboros.BehaviorTreeEditor
{
    public class StateNode : BaseNode
    {
        public State currentState;
        bool collapse;

        public override void DrawWindow()
        {
            if(currentState == null)
            {
                EditorGUILayout.LabelField("Add state to modify");
            }
            else
            {
                if (!collapse)
                {
                    windowRect.height = 300;
                }
                else
                {
                    windowRect.height = 100;
                }
                collapse = EditorGUILayout.Toggle(" ", collapse);
            }

            currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);
        }

        public override void DrawCurve()
        {
           
        }
    }
}

