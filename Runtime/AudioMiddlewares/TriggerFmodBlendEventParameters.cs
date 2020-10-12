#if WDE_USE_FMOD

using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    [AddComponentMenu("Wired Dialog Engine/FMOD Integration/Blend event parameters")]
    public class TriggerFmodBlendEventParameters : DialogTrigger
    {
        [Tooltip("The StudioEventEmitter that will start or stop playing when the trigger function is called.")]
        [SerializeField]
        protected StudioEventEmitter targetEmitter;
        /// <summary>
        /// The <see cref="StudioEventEmitter"/> that will start or stop playing when the trigger function is called.
        /// </summary>
        public StudioEventEmitter TargetEmitter
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

        [Tooltip("The time it will take to change the parameters from their current state to their target state.")]
        [SerializeField]
        protected float blendingTime;
        /// <summary>
        /// The time it will take to change the parameters from their current state to their target state.
        /// </summary>
        public float BlendingTime
        {
            get
            {
                return blendingTime;
            }
            set
            {
                blendingTime = value;
            }
        }

        /// <summary>
        /// The time between two blending steps.
        /// </summary>
        [Tooltip("The time between two blending steps.")]
        public float BlendingPrecision;

        /// <summary>
        /// A reference to the coroutine that is currently blending your parameters, if the Trigger method has been called.
        /// Null when not blending.
        /// </summary>
        public Coroutine BlendingCoroutine { get; private set; }

        [Tooltip("The curve used to interpolate the default parameters with the trigger parameters. Lower = default parameter stronger," +
            "higher = trigger parameter stronger.")]
        [SerializeField]
        private AnimationCurve blendingCurve;
        /// <summary>
        /// The curve used to interpolate the default parameters with the trigger parameters.
        /// </summary>
        public AnimationCurve BlendingCurve
        {
            get
            {
                return blendingCurve;
            }
            set
            {
                blendingCurve = value;
            }
        }

        [Tooltip("The default parameters on your trigger.")]
        [SerializeField]
        private List<ParamRef> defaultParams;
        public List<ParamRef> DefaultParams
        {
            get
            {
                return defaultParams;
            }
            set
            {
                defaultParams = value;
            }
        }

        [Tooltip("Check it to apply the exact trigger parameters at the end of the blending.")]
        [SerializeField]
        private bool applyLastValue;
        public bool ApplyLastValue
        {
            get
            {
                return applyLastValue;
            }

            set
            {
                applyLastValue = value;
            }
        }

        /// <summary>
        /// Blend all the default parameters (defaultParams field) specified in this trigger with the parameters of this trigger,
        /// during the specified blendingTime, and with the specified blending precision. The interpolation between the two
        /// parameters is done using the specified blending curve (<see cref="AnimationCurve"/>). It first checks that all the
        /// parameters of this trigger can be cast to float.
        /// </summary>
        public override void Trigger()
        {
            foreach (KeyValuePair<string, string> paramsKvp in parameters)
            {
                float param;
                if (!float.TryParse(paramsKvp.Value, out param))
                {
                    Debug.Log("Unable to cast the parameter \"" + paramsKvp.Key + "\" of the trigger \"" + gameObject.name + "\" into a float. The value of the parameter is" +
                        " \"" + paramsKvp.Value + "\". The blending won't be done.");
                    return;
                }
            }

            BlendingCoroutine = StartCoroutine(BlendingProcedure());
            base.Trigger();
        }

        protected IEnumerator BlendingProcedure()
        {
            Debug.Log("Start blending...");
            ParamRef[] previousParams = new ParamRef[defaultParams.Count];
            defaultParams.CopyTo(previousParams); // Copy it to create new references

            for (float i = 0; i < blendingTime;)
            {
                float currentBlend = i / blendingTime; // The x position in the blending curve
                foreach (ParamRef paramRef in previousParams)
                {
                    float targetValue = float.Parse(parameters[paramRef.Name]); // The value we'll have at the end
                    float startValue = paramRef.Value; // The value we'd at the beginning
                    float currentValue = Mathf.Lerp(startValue, targetValue, blendingCurve.Evaluate(currentBlend)); // The value we want now (linear interpolation)

                    targetEmitter.SetParameter(paramRef.Name, currentValue);
                    Debug.Log(paramRef.Name + " : " + currentValue);
                }

                i += BlendingPrecision;
                yield return new WaitForSeconds(BlendingPrecision);
            }

            if(applyLastValue)
            {
                foreach (ParamRef paramRef in previousParams)
                {
                    float targetValue = float.Parse(parameters[paramRef.Name]); // The value we'll have at the end
                    targetEmitter.SetParameter(paramRef.Name, targetValue);
                    Debug.Log(paramRef.Name + " : " + targetValue);
                }
            }
            BlendingCoroutine = null; // Reset the reference
        }
    }
}

#endif