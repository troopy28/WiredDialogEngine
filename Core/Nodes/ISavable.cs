namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    /// <summary>
    /// <para>    
    /// Savable nodes have a <see cref="Save"/> method called just before the dialog is saved. It enable
    /// the node to do some saving stuff before the dialog is serialized. This is useful to manage the
    /// serialization of nodes in the editor. This method is NEVER called at runtime.
    /// </para>
    /// <para>
    /// Take care however, not to use the <see cref="UnityEditor"/> namespace because it doesn't exists
    /// at runtime, leading to a compilation error.
    /// </para>
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// Called just before the dialog is saved.
        /// </summary>
        void Save();
    }
}