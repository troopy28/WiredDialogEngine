using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Runtime;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data
{
    public class SetVariableValWireNode : ExecutionWireNode, ILoadable, IProcessable
    {
        public string VariableName;
        /// <summary>
        /// The variable that has been retrieved by the <see cref="Load"/> method. Null if <see cref="Load"/> hasn't been called before.
        /// </summary>
        public Variable RetrievedVariable { get; private set; }
        public string UpdatedValue { get; set; }

        public SetVariableValWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public SetVariableValWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public void Load()
        {
            if (DialogEngineRuntime.IsRuntime)
            {
                RetrievedVariable = DialogEngineRuntime.GetCurrentVariablesContainer().FindVariable(VariableName);
                if (object.Equals(RetrievedVariable, null))
                {
                    Debug.LogError("Unable to retrieve the variable \"" + VariableName + "\". Make sure it is in the list " +
                        "of the variables in the DialogEngineRuntime.");
                    return;
                }
                RetrievedVariable.Load(true);
            }
        }

        public void Unload()
        {
            RetrievedVariable.Save(true);
            RetrievedVariable.Unload();
        }

        public void Process()
        {
#if ENABLE_DEBUG_DIALOG
            Debug.Log("Processing an update variable node------");
            Debug.Log("Previous value: " + RetrievedVariable.Value);
#endif
            RetrievedVariable.SetValueAtRuntime(UpdatedValue);
#if ENABLE_DEBUG_DIALOG
            Debug.Log("New value: " + RetrievedVariable.Value);
            Debug.Log("----------------------------------------");
#endif
        }
    }
}