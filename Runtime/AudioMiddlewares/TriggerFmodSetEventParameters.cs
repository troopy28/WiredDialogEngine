#if WDE_USE_FMOD

using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    [AddComponentMenu("Wired Dialog Engine/FMOD Integration/Set event parameters")]
    public class TriggerFmodSetEventParameters : DialogTrigger
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

        /// <summary>
        /// Applies all the parameters of this trigger to the parameters of the specified event emitter.
        /// </summary>
        public override void Trigger()
        {
            foreach (KeyValuePair<string, string> paramsKvp in parameters)
            {
                float param;
                if (float.TryParse(paramsKvp.Value, out param))
                    targetEmitter.SetParameter(paramsKvp.Key, param);
                else
                    Debug.Log("Unable to cast the parameter \"" + paramsKvp.Key + "\" of the trigger \"" + gameObject.name + "\" into a float. The value of the parameter is" +
                        " \"" + paramsKvp.Value + "\"");
            }
            base.Trigger();
        }
    }
}

#endif