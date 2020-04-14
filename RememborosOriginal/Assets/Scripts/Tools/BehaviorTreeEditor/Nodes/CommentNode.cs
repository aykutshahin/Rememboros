using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros.BehaviorTreeEditor
{
    public class CommentNode : BaseNode
    {
        string comment = "This is a comment";

        public override void DrawWindow()
        {
            comment = GUILayout.TextArea(comment, 200);
        }
    }
}

