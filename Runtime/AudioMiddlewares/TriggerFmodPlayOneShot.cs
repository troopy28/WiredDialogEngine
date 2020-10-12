#if WDE_USE_FMOD

using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    [AddComponentMenu("Wired Dialog Engine/FMOD Integration/Trigger event one shot")]
    public class TriggerFmodPlayOneShot : DialogTrigger
    {
        [Tooltip("The path of the FMOD event to trigger.")]
        [SerializeField]
        protected string targetEvent;
        public string TargetEvent
        {
            get
            {
                return targetEvent;
            }
            set
            {
                targetEvent = value;
            }
        }

        [Tooltip("Check this to play the event at a specific position. THE DEFAULT POSITION IS THE POSITION OF THIS GAME OBJECT.")]
        [SerializeField]
        protected bool useCustomLocation = true;
        public bool UseCustomLocation
        {
            get
            {
                return useCustomLocation;
            }
            set
            {
                useCustomLocation = value;
            }
        }

        [Tooltip("The custom location where you want to play the event. You need to check the useCustomLocation flag to use this parameter.")]
        [SerializeField]
        protected Vector3 customLocation;
        public Vector3 CustomLocation
        {
            get
            {
                return customLocation;
            }
            set
            {
                customLocation = value;
            }
        }

        /// <summary>
        /// Triggers the specified FMOD event.
        /// </summary>
        public override void Trigger()
        {
            if (useCustomLocation)
                FMODUnity.RuntimeManager.PlayOneShot(targetEvent, customLocation);
            else
                FMODUnity.RuntimeManager.PlayOneShot(targetEvent, transform.position);
            base.Trigger();
        }
    }
}

#endif