using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Comparation
{
    public class InequalityNodeDisplayer : WireNodeDisplayer
    {
        private InequalityNodeDisplayer(InequalityWireNode node) : base(node)
        {

        }

        protected override void DoCustomDrawing()
        {
        }

        public static InequalityNodeDisplayer CreateDisplayerFor(InequalityWireNode node)
        {
            InequalityNodeDisplayer nodeRenderer = new InequalityNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 140, 70)
            };
            return nodeRenderer;
        }

        public static InequalityNodeDisplayer CreateReplyNodeDisplayer(Vector2 position)
        {
            InequalityWireNode node = new InequalityWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Non equal"
            };
            DialogEditor.InitializeNode(ref node);

            node.A = new InputWirePin(node, DialogEditor.Instance.EditingDialog)
            {
                PinName = "A",
                DataType = typeof(object)
            };
            node.A.RefusedTypes.Add(WDEngine.ActivityStream.Name);
            node.A.RefusedTypes.Add(typeof(bool).Name);
            node.A.RefusedTypes.Add(typeof(int).Name);
            node.A.RefusedTypes.Add(typeof(float).Name);

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


            InequalityNodeDisplayer nodeRenderer = new InequalityNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 140, 70)
            };
            return nodeRenderer;
        }
    }
}