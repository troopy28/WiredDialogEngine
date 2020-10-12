using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation
{
    [Serializable]
    public class LesserWireNode : ComparationWireNode
    {
        [NonSerialized]
        public InputWirePin A;
        [NonSerialized]
        public InputWirePin B;

        public LesserWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public LesserWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {
        }

        public override bool GetResult()
        {
            A = Inputs[0];
            B = Inputs[1];

            WireNode aOwner = A.GetConnectedPin().GetOwner();
            WireNode bOwner = B.GetConnectedPin().GetOwner();
            IComparable va = null;
            IComparable vb = null;

            if (aOwner is ConstantWireNode)
            {
                Constant constantA = ((ConstantWireNode)aOwner).Constant;
                if (constantA.Type == VariableType.FLOAT)
                    va = constantA.GetValueAs<float>();
                if (constantA.Type == VariableType.INT)
                    va = constantA.GetValueAs<int>();

                if (bOwner is ConstantWireNode)
                {
                    Constant constantB = ((ConstantWireNode)bOwner).Constant;
                    if (constantB.Type == VariableType.FLOAT)
                        vb = constantB.GetValueAs<float>();
                    if (constantB.Type == VariableType.INT)
                        vb = constantB.GetValueAs<int>();
                }
                else if (bOwner is GetVariableWireNode)
                {
                    Variable variableB = ((GetVariableWireNode)bOwner).RetrievedVariable;
                    if (variableB.Type == VariableType.FLOAT)
                        vb = variableB.GetValueAs<float>();
                    if (variableB.Type == VariableType.INT)
                        vb = variableB.GetValueAs<int>();
                }
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode bGavwn = (GetAnimatorVariableWireNode)bOwner;
                    try
                    {
                        vb = bGavwn.GetVariableAsFloat();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            vb = bGavwn.GetVariableAsInteger();
                        }
                        catch (Exception) { }
                    }
                }
            }

            else if (aOwner is GetVariableWireNode)
            {
                Variable variableA = ((GetVariableWireNode)aOwner).RetrievedVariable;
                if (variableA.Type == VariableType.FLOAT)
                    va = variableA.GetValueAs<float>();
                if (variableA.Type == VariableType.INT)
                    va = variableA.GetValueAs<int>();
                if (bOwner is ConstantWireNode)
                {
                    Constant constantB = ((ConstantWireNode)bOwner).Constant;
                    if (constantB.Type == VariableType.FLOAT)
                        vb = constantB.GetValueAs<float>();
                    if (constantB.Type == VariableType.INT)
                        vb = constantB.GetValueAs<int>();
                }
                else if (bOwner is GetVariableWireNode)
                {
                    Variable variableB = ((GetVariableWireNode)bOwner).RetrievedVariable;
                    if (variableB.Type == VariableType.FLOAT)
                        vb = variableB.GetValueAs<float>();
                    if (variableB.Type == VariableType.INT)
                        vb = variableB.GetValueAs<int>();
                }
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode bGavwn = (GetAnimatorVariableWireNode)bOwner;
                    try
                    {
                        vb = bGavwn.GetVariableAsFloat();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            vb = bGavwn.GetVariableAsInteger();
                        }
                        catch (Exception) { }
                    }
                }
            }

            else if (aOwner is GetAnimatorVariableWireNode)
            {
                GetAnimatorVariableWireNode aGavwn = (GetAnimatorVariableWireNode)aOwner;
                try
                {
                    va = aGavwn.GetVariableAsFloat();
                }
                catch (Exception)
                {
                    try
                    {
                        va = aGavwn.GetVariableAsInteger();
                    }
                    catch (Exception) { }
                }
                if (bOwner is ConstantWireNode)
                {
                    Constant constantB = ((ConstantWireNode)bOwner).Constant;
                    if (constantB.Type == VariableType.FLOAT)
                        vb = constantB.GetValueAs<float>();
                    if (constantB.Type == VariableType.INT)
                        vb = constantB.GetValueAs<int>();
                }
                else if (bOwner is GetVariableWireNode)
                {
                    Variable variableB = ((GetVariableWireNode)bOwner).RetrievedVariable;
                    if (variableB.Type == VariableType.FLOAT)
                        vb = variableB.GetValueAs<float>();
                    if (variableB.Type == VariableType.INT)
                        vb = variableB.GetValueAs<int>();
                }
                else if (bOwner is GetAnimatorVariableWireNode)
                {
                    GetAnimatorVariableWireNode bGavwn = (GetAnimatorVariableWireNode)bOwner;
                    try
                    {
                        vb = bGavwn.GetVariableAsFloat();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            vb = bGavwn.GetVariableAsInteger();
                        }
                        catch (Exception) { }
                    }
                }
            }

            if (va == null || vb == null)
                return false;
            int comparation = va.CompareTo(vb);
            return comparation < 0;
        }
    }
}