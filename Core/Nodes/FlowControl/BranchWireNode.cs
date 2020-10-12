using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl
{
    [Serializable]
    public class BranchWireNode : ExecutionWireNode
    {
        [JsonIgnore]
        public OutputWirePin TruePin { get; set; }
        [JsonIgnore]
        public OutputWirePin FalsePin { get; set; }

        public BranchWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public override ExecutionWireNode GetNextExecutionNode()
        {
            TruePin = Outputs[0];
            FalsePin = Outputs[1];
            
            bool conditionResult;
            InputWirePin conditionInput = Inputs[1];

            WireNode conditionNode = conditionInput.GetConnectedPin().GetOwner();

            if (conditionNode is ComparationWireNode) // If the condition input is linked to a comparation, then we get the value of this comparation
                conditionResult = (conditionNode as ComparationWireNode).GetResult(); 
            else // Otherwise we just take the input value, which can only be a boolean
                conditionResult = (bool)conditionInput.Value;

            if (conditionResult) // The condition is true
            {
                // Try to return the TRUE pin target's node
                if (TruePin.IsConnected == false || TruePin.GetConnectedPin() == null)
                    return null;
                return TruePin.GetConnectedPin().GetOwner() as ExecutionWireNode;
            }
            // Try to return the FALSE pin target's node
            if (FalsePin.IsConnected == false || FalsePin.GetConnectedPin() == null)
                return null;
            return FalsePin.GetConnectedPin().GetOwner() as ExecutionWireNode;
        }
    }
}