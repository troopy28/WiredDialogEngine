using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Triggers
{
    public class SetTriggerParamValueNodeDisplayer : WireNodeDisplayer
    {
        private SetTriggerParamValueNodeDisplayer(SetTriggerParamValueWireNode node) : base(node)
        {

        }

        protected override void DoCustomDrawing()
        {
            SetTriggerParamValueWireNode renderedNode = GetRenderedNodeAs<SetTriggerParamValueWireNode>();

            EditorGUILayout.BeginHorizontal();
            renderedNode.TargetGameObjectName = EditorGUILayout.TextField(
                new GUIContent("G.O.", "The name of the game object holding the DialogTrigger component, or a child of it."),
                renderedNode.TargetGameObjectName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.TriggerName = EditorGUILayout.TextField(
                new GUIContent("T. name", "The name of the trigger you want to use. T"),
                renderedNode.TriggerName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.ParameterName = EditorGUILayout.TextField(
                new GUIContent("P. name", "The name of the trigger parameter to change."),
                renderedNode.ParameterName
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.ParameterValue = EditorGUILayout.TextField(
                new GUIContent("P. val", "The value you want to give to the parameter of the DialogTrigger."),
                renderedNode.ParameterValue
                );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            renderedNode.Delay = EditorGUILayout.FloatField(
                new GUIContent("Delay", "The delay before going to the next node after changing the parameter, in seconds."),
                renderedNode.Delay
                );
            EditorGUILayout.EndHorizontal();
        }

        public static SetTriggerParamValueNodeDisplayer CreateDisplayerFor(SetTriggerParamValueWireNode node)
        {
            SetTriggerParamValueNodeDisplayer nodeRenderer = new SetTriggerParamValueNodeDisplayer(node)
            {
                WindowRect = new Rect(node.DisplayerPosition.x, node.DisplayerPosition.y, 270, 140)
            };
            return nodeRenderer;
        }

        public static SetTriggerParamValueNodeDisplayer CreateSetTriggerParamDisplayer(Vector2 position)
        {
            SetTriggerParamValueWireNode node = new SetTriggerParamValueWireNode(DialogEditor.Instance.EditingDialog)
            {
                NodeName = "Set trigger param"
            };
            DialogEditor.InitializeNode(ref node);

            SetTriggerParamValueNodeDisplayer nodeRenderer = new SetTriggerParamValueNodeDisplayer(node)
            {
                WindowRect = new Rect(position.x, position.y, 270, 140)
            };
            return nodeRenderer;
        }
    }
}