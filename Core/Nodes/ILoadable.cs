namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    /// <summary>
    /// <para>
    /// Loadable nodes have a <see cref="Load"/> method called when the dialog is loaded. It enables
    /// the node to do some initialization stuff before the dialog is interpreted.
    /// </para>
    /// <para>
    /// They also have an <see cref="Unload"/> method called when the dialog is no more interpreted.
    /// This allows the node to release some resources that it is managing.
    /// </para>
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// Called when the dialog is loaded.
        /// </summary>
        void Load();
        /// <summary>
        /// Called when the dialog is no more interpreted.
        /// </summary>
        void Unload();
    }
}