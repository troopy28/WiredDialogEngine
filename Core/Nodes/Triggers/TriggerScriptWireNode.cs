using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers
{
    [Serializable]
    public class TriggerScriptWireNode : ExecutionWireNode, IDelayable, IProcessable
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

        public TriggerScriptWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public TriggerScriptWireNode(WireDialog associatedDialog) : base(associatedDialog)
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
                components.ToList()[0].Trigger();
        }
    }
}