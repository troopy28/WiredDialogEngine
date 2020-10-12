using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interaction
{
    /// <summary>
    /// The class providing a way to call scripts from the Visual Dialog Scripting.
    /// </summary>
    [AddComponentMenu("Wired Dialog Engine/Dialog trigger")]
    public class DialogTrigger : MonoBehaviour, IDialogTrigger
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

        /// <summary>
        /// <para>
        /// Called by the dialog whenever a TriggerScriptNode is processed, and when the target of this node
        /// is this script. Basically, this method doesn't do anything but calling the <see cref="OnTriggered"/> 
        /// event.
        /// </para>
        /// <para>
        /// This method is virtual, and thus overridable to enable you to create custom dialog triggers.
        /// </para>
        /// </summary>
        public virtual void Trigger()
        {
            if (OnTriggered != null)
                OnTriggered.Invoke();
        }
    }
}