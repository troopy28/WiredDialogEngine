using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    [Serializable]
    public class DialogBeginningWireNode : ExecutionWireNode, IDelayable
    {
        public float Delay { get; set; }

        public DialogBeginningWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public DialogBeginningWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
            Inputs.Clear();
        }

        public float GetDelay()
        {
            return Delay;
        }

        public override ExecutionWireNode GetNextExecutionNode()
        {
            OutputWirePin output = Outputs[0];
            InputWirePin connectedPin = output.GetConnectedPin() as InputWirePin;
            WireNode connectedPinOwner = connectedPin.GetOwner();
            if (!output.IsConnected || connectedPin == null)
                return null;
            return connectedPinOwner as ExecutionWireNode;
        }
    }
}