using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Comparation
{
    public class LesserNodeDisplayer : WireNodeDisplayer
    {
        private LesserNodeDisplayer(LesserWireNode node) : base(node)
        {

        }

        protected override void DoCustomDrawing()
        {
        }

        public static LesserNodeDisplayer CreateDisplayerFor(LesserWireNode node)
        {
            LesserNodeDisplayer nodeRenderer = new LesserNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 140, 70)
            };
            return nodeRenderer;
        }

        public static LesserNodeDisplayer CreateLesserNodeDisplayer(Vector2 position)
        {
            LesserWireNode node = new LesserWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Lesser"
            };
            DialogEditor.InitializeNode(ref node);

            node.A = new InputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "A",
                DataType = typeof(object)
            };
            node.A.RefusedTypes.Add(WDEngine.ActivityStream.Name);
            node.A.RefusedTypes.Add(typeof(bool).Name);

            node.B = new InputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "B",
                DataType = typeof(object)
            };
            node.B.RefusedTypes.Add(WDEngine.ActivityStream.Name);
            node.B.RefusedTypes.Add(typeof(bool).Name);
            node.B.RefusedTypes.Add(typeof(int).Name);
            node.B.RefusedTypes.Add(typeof(float).Name);

            node.Inputs.Add(node.A);
            node.Inputs.Add(node.B);

            LesserNodeDisplayer nodeRenderer = new LesserNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 140, 70)
            };
            return nodeRenderer;
        }
    }
}