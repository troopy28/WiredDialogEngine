using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class BranchNodeDisplayer : WireNodeDisplayer
    {

        private BranchNodeDisplayer(BranchWireNode node) : base(node)
        {

        }

        protected override void DoCustomDrawing()
        {

        }

        public static BranchNodeDisplayer CreateDisplayerFor(BranchWireNode node)
        {
            BranchNodeDisplayer nodeRenderer = new BranchNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 220, 70)
            };
            return nodeRenderer;
        }

        public static BranchNodeDisplayer CreateBranchDisplayer(Vector2 position)
        {
            BranchWireNode node = new BranchWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Branch"
            };
            DialogEditor.InitializeNode(ref node);

            node.FalsePin = new OutputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "False",
                DataType = WDEngine.ActivityStream
            };
            node.Outputs[0].PinName = "True";
            node.TruePin = node.Outputs[0];
            node.Outputs.Add(node.FalsePin);

            node.Inputs.Add(new InputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "Condition",
                DataType = typeof(bool)
            });

            BranchNodeDisplayer nodeRenderer = new BranchNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 220, 70)
            };
            return nodeRenderer;
        }
    }
}