using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation
{
    [Serializable]
    public class InequalityWireNode : ComparationWireNode
    {
        public InputWirePin A;
        public InputWirePin B;

        public InequalityWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public InequalityWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public override bool GetResult()
        {
            A = Inputs[0];
            B = Inputs[1];
            WireNode aOwner = A.GetConnectedPin().GetOwner();
            WireNode bOwner = B.GetConnectedPin().GetOwner();
            if (aOwner is GetVariableWireNode)
            {
                GetVariableWireNode varNode = (GetVariableWireNode)aOwner;
                if (bOwner is GetVariableWireNode)
                    return !varNode.RetrievedVariable.Equals((bOwner as GetVariableWireNode).RetrievedVariable);
                else if (bOwner is ConstantWireNode)
                    return !varNode.RetrievedVariable.Equals((bOwner as ConstantWireNode).Constant);
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode animVar = (GetAnimatorVariableWireNode)bOwner;
                    switch (varNode.RetrievedVariable.Type)
                    {
                        case VariableType.FLOAT:
                            return varNode.RetrievedVariable.GetValueAs<float>() != animVar.GetVariableAsFloat();
                        case VariableType.INT:
                            return varNode.RetrievedVariable.GetValueAs<int>() != animVar.GetVariableAsInteger();
                        case VariableType.STRING:
                            return true;
                    }
                }
            }
            else if (aOwner is ConstantWireNode)
            {
                ConstantWireNode constNode = (ConstantWireNode)aOwner;

                if (bOwner is GetVariableWireNode) // To compare a variable and a constant, use the equal method of the variable class
                    return !((GetVariableWireNode)bOwner).RetrievedVariable.Equals(constNode.Constant);
                else if (bOwner is ConstantWireNode) // To compare two constants, just compare their raw value
                    return !constNode.Constant.Value.Equals((bOwner as ConstantWireNode).Constant.Value);
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode animVar = (GetAnimatorVariableWireNode)bOwner;
                    switch (constNode.Constant.Type)
                    {
                        case VariableType.FLOAT:
                            return ((float)constNode.Constant.Value) != animVar.GetVariableAsFloat();
                        case VariableType.INT:
                            return ((int)constNode.Constant.Value) != animVar.GetVariableAsInteger();
                        case VariableType.STRING:
                            return true;
                    }
                }
            }
            else if (aOwner is GetAnimatorVariableWireNode)
            {
                GetAnimatorVariableWireNode animVarNode = (GetAnimatorVariableWireNode)aOwner;
                if (bOwner is GetVariableWireNode) // To compare a variable and a animator variable, use the equal method of the variable class
                {
                    Variable myVar = (bOwner as GetVariableWireNode).RetrievedVariable;
                    switch (myVar.Type)
                    {
                        case VariableType.FLOAT:
                            return !myVar.Equals(animVarNode.GetVariableAsFloat());
                        case VariableType.INT:
                            return !myVar.Equals(animVarNode.GetVariableAsInteger());
                        case VariableType.STRING:
                            return true;
                    }
                }
                else if (bOwner is ConstantWireNode) // To compare two constants, just compare their raw value
                {
                    Constant myConst = (bOwner as ConstantWireNode).Constant;
                    switch (myConst.Type)
                    {
                        case VariableType.FLOAT:
                            return myConst.GetValueAs<float>() != animVarNode.GetVariableAsFloat();
                        case VariableType.INT:
                            return myConst.GetValueAs<int>() != animVarNode.GetVariableAsInteger();
                        case VariableType.STRING:
                            return false;
                    }
                }
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode other = (GetAnimatorVariableWireNode)bOwner;
                    try
                    {
                        return !(other.GetVariableAsFloat() == animVarNode.GetVariableAsFloat()
                            || other.GetVariableAsInteger() == animVarNode.GetVariableAsInteger()
                            || other.GetVariableAsBool() == animVarNode.GetVariableAsBool());
                    }
                    catch (Exception)
                    {
                        return !(other.GetVariableAsBool() == animVarNode.GetVariableAsBool());
                    }
                }
            }

            return true;
        }
    }
}