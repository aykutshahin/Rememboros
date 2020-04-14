using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rememboros.BehaviorTreeEditor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        #region Variables
        static List<BaseNode> windows = new List<BaseNode>();
        Vector3 mousePosition;
        bool makeTransition;
        bool clickedOnWindow;
        BaseNode selectedNode;

        //Types of user input actions
        public enum UserActions
        {
            addState,
            addTransitionNode,
            deleteNode,
            commentNode
        }
        #endregion

        #region Init
        //Creating BehaviorTreeEditor section on tool bar
        [MenuItem("Behavior Tree Editor / Editor")]
        static void ShowEditor()
        {
            BehaviorTreeEditor editor = EditorWindow.GetWindow<BehaviorTreeEditor>();
            editor.minSize = new Vector2(800, 600);
        }
        #endregion

        #region GUI Methods
        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();
        }

        private void OnEnable()
        {
        }

        void DrawWindows()
        {
            BeginWindows();
            foreach(BaseNode n in windows)
            {
                n.DrawCurve();
            }
            for(int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }
            EndWindows();
        }

        void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        /// <summary>
        /// Getting user input function
        /// </summary>
        void UserInput(Event e)
        {
            if(e.button == 1 && !makeTransition)
            {
                if(e.type == EventType.MouseDown)
                {
                    RightClick(e);
                }
            }
            if (e.button == 0 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                   
                }
            }
        }

        /// <summary>
/// Clicking right mouse button function
/// </summary>
        void RightClick(Event e)
        {
            selectedNode = null;
            for(int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];
                    break;
                }
            }

            if (!clickedOnWindow)
            {
                AddNewNodes(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        /// <summary>
        /// When clicking right mouse button on gui's empty space, this method creates a menu
        /// </summary>
        void AddNewNodes(Event e)
        {     
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add State"), false, ContextCallBack, UserActions.addState);
            menu.AddItem(new GUIContent("Add Comment"), false, ContextCallBack, UserActions.commentNode);
            menu.ShowAsContext();
            e.Use();
        }

        //14.01 dakika kaldım
        /// <summary>
        /// When clicking right mouse button on gui's one of state windows, this method creates a menu to modify that state window
        /// </summary>
        void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (selectedNode is StateNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Add Transition"), false, ContextCallBack, UserActions.addTransitionNode);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserActions.deleteNode);
            }
            if(selectedNode is CommentNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallBack, UserActions.deleteNode);
            }
            menu.ShowAsContext();
            e.Use();
        }

        /// <summary>
        ///When adding item to generic menu it used to specify what kind of action will be placed
        /// </summary>
        void ContextCallBack(object o)
        {
            UserActions a = (UserActions)o;
            switch (a)
            {
                case UserActions.addState:
                    StateNode stateNode = new StateNode { windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300), windowTitle = "State" };
                    windows.Add(stateNode);
                    break;
                case UserActions.addTransitionNode:
                    break;
                case UserActions.commentNode:
                    CommentNode commentNode = new CommentNode { windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100), windowTitle = "Comment" };
                    windows.Add(commentNode);
                    break;
                default:
                    break;
                case UserActions.deleteNode:
                    if(selectedNode != null)
                    {
                        windows.Remove(selectedNode);
                    }
                    break;
            }
        }
        #endregion

        #region Helper Methods
        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3((left) ? start.x + start.width : start.x,start.y + (start.height * .5f), 0);
            Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);
            for(int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan,shadow,null, (i+1) * .5f);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 1);
        }
        #endregion
    }
}

