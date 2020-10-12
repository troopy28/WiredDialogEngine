using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Triggers
{
    public class TriggerScriptNodeDisplayer : WireNodeDisplayer
    {
        private TriggerScriptNodeDisplayer(TriggerScriptWireNode node) : base(node)
        {

        }

        protected override void DoCustomDrawing()
        {
            TriggerScriptWireNode renderedNode = GetRenderedNodeAs<TriggerScriptWireNode>();

            EditorGUILayout.BeginHorizontal();
            renderedNode.TargetGameObjectName = EditorGUILayout.TextField(
                new GUIContent("G.O.", "The name of the game object holding the DialogTrigger component, or a child of it."),
                renderedNode.TargetGameObjectName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.TriggerName = EditorGUILayout.TextField(
                new GUIContent("T. Name", "The name of the trigger you want to use."),
                renderedNode.TriggerName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.Delay = EditorGUILayout.FloatField(
                new GUIContent("Delay", "The delay before going to the next node after calling the trigger, in seconds."),
                renderedNode.Delay
                );
            EditorGUILayout.EndHorizontal();
        }

        public static TriggerScriptNodeDisplayer CreateDisplayerFor(TriggerScriptWireNode node)
        {
            TriggerScriptNodeDisplayer nodeRenderer = new TriggerScriptNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 100)
            };
            return nodeRenderer;
        }

        public static TriggerScriptNodeDisplayer CreateTriggerScriptNodeDisplayer(Vector2 position)
        {
            TriggerScriptWireNode node = new TriggerScriptWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Trigger script"
            };
            DialogEditor.InitializeNode(ref node);

            TriggerScriptNodeDisplayer nodeRenderer = new TriggerScriptNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 100)
            };
            return nodeRenderer;
        }
    }
}