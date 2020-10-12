using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class ChooseReplyNodeDisplayer : WireNodeDisplayer
    {
        public ChooseReplyNodeDisplayer(ChooseReplyWireNode node) : base(node)
        {
        }

        protected override void DoCustomDrawing()
        {
            int outputsCount = RenderedNode.Outputs.Count;

            EditorGUILayout.BeginHorizontal();
            int wantedOutputsCount = Mathf.Clamp(EditorGUILayout.IntField("Count", outputsCount, GUILayout.Width(182)), 2, 20);
            EditorGUILayout.EndHorizontal();

            int delta = 0;
            if (outputsCount < wantedOutputsCount) // Need to add outputs
            {
                delta = wantedOutputsCount - outputsCount;
                for (int i = 0; i < delta; i++)
                {
                    OutputWirePin newPin = new OutputWirePin(RenderedNode, DialogEditor.Instance.EditingDialog)
                    {
                        PinName = "Choice",
                        DataType = WDEngine.ActivityStream
                    };
                    AddOutput(newPin);
                }
            }
            else if (outputsCount > wantedOutputsCount)
            {
                delta = outputsCount - wantedOutputsCount;
                for (int i = 0; i < delta; i++)
                {
                    RemoveLastOutput();
                }
            }

            if (delta != 0)
                SetRenderedNode(RenderedNode);

            outputsCount = wantedOutputsCount;

            int finalHeight = 50;
            foreach (OutputWirePin outputs in RenderedNode.Outputs)
            {
                if (!outputs.IsConnected)
                    outputs.PinName = "Choice";
                else
                    outputs.PinName = outputs.GetConnectedPin().GetOwner().NodeName;
                finalHeight += 25;
            }
            WindowRect.height = finalHeight;
        }

        public void AddOutput()
        {
            OutputWirePin pin = new OutputWirePin(RenderedNode, DialogEditor.Instance.EditingDialog)
            {
                PinName = "Choice",
                DataType = WDEngine.ActivityStream
            };
            pin.RefusedNodes.Add(typeof(BranchWireNode).Name);
            pin.RefusedNodes.Add(typeof(ReunionWireNode).Name);
            pin.RefusedNodes.Add(typeof(ChooseReplyWireNode).Name);
            pin.RefusedNodes.Add(typeof(SetVariableValWireNode).Name);
            pin.RefusedNodes.Add(typeof(SetAnimatorVariableWireNode).Name);
            AddOutput(pin);
        }

        public void AddOutput(OutputWirePin output)
        {
            if (!output.RefusedNodes.Contains(typeof(BranchWireNode).Name))
                output.RefusedNodes.Add(typeof(BranchWireNode).Name);
            if (!output.RefusedNodes.Contains(typeof(ReunionWireNode).Name))
                output.RefusedNodes.Add(typeof(ReunionWireNode).Name);
            if (!output.RefusedNodes.Contains(typeof(ChooseReplyWireNode).Name))
                output.RefusedNodes.Add(typeof(ChooseReplyWireNode).Name);
            if (!output.RefusedNodes.Contains(typeof(SetVariableValWireNode).Name))
                output.RefusedNodes.Add(typeof(SetVariableValWireNode).Name);
            if (!output.RefusedNodes.Contains(typeof(SetAnimatorVariableWireNode).Name))
                output.RefusedNodes.Add(typeof(SetAnimatorVariableWireNode).Name);

            RenderedNode.Outputs.Add(output);
            SetRenderedNode(RenderedNode);
        }

        public void RemoveOutput(OutputWirePin output)
        {
            Dictionary<uint, WirePin> dic = DialogEditor.Instance.EditingDialog.Pins;
            if (dic.ContainsKey(output.WirePinId))
                dic.Remove(output.WirePinId);

            output.Disconnect(true);
            RenderedNode.Outputs.Remove(output);
        }

        public void RemoveLastOutput()
        {
            int idx = RenderedNode.Outputs.Count - 1;

            Dictionary<uint, WirePin> dic = DialogEditor.Instance.EditingDialog.Pins;
            if (dic.ContainsKey(RenderedNode.Outputs[idx].WirePinId))
                dic.Remove(RenderedNode.Outputs[idx].WirePinId);

            RenderedNode.Outputs[idx].Disconnect(true);
            RenderedNode.Outputs.RemoveAt(idx);
        }

        public static ChooseReplyNodeDisplayer CreateDisplayerFor(ChooseReplyWireNode node)
        {
            ChooseReplyNodeDisplayer nodeRenderer = new ChooseReplyNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 240, 100)
            };
            return nodeRenderer;
        }

        public static ChooseReplyNodeDisplayer CreateChooseReplyNodeDisplayer(Vector2 position)
        {
            ChooseReplyWireNode node = new ChooseReplyWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Choice"
            };
            DialogEditor.InitializeNode(ref node);
            node.Outputs[0].PinName = "Choice";

            ChooseReplyNodeDisplayer nodeRenderer = new ChooseReplyNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 240, 100)
            };
            nodeRenderer.AddOutput();
            return nodeRenderer;
        }
    }
}
