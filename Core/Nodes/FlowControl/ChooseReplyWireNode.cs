using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl
{
    [Serializable]
    public class ChooseReplyWireNode : ExecutionWireNode
    {
        public int ChosenOutput { get; set; }

        public ChooseReplyWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public override ExecutionWireNode GetNextExecutionNode()
        {
            return Outputs[ChosenOutput].GetConnectedPin().GetOwner() as ExecutionWireNode;
        }
    }
}