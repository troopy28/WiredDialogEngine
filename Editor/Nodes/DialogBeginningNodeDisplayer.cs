using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class DialogBeginningNodeDisplayer : WireNodeDisplayer
    {

        private DialogBeginningNodeDisplayer(DialogBeginningWireNode node) : base(node)
        {

        }

        protected override GenericMenu GetContextMenu()
        {
            return null;
        }

        protected override void DoCustomDrawing()
        {
            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<DialogBeginningWireNode>().Delay = EditorGUILayout.FloatField(
                new GUIContent("Delay", "The delay before starting playing the dialog"),
                GetRenderedNodeAs<DialogBeginningWireNode>().Delay);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Everything begins here. Your dialog logic " + Environment.NewLine +
                            "should (and must) be wired from there. ");
            EditorGUILayout.EndHorizontal();
        }

        public static DialogBeginningNodeDisplayer CreateDisplayerFor(DialogBeginningWireNode node)
        {
            DialogBeginningNodeDisplayer nodeRenderer = new DialogBeginningNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 100)
            };
            return nodeRenderer;
        }

        public static DialogBeginningNodeDisplayer CreateDialogBeginningDisplayer(Vector2 position)
        {
            DialogBeginningWireNode node = new DialogBeginningWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Dialog beginning"
            };
            DialogEditor.InitializeNode(ref node);

            DialogBeginningNodeDisplayer nodeRenderer = new DialogBeginningNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 100)
            };
            return nodeRenderer;
        }
    }
}