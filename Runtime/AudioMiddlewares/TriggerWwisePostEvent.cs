#if WDE_USE_WWISE

using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    [AddComponentMenu("Wired Dialog Engine/Wwise Integration/Post event")]
    public class TriggerWwisePostEvent : DialogTrigger
    {
        [Tooltip("The name (in Wwise) of the event to trigger.")]
        [SerializeField]
        private string eventName;
        /// <summary>
        /// The name (in Wwise) of the event to trigger.
        /// </summary>
        public string EventName
        {
            get
            {
                return eventName;
            }

            set
            {
                eventName = value;
            }
        }

        [Tooltip("The game object associated to this event. Its position will be the origin of the sound.")]
        [SerializeField]
        private GameObject gameObjectId;
        /// <summary>
        /// The game object associated to this event. Its position will be the origin of the sound.
        /// </summary>
        public GameObject GameObjectId
        {
            get
            {
                return gameObjectId;
            }

            set
            {
                gameObjectId = value;
            }
        }

        public override void Trigger()
        {
            AkSoundEngine.PostEvent(eventName, gameObjectId);
            base.Trigger();
        }
    }
}

#endif