using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Data
{
    public class SetVariableValNodeDisplayer : WireNodeDisplayer
    {
        public ConstantNodeField ConstantField { get; private set; }

        private SetVariableValNodeDisplayer(SetVariableValWireNode node) : base(node)
        {
        }

        protected override void DoCustomDrawing()
        {
            // Variable name
            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<SetVariableValWireNode>().VariableName =
                EditorGUILayout.TextField(
                    new GUIContent("Variable name", "The name you gave to the variable."),
                    GetRenderedNodeAs<SetVariableValWireNode>().VariableName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<SetVariableValWireNode>().UpdatedValue =
                EditorGUILayout.TextField(
                    new GUIContent("Value", "The value the variable will take. Take care about the type, which can't change during the execution."),
                    GetRenderedNodeAs<SetVariableValWireNode>().UpdatedValue
                );
            EditorGUILayout.EndHorizontal();
        }

        public static SetVariableValNodeDisplayer CreateDisplayerFor(SetVariableValWireNode node)
        {
            SetVariableValNodeDisplayer nodeRenderer = new SetVariableValNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 90)
            };  
            return nodeRenderer;
        }

        public static SetVariableValNodeDisplayer CreateSetVariableValDisplayer(Vector2 position)
        {
            SetVariableValWireNode node = new SetVariableValWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Update variable"
            };
            DialogEditor.InitializeNode(ref node);

            SetVariableValNodeDisplayer nodeRenderer = new SetVariableValNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 90)
            };
            return nodeRenderer;
        }
    }
}
