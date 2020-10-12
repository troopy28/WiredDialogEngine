using Assets.WiredTools.WiredDialogEngine.Core.Replies;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields
{
    public class ReplyNodeField : WireNodeField<Reply>
    {
        public ReplyNodeField(WireNodeDisplayer owner) : base(owner)
        {
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 50;
            FieldValue = EditorGUILayout.ObjectField(FieldName, FieldValue, DataType, false, GUILayout.Width(200)) as Reply;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}