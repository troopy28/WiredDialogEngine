using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers
{
    [Serializable]
    public class SetTriggerParamValueWireNode : ExecutionWireNode, IDelayable, IProcessable
    {
        /// <summary>
        /// The game object holding the trigger.
        /// </summary>
        public string TargetGameObjectName { get; set; }

        /// <summary>
        /// The name of the trigger you want to use.
        /// </summary>
        public string TriggerName { get; set; }

        /// <summary>
        /// The delay to wait after the node has been interpreted, in seconds.
        /// </summary>
        public float Delay;

        /// <summary>
        /// The name of the trigger parameter to change.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// The value the trigger parameter will take.
        /// </summary>
        public string ParameterValue { get; set; }

        public SetTriggerParamValueWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public SetTriggerParamValueWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public float GetDelay()
        {
            return Delay;
        }

        public void Process()
        {
            IEnumerable<IDialogTrigger> components = GameObject.Find(TargetGameObjectName).GetComponents<IDialogTrigger>().Where(comp => comp.TriggerName == TriggerName);
            if (components.Count() > 0)
            {
                IDialogTrigger trigger = components.ToList()[0];
                if (trigger.Parameters.ContainsKey(ParameterName))
                    trigger.Parameters[ParameterName] = ParameterValue;
                else
                    trigger.Parameters.Add(ParameterName, ParameterValue);
            }
        }
    }
}