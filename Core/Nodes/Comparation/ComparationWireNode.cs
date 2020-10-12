using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation
{
    [Serializable]
    public abstract class ComparationWireNode : WireNode
    {
        protected ComparationWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        protected ComparationWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        /// <summary>
        /// Returns the result of the comparation.
        /// </summary>
        /// <returns>Returns the result of the comparation.</returns>
        public abstract bool GetResult();
    }
}