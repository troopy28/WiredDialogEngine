using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields
{
    public class ActorIdentifierNodeField : WireNodeField<ActorIdentifier>
    {
        public ActorIdentifierNodeField(WireNodeDisplayer owner) : base(owner)
        {
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 50;
            FieldValue = EditorGUILayout.ObjectField(new GUIContent(FieldName, "The actor that will do the action."), FieldValue, DataType, false, GUILayout.Width(200)) as ActorIdentifier;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}