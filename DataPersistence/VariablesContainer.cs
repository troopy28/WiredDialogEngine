using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.DataPersistence
{
    [Serializable]
    public class VariablesContainer
    {
        [JsonProperty]
        [SerializeField]
        private List<Variable> independantVariables;
        /// <summary>
        /// The list of the variables that are registered into this variable container. The variables registered
        /// can be saved and retrieved from the disk using the <see cref="FindVariable(string)"/> and the 
        /// <see cref="DialogFilesManager.SaveVariables(VariablesContainer)"/> functions.
        /// </summary>
        [JsonIgnore]
        public List<Variable> IndependantVariables
        {
            get
            {
                return independantVariables;
            }
            set
            {
                independantVariables = value;
            }
        }

        /// <summary>
        /// Returns the specified variable by its name. Can be slow if there are many variables.
        /// Try to use it the least possible (store the variables after have found them).
        /// </summary>
        /// <param name="variableName">The name of the varible you want to get.</param>
        /// <returns>Returns the specified variable by its name.</returns>
        public Variable FindVariable(string variableName)
        {
            foreach (Variable curr in independantVariables)
                if (curr.VariableName.Equals(variableName))
                    return curr;
            return null;
        }
    }
}