using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Triggers;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public static class NodeDisplayersFactory
    {
        public static EqualityNodeDisplayer CreateDisplayer(this EqualityWireNode node)
        {
            return EqualityNodeDisplayer.CreateDisplayerFor(node);
        }

        public static GreaterNodeDisplayer CreateDisplayer(this GreaterWireNode node)
        {
            return GreaterNodeDisplayer.CreatDisplayerFor(node);
        }

        public static InequalityNodeDisplayer CreateDisplayer(this InequalityWireNode node)
        {
            return InequalityNodeDisplayer.CreateDisplayerFor(node);
        }

        public static LesserNodeDisplayer CreateDisplayer(this LesserWireNode node)
        {
            return LesserNodeDisplayer.CreateDisplayerFor(node);
        }

        public static ConstantNodeDisplayer CreateDisplayer(this ConstantWireNode node)
        {
            return ConstantNodeDisplayer.CreateDisplayerFor(node);
        }

        public static BranchNodeDisplayer CreateDisplayer(this BranchWireNode node)
        {
            return BranchNodeDisplayer.CreateDisplayerFor(node);
        }

        public static DialogBeginningNodeDisplayer CreateDisplayer(this DialogBeginningWireNode node)
        {
            return DialogBeginningNodeDisplayer.CreateDisplayerFor(node);
        }

        public static SayReplyNodeDisplayer CreateDisplayer(this SayReplyWireNode node)
        {
            return SayReplyNodeDisplayer.CreateDisplayerFor(node);
        }

        public static SetAnimatorVariableNodeDisplayer CreateDisplayer(this SetAnimatorVariableWireNode node)
        {
            return SetAnimatorVariableNodeDisplayer.CreateDisplayerFor(node);
        }

        public static GetAnimatorVariableNodeDisplayer CreateDisplayer(this GetAnimatorVariableWireNode node)
        {
            return GetAnimatorVariableNodeDisplayer.CreateDisplayerFor(node);
        }

        public static ReunionNodeDisplayer CreateDisplayer(this ReunionWireNode node)
        {
            return ReunionNodeDisplayer.CreateDisplayerFor(node);
        }

        public static GetVariableNodeDisplayer CreateDisplayer(this GetVariableWireNode node)
        {
            return GetVariableNodeDisplayer.CreateDisplayerFor(node);
        }

        public static SetVariableValNodeDisplayer CreateDisplayer(this SetVariableValWireNode node)
        {
            return SetVariableValNodeDisplayer.CreateDisplayerFor(node);
        }

        public static ChooseReplyNodeDisplayer CreateDisplayer(this ChooseReplyWireNode node)
        {
            return ChooseReplyNodeDisplayer.CreateDisplayerFor(node);
        }

        public static TriggerScriptNodeDisplayer CreateDisplayer(this TriggerScriptWireNode node)
        {
            return TriggerScriptNodeDisplayer.CreateDisplayerFor(node);
        }

        public static SetTriggerParamValueNodeDisplayer CreateDisplayer(this SetTriggerParamValueWireNode node)
        {
            return SetTriggerParamValueNodeDisplayer.CreateDisplayerFor(node);
        }
    }
}