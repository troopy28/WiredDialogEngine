using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using UnityEngine;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Animation
{
    public class SetAnimatorVariableNodeDisplayer : WireNodeDisplayer
    {
        public AnimatorVariableNodeField VariableField { get; private set; }
        public ActorIdentifierNodeField ActorField { get; private set; }

        private SetAnimatorVariableNodeDisplayer(SetAnimatorVariableWireNode node) : base(node)
        {
            VariableField = new AnimatorVariableNodeField(this, node.Variable)
            {
                FieldName = "Value"
            };
            VariableField.FieldValue = node.Variable;
            ActorField = new ActorIdentifierNodeField(this)
            {
                FieldName = "Target"
            };
            ActorField.FieldValue = node.TargetActor;
        }

        protected override void DoCustomDrawing()
        {
            VariableField.Draw();
            GetRenderedNodeAs<SetAnimatorVariableWireNode>().Variable = VariableField.FieldValue;

            ActorField.Draw();
            GetRenderedNodeAs<SetAnimatorVariableWireNode>().TargetActor = ActorField.FieldValue;
        }

        public static SetAnimatorVariableNodeDisplayer CreateDisplayerFor(SetAnimatorVariableWireNode node)
        {
            SetAnimatorVariableNodeDisplayer nodeRenderer = new SetAnimatorVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 130)
            };
            return nodeRenderer;
        }

        public static SetAnimatorVariableNodeDisplayer CreateSetAnimatorVariableNodeDisplayer(Vector2 position)
        {
            SetAnimatorVariableWireNode node = new SetAnimatorVariableWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Set animator variable",
                Variable = new Core.Variables.AnimatorVariable()
            };
            DialogEditor.InitializeNode(ref node);

            SetAnimatorVariableNodeDisplayer nodeRenderer = new SetAnimatorVariableNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 130)
            };
            return nodeRenderer;
        }
    }
}