using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class SayReplyNodeDisplayer : WireNodeDisplayer
    {
        public ReplyNodeField ReplyField { get; private set; }
        public ActorIdentifierNodeField ActorField { get; private set; }

        private SayReplyNodeDisplayer(SayReplyWireNode node) : base(node)
        {
            ReplyField = new ReplyNodeField(this)
            {
                FieldName = "Reply"
            };
            ReplyField.FieldValue = node.Reply;
            ActorField = new ActorIdentifierNodeField(this)
            {
                FieldName = "Target"
            };
            ActorField.FieldValue = node.Target;
        }

        protected override void DoCustomDrawing()
        {
            ReplyField.Draw();
            GetRenderedNodeAs<SayReplyWireNode>().Reply = ReplyField.FieldValue;

            ActorField.Draw();
            GetRenderedNodeAs<SayReplyWireNode>().Target = ActorField.FieldValue;

            EditorGUILayout.BeginHorizontal();
            GetRenderedNodeAs<SayReplyWireNode>().Delay = EditorGUILayout.FloatField(
                new GUIContent("Delay", "The delay before going to the next node after saying the reply, in seconds."),
                GetRenderedNodeAs<SayReplyWireNode>().Delay,
                GUILayout.Width(182)
                );
            EditorGUILayout.EndHorizontal();
        }

        public static SayReplyNodeDisplayer CreateDisplayerFor(SayReplyWireNode node)
        {
            SayReplyNodeDisplayer nodeRenderer = new SayReplyNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 100)
            };
            return nodeRenderer;
        }

        public static SayReplyNodeDisplayer CreateSayReplyNodeDisplayer(Vector2 position)
        {
            SayReplyWireNode node = new SayReplyWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Say reply"
            };
            DialogEditor.InitializeNode(ref node);

            SayReplyNodeDisplayer nodeRenderer = new SayReplyNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 100)
            };
            return nodeRenderer;
        }
    }
}