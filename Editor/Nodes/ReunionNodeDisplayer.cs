using Assets.WiredTools.WiredDialogEngine.Core;
using UnityEditor;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using UnityEngine;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class ReunionNodeDisplayer : WireNodeDisplayer
    {
        public ReunionNodeDisplayer(ReunionWireNode node) : base(node)
        {
        }

        protected override void DoCustomDrawing()
        {
            int inputsCount = RenderedNode.Inputs.Count;

            EditorGUILayout.BeginHorizontal();
            int wantedInputsCount = Mathf.Clamp(EditorGUILayout.IntField("Count", inputsCount, GUILayout.Width(182)), 2, 20);
            EditorGUILayout.EndHorizontal();

            int delta = 0;
            if (inputsCount < wantedInputsCount) // Need to add inputs
            {
                delta = wantedInputsCount - inputsCount;
                for (int i = 0; i < delta; i++)
                {
                    GetRenderedNodeAs<ReunionWireNode>().AddInput(new InputWirePin(RenderedNode, DialogEditor.Instance.EditingDialog)
                    {
                        PinName = "Input " + (inputsCount + i + 1),
                        DataType = WDEngine.ActivityStream
                    });
                }
            }
            else if (inputsCount > wantedInputsCount)
            {
                delta = inputsCount - wantedInputsCount;
                for (int i = 0; i < delta; i++)
                {
                    GetRenderedNodeAs<ReunionWireNode>().RemoveLastInput();
                }
            }

            if (delta != 0)
                SetRenderedNode(RenderedNode);

            inputsCount = wantedInputsCount;
            WindowRect.height = 60 + 25 * inputsCount;
        }

        public static ReunionNodeDisplayer CreateDisplayerFor(ReunionWireNode node)
        {
            ReunionNodeDisplayer nodeRenderer = new ReunionNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 220, 150)
            };
            return nodeRenderer;
        }

        public static ReunionNodeDisplayer CreateReunionNodeDisplayer(Vector2 position)
        {
            ReunionWireNode node = new ReunionWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Reunion"
            };
            DialogEditor.InitializeNode(ref node);

            node.Inputs[0].PinName = "Input 1";
            node.AddInput(new InputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "Input 2",
                DataType = WDEngine.ActivityStream
            });

            ReunionNodeDisplayer nodeRenderer = new ReunionNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 220, 150)
            };

            Debug.Log("Created a REUNION NODE DISPLAYER.");

            return nodeRenderer;
        }
    }
}