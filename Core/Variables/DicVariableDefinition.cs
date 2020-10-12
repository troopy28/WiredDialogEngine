using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Variables
{
    [Serializable]
    public class DicVariableDefinition
    {
        [Tooltip("The variable name.")]
        [SerializeField]
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [Tooltip("The variable iself.")]
        [SerializeField]
        private Variable variable;
        public Variable Variable
        {
            get
            {
                return variable;
            }
            set
            {
                variable = value;
            }
        }
    }
}