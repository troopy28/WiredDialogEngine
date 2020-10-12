using Assets.WiredTools.WiredDialogEngine.Core.Replies;
using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation
{
    public struct InterpretationResult
    {
        public InterpretationResultType ResultType;

        public float Delay;

        public Reply Reply;
        public ActorIdentifier Target;
        public Choice Choice;    
    }

    public enum InterpretationResultType
    {
        UNDEFINED,
        SAY_REPLICA,
        CHOICE,
        FLOW_NODE,
        END_OF_DIALOG
    }
}