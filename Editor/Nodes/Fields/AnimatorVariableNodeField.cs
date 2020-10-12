using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Variables;
using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields
{
    public class AnimatorVariableNodeField : WireNodeField<AnimatorVariable>
    {

        public AnimatorVariableNodeField(WireNodeDisplayer owner, AnimatorVariable variable) : base(owner)
        {
            FieldValue = variable;
        }

        public override void Draw()
        {
            // Variable name
            EditorGUILayout.BeginHorizontal();
            FieldValue.Name = EditorGUILayout.TextField(new GUIContent("Name", ""), FieldValue.Name);
            EditorGUILayout.EndHorizontal();


            AnimatorVariableType prevType = FieldValue.Type.Copy();
            EditorGUILayout.BeginHorizontal();
            FieldValue.Type = (AnimatorVariableType)EditorGUILayout.EnumPopup("Type", FieldValue.Type);
            EditorGUILayout.EndHorizontal();

            // Handle type changing
            if (prevType != FieldValue.Type)
            {
                if (prevType == AnimatorVariableType.FLOAT && FieldValue.Type == AnimatorVariableType.INT)
                    FieldValue.StoredValue = ((int)FieldValue.GetValueAs<float>()).ToString();
                else if (prevType == AnimatorVariableType.INT && FieldValue.Type == AnimatorVariableType.FLOAT)
                    FieldValue.StoredValue = ((float)FieldValue.GetValueAs<int>()).ToString();
                else
                    FieldValue.StoredValue = null;
            }

            // Field value
            EditorGUILayout.BeginHorizontal();
            switch (FieldValue.Type)
            {
                case AnimatorVariableType.FLOAT:
                    FieldValue.StoredValue = (EditorGUILayout.FloatField(FieldName, FieldValue.GetValueAs<float>())).ToString();
                    break;
                case AnimatorVariableType.INT:
                    FieldValue.StoredValue = (EditorGUILayout.IntField(FieldName, FieldValue.GetValueAs<int>())).ToString();
                    break;
                case AnimatorVariableType.BOOL:
                    FieldValue.StoredValue = (EditorGUILayout.Toggle(FieldName, FieldValue.GetValueAs<bool>())).ToString();
                    break;
                default: // Trigger
                    EditorGUILayout.LabelField("Will call the trigger.");
                    break;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}