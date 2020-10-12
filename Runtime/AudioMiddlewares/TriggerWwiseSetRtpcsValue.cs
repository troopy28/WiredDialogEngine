#if WDE_USE_WWISE

using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares
{
    [AddComponentMenu("Wired Dialog Engine/Wwise Integration/Set RTPC(s) value(s)")]
    public class TriggerWwiseSetRtpcsValue : WwiseCustomTrigger
    {
        public override void Trigger()
        {
            foreach (KeyValuePair<string, string> paramsKvp in parameters)
            {
                float param;
                if (float.TryParse(paramsKvp.Value, out param))
                    AkSoundEngine.SetRTPCValue(paramsKvp.Key, param);
                else
                    Debug.Log("Unable to cast the parameter \"" + paramsKvp.Key + "\" of the trigger \"" + gameObject.name + "\" into a float. The value of the parameter is" +
                        " \"" + paramsKvp.Value + "\"");
            }
            base.Trigger();
        }
    }
}

#endif