namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes
{
    /// <summary>
    /// <see cref="IDelayable"/> nodes have a <see cref="GetDelay"/> function that enable the dialog interpreter
    /// to ask the delay to wait before going to the next node. If a node implements <see cref="IDelayable"/> and 
    /// <see cref="ILoadable"/>, the <see cref="ILoadable.Load"/> function will be called BEFORE the <see cref="GetDelay"/>
    /// function, so the <see cref="GetDelay"/> function can use some node's data that requires the loading of the
    /// node to do its stuff.
    /// </summary>
    public interface IDelayable
    {
        float GetDelay();
    }
}