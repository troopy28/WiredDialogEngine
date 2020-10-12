using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Variables;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation
{
    [Serializable]
    public class SetAnimatorVariableWireNode : ExecutionWireNode, ILoadable, IProcessable
    {
        /* 
        * /!\ -- /!\
        * FOR LATER : if the VariableValue cannot be cast at runtime, then it will be useful to have a field for every possible
        *  value type : one float, one int, one bool etc in the Variable class.
        */
        public AnimatorVariable Variable { get; set; }

        [JsonIgnore]
        public ActorIdentifier TargetActor { get; set; }
        public string TargetResourcePath { get; set; }

        public SetAnimatorVariableWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public SetAnimatorVariableWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public void Process()
        {
            Animator animator;
            if (!TargetActor.GetActorAnimator(out animator))
            {
                throw new Exception("Error while playing the dialog: you are trying to set the value of an animator parameter, " +
                "but the actor can't access to it's animator.");
            }
            
            switch (Variable.Type)
            {
                case AnimatorVariableType.BOOL:
                    animator.SetBool(Variable.Name, Variable.GetValueAs<bool>());
                    break;
                case AnimatorVariableType.FLOAT:
                    animator.SetFloat(Variable.Name, Variable.GetValueAs<float>());
                    break;
                case AnimatorVariableType.INT:
                    animator.SetInteger(Variable.Name, Variable.GetValueAs<int>());
                    break;
                case AnimatorVariableType.TRIGGER:
                    animator.SetTrigger(Variable.Name);
                    break;
            }
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