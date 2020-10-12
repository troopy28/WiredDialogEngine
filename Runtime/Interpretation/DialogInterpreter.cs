using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation
{
    /// <summary>
    /// An example of dialog interpreter. This one works pretty well with the default nodes of the Wired Dialog Engine,
    /// but you can also write your own interpreter to gain more flexibility, and do whatever you want with your nodes.
    /// </summary>
    public class DialogInterpreter
    {
        /// <summary>
        /// The dialog that's being interpreted.
        /// </summary>
        public WireDialog Dialog { get; private set; }
        /// <summary>
        /// Are we at the at the end of a dialog.
        /// </summary>
        public bool EndOfDialog { get; private set; }
        /// <summary>
        /// The execution node that's currently being interpreted.
        /// </summary>
        private ExecutionWireNode currentExecNode;
        private bool atBeginning = true;
        public ExecutionWireNode LastInterpretedNode
        {
            get
            {
                return currentExecNode;
            }
        }
        private bool lastNodeWasChoice;
        public int ChosenReply { get; set; }

        /// <summary>
        /// Define the dialog this interpreter will interpret when you will use the <see cref="InterpretNextNode"/>
        /// function. It then loads the dialog in order to be able to read it.
        /// </summary>
        /// <param name="dialog"></param>
        public void SetWireDialog(WireDialog dialog)
        {
            Dialog = dialog;
            Dialog.Load();
            atBeginning = true;
            EndOfDialog = false;
        }

        public virtual InterpretationResult InterpretNextNode()
        {
            InterpretationResult result = new InterpretationResult()
            {
                ResultType = InterpretationResultType.UNDEFINED
            };

            if (lastNodeWasChoice)
                (currentExecNode as ChooseReplyWireNode).ChosenOutput = ChosenReply;

            lastNodeWasChoice = false;
            if (atBeginning) // If at beginning, start by getting the node connected to the beginning node
            {
                currentExecNode = Dialog.GetBeginning();
                result.ResultType = InterpretationResultType.FLOW_NODE; // The dialog beginning is a flow node
                atBeginning = false;
            }
            else
                currentExecNode = currentExecNode.GetNextExecutionNode();

            if (currentExecNode == null)
            {
                EndOfDialog = true;
                result.ResultType = InterpretationResultType.END_OF_DIALOG;
            }

            if (currentExecNode is IDelayable)
                result.Delay = (currentExecNode as IDelayable).GetDelay();

            if (currentExecNode is IProcessable)
                (currentExecNode as IProcessable).Process();


            // The next node can only be an execution node
            if (currentExecNode is SayReplyWireNode)
            {
                result.ResultType = InterpretationResultType.SAY_REPLICA;
                result.Reply = (currentExecNode as SayReplyWireNode).Reply;
                result.Target = (currentExecNode as SayReplyWireNode).Target;
            }
            else if (currentExecNode is BranchWireNode || currentExecNode is ReunionWireNode || currentExecNode is SetVariableValWireNode 
                || currentExecNode is SetTriggerParamValueWireNode || currentExecNode is TriggerScriptWireNode)
            {
                result.ResultType = InterpretationResultType.FLOW_NODE;
            }
            else if (currentExecNode is ChooseReplyWireNode)
            {
                result.ResultType = InterpretationResultType.CHOICE;
                result.Choice = new Choice();
                (currentExecNode as ChooseReplyWireNode).Outputs.ForEach(output =>
                {
                    SayReplyWireNode owner = output.GetConnectedPin().GetOwner() as SayReplyWireNode;
                    if (owner != null)
                    {
                        result.Choice.Choices.Add(owner.Reply);
                    }
                });
                lastNodeWasChoice = true;
            }

            return result;
        }
    }
}