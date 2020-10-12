using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Animation
{
    public class GetAnimatorVariableNodeDisplayer : WireNodeDisplayer
    {
        public ActorIdentifierNodeField ActorField { get; private set; }

        private GetAnimatorVariableNodeDisplayer(GetAnimatorVariableWireNode node) : base(node)
        {
            ActorField = new ActorIdentifierNodeField(this)
            {
                FieldName = "Target"
            };
            ActorField.FieldValue = node.TargetActor;
        }

        protected override void DoCustomDrawing()
        {
            // Variable name
            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<GetAnimatorVariableWireNode>().VariableName = EditorGUILayout.TextField(
                new GUIContent("Name", "The name of the parameter in the Unity animator"),
                GetRenderedNodeAs<GetAnimatorVariableWireNode>().VariableName, 
                GUILayout.Width(182));
            EditorGUILayout.EndHorizontal();

            // Actor
            ActorField.Draw();
            GetRenderedNodeAs<GetAnimatorVariableWireNode>().TargetActor = ActorField.FieldValue;
        }

        public static GetAnimatorVariableNodeDisplayer CreateDisplayerFor(GetAnimatorVariableWireNode node)
        {
            GetAnimatorVariableNodeDisplayer nodeRenderer = new GetAnimatorVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 220, 80)
            };
            return nodeRenderer;
        }

        public static GetAnimatorVariableNodeDisplayer CreateGetAnimatorVariableNodeDisplayer(Vector2 position)
        {
            GetAnimatorVariableWireNode node = new GetAnimatorVariableWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Get animator variable"
            };
            DialogEditor.InitializeNode(ref node);

            node.Outputs.Add(new OutputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "Variable",
                DataType = typeof(AnimatorVariable)
            });
            GetAnimatorVariableNodeDisplayer nodeRenderer = new GetAnimatorVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 220, 80)
            };
            return nodeRenderer;
        }
    }
}