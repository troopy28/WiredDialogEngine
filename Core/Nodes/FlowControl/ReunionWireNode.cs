using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl
{
    [Serializable]
    public class ReunionWireNode : ExecutionWireNode
    {
        public ReunionWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public void AddInput(InputWirePin input)
        {
            Inputs.Add(input);
        }

        public void RemoveInput(InputWirePin input)
        {
            input.Disconnect(true);
            Inputs.Remove(input);
        }

        public void RemoveLastInput()
        {
            int idx = Inputs.Count - 1;
            Inputs[idx].Disconnect(true);
            Inputs.RemoveAt(idx);
        }
    }
}