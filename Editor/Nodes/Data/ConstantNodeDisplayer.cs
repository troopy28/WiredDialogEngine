using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Data
{
    public class ConstantNodeDisplayer : WireNodeDisplayer
    {
        public ConstantNodeField ConstantField { get; private set; }

        private ConstantNodeDisplayer(ConstantWireNode node) : base(node)
        {
            ConstantField = new ConstantNodeField(this, node.Constant)
            {
                FieldName = "Value"
            };
        }

        protected override void DoCustomDrawing()
        {
            ConstantField.Draw();
            GetRenderedNodeAs<ConstantWireNode>().Constant = ConstantField.FieldValue;
        }

        public static ConstantNodeDisplayer CreateDisplayerFor(ConstantWireNode node)
        {
            ConstantNodeDisplayer nodeRenderer = new ConstantNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 220, 90)
            };
            return nodeRenderer;
        }

        public static ConstantNodeDisplayer CreateConstantNodeDisplayer(Vector2 position)
        {
            ConstantWireNode node = new ConstantWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Constant",
                Constant = new Constant()
            };
            DialogEditor.InitializeNode(ref node);

            node.Outputs.Add(new OutputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                DataType = typeof(Variable),
                PinName = "Variable"
            });

            ConstantNodeDisplayer nodeRenderer = new ConstantNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 220, 90)
            };
            return nodeRenderer;
        }
    }
}