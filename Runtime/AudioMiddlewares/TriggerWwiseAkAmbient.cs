#if WDE_USE_WWISE

using Assets.WiredTools.WiredDialogEngine.Runtime.AudioMiddlewares;
using UnityEngine;

[AddComponentMenu("Wired Dialog Engine/Wwise Integration/Trigger AkAmbient")]
public class TriggerWwiseAkAmbient : WwiseCustomTrigger
{
    [Tooltip("The StudioEventEmitter that will start or stop playing when the trigger function is called.")]
    [SerializeField]
    protected AkAmbient targetAkAmbient;
    /// <summary>
    /// The <see cref="FMODUnity.StudioEventEmitter"/> that will start or stop playing when the trigger function is called.
    /// </summary>
    public AkAmbient TargetAkAmbient
    {
        get
        {
            return targetAkAmbient;
        }
        set
        {
            targetAkAmbient = value;
        }
    }

    [Tooltip("The action to do with the AkAmbient: play, or stop.")]
    [SerializeField]
    private AkAmbientAction action;
    public AkAmbientAction Action
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
        if (action == AkAmbientAction.PLAY)
        {
            if (triggerDelegate != null)
            {
                triggerDelegate(null);
            }
        }
        else
        {
            targetAkAmbient.Stop(0);
        }
        base.Trigger();
        
    }
}

public enum AkAmbientAction
{
    PLAY,
    STOP
}

#endif