using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation
{
    [Serializable]
    public class GetAnimatorVariableWireNode : WireNode, ILoadable
    {
        public string VariableName { get; set; }
        [JsonIgnore]
        public ActorIdentifier TargetActor { get; set; }
        public string TargetResourcePath { get; set; }

        public GetAnimatorVariableWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public GetAnimatorVariableWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public bool GetVariableAsBool()
        {
            Animator animator;
            if (TargetActor.GetActorAnimator(out animator))
            {
                return animator.GetBool(VariableName);
            }
            throw new Exception("Error while playing the dialog: you are trying to get the value of an animator parameter, " +
                "but the actor can't access to it's animator.");
        }

        public float GetVariableAsFloat()
        {
            Animator animator;
            if (TargetActor.GetActorAnimator(out animator))
            {
                return animator.GetFloat(VariableName);
            }
            throw new Exception("Error while playing the dialog: you are trying to get the value of an animator parameter, " +
                "but the actor can't access to it's animator.");
        }

        public int GetVariableAsInteger()
        {
            Animator animator;
            if(TargetActor.GetActorAnimator(out animator))
            {
                return animator.GetInteger(VariableName);
            }
            throw new Exception("Error while playing the dialog: you are trying to get the value of an animator parameter, " +
                "but the actor can't access to it's animator.");
        }

        public void Load()
        {
            TargetActor = Resources.Load<ActorIdentifier>(TargetResourcePath);
        }

        public void Unload()
        {
            Resources.UnloadAsset(TargetActor);
        }
    }
}