using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data
{
    [Serializable]
    public class ConstantWireNode : WireNode
    {
        public Constant Constant { get; set; }
        
        public ConstantWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public ConstantWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }
    }
}