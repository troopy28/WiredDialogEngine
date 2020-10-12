using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Runtime;
using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data
{
    [Serializable]
    public class GetVariableWireNode : WireNode, ILoadable
    {
        public string VariableName;
        /// <summary>
        /// The variable that has been retrieved by the <see cref="Load"/> method. Null if <see cref="Load"/> hasn't been called before.
        /// </summary>
        public Variable RetrievedVariable { get; private set; }

        public GetVariableWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public GetVariableWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public float GetVariableAsFloat()
        {
            if (RetrievedVariable.Equals(null))
                Load();
            return RetrievedVariable.GetValueAs<float>();
        }

        public int GetVariableAsInteger()
        {
            if (RetrievedVariable.Equals(null))
                Load();
            return RetrievedVariable.GetValueAs<int>();
        }

        public string GetVariableAsString()
        {
            if (RetrievedVariable.Equals(null))
                Load();
            return RetrievedVariable.GetValueAs<string>();
        }

        public void Load()
        {
            if (DialogEngineRuntime.IsRuntime)
            {
                RetrievedVariable = DialogEngineRuntime.GetCurrentVariablesContainer().FindVariable(VariableName);
                RetrievedVariable.Load(true);
            }
        }

        public void Unload()
        {
            RetrievedVariable.Save(DialogEngineRuntime.IsRuntime);
            RetrievedVariable.Unload();
        }
    }
}