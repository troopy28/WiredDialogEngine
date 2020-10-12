namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    /// <summary>
    /// The nodes that implement <see cref="IProcessable"/> have a <see cref="Process"/> function called when the node is interpreted.
    /// It enables them to do some specific work before going to the other node.
    /// </summary>
    public interface IProcessable
    {
        /// <summary>
        /// Called when the node is interpreted.
        /// </summary>
        void Process();
    }
}