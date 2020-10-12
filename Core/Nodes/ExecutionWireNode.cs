using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    /// <summary>
    /// An <see cref="ExecutionWireNode"/> node is a <see cref="WireNode"/> that has an input activity and an output activity,
    /// and thus a node that is a part of the execution (or activity) flow.
    /// </summary>
    [Serializable]
    public abstract class ExecutionWireNode : WireNode
    {
        protected ExecutionWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        protected ExecutionWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        /// <summary>
        /// Returns the <see cref="ExecutionWireNode"/> connected to the output pin of this <see cref="ExecutionWireNode"/>. If
        /// the pin isn't connected to anything, returns null.
        /// </summary>
        /// <returns>Returns the <see cref="ExecutionWireNode"/> connected to the output pin of this <see cref="ExecutionWireNode"/>. If
        /// the pin isn't connected to anything, returns null.</returns>
        public virtual ExecutionWireNode GetNextExecutionNode()
        {
            if (!Outputs[0].IsConnected || Outputs[0].GetConnectedPin() == null)
                return null;
            return Outputs[0].GetConnectedPin().GetOwner() as ExecutionWireNode;
        }
    }
}