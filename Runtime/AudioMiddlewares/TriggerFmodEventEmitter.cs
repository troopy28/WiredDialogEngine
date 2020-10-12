#if WDE_USE_FMOD

using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    /// <summary>
    /// A class that triggers the specified <see cref="FMODUnity.StudioEventEmitter"/> (FMOD plugin required).
    /// </summary>
    [AddComponentMenu("Wired Dialog Engine/FMOD Integration/Trigger event emitter")]
    public class TriggerFmodEventEmitter : DialogTrigger
    {
        [Tooltip("The StudioEventEmitter that will start or stop playing when the trigger function is called.")]
        [SerializeField]
        protected FMODUnity.StudioEventEmitter targetEmitter;
        /// <summary>
        /// The <see cref="FMODUnity.StudioEventEmitter"/> that will start or stop playing when the trigger function is called.
        /// </summary>
        public FMODUnity.StudioEventEmitter TargetEmitter
        {
            get
            {
                return targetEmitter;
            }
            set
            {
                targetEmitter = value;
            }
        }

        [SerializeField]
        protected EventEmitterTriggerAction action;
        /// <summary>
        /// The action to do when triggered. Play, or stop the target <see cref="FMODUnity.StudioEventEmitter"/>?
        /// </summary>
        public EventEmitterTriggerAction Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        /// <summary>
        /// Starts the target emitter to play, and calls the base method, thus invoking the OnTriggered event.
        /// </summary>
        public override void Trigger()
        {
            // A way to start (or stop) the EventEmitter playing. Although the official documentation has an example using the sendMessage("Play")
            // method, calling directly the Play() function is way better, because we avoid using Reflection.
            // See also: https://www.fmod.org/docs/content/generated/engine_new_unity/emitter.html
            if (action == EventEmitterTriggerAction.PLAY)
                targetEmitter.Play();
            else
                targetEmitter.Stop();
            base.Trigger();
        }
    }

    /// <summary>
    /// The action to do: start playing, or stopping a <see cref="FMODUnity.StudioEventEmitter"/>.
    /// </summary>
    public enum EventEmitterTriggerAction
    {
        PLAY,
        STOP
    }
}

#endif