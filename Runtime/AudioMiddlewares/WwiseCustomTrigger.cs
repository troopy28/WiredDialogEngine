#if WDE_USE_WWISE

using System.Collections.Generic;
using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    public abstract class WwiseCustomTrigger : AkTriggerBase, IDialogTrigger
    {
        public delegate void TriggerEvent();
        public event TriggerEvent OnTriggered;

        protected Dictionary<string, string> parameters;
        public Dictionary<string, string> Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        [Tooltip("The name of the trigger. It is used to distinguish the different dialog triggers of a single game object, thus" +
            "enabling you to have several triggers on the same game object.")]
        [SerializeField]
        protected string triggerName;
        /// <summary>
        /// The name of the trigger. It is used to distinguish the different dialog triggers of a single game object, thus
        /// enabling you to have several triggers on the same game object.
        /// </summary>
        public string TriggerName
        {
            get
            {
                return triggerName;
            }
            set
            {
                triggerName = value;
            }
        }

        public void Awake()
        {
            parameters = new Dictionary<string, string>();
        }

        public virtual new void Trigger()
        {
            if (OnTriggered != null)
                OnTriggered.Invoke();
        }
    }
}

#endif