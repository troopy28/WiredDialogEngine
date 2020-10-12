using System.Collections.Generic;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interaction
{
    public interface IDialogTrigger
    {
        Dictionary<string, string> Parameters { get; set; }
        string TriggerName { get; set; }
        void Trigger();
    }
}