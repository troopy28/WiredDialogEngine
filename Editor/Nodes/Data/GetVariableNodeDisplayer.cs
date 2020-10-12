using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Data
{
    public class GetVariableNodeDisplayer : WireNodeDisplayer
    {
        private GetVariableNodeDisplayer(GetVariableWireNode node) : base(node)
        {
        }

        protected override void DoCustomDrawing()
        {
            // Variable name
            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<GetVariableWireNode>().VariableName =
                EditorGUILayout.TextField(
                    new GUIContent("Name", "The name you gave to the variable"),
                    GetRenderedNodeAs<GetVariableWireNode>().VariableName
                );
            EditorGUILayout.EndHorizontal();
        }

        public static GetVariableNodeDisplayer CreateDisplayerFor(GetVariableWireNode node)
        {
            GetVariableNodeDisplayer nodeRenderer = new GetVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 220, 70)
            };
            return nodeRenderer;
        }

        public static GetVariableNodeDisplayer CreateGetVariableNodeDisplayer(Vector2 position)
        {
            GetVariableWireNode node = new GetVariableWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Get variable"
            };
            DialogEditor.InitializeNode(ref node);

            node.Outputs.Add(new OutputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "Variable",
                DataType = typeof(Variable)
            });

            GetVariableNodeDisplayer nodeRenderer = new GetVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 220, 70)
            };
            return nodeRenderer;
        }
    }
}