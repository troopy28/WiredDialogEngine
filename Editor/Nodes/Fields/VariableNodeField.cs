using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using UnityEditor;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields
{
    public class VariableNodeField : WireNodeField<Variable>
    {
        public VariableNodeField(WireNodeDisplayer owner, Variable variable) : base(owner)
        {
            FieldValue = variable;
        }

        public override void Draw()
        {
            VariableType prevType = FieldValue.Type.Copy();
            EditorGUILayout.BeginHorizontal();
            FieldValue.Type = (VariableType)EditorGUILayout.EnumPopup("Variable type", FieldValue.Type);
            EditorGUILayout.EndHorizontal();

            if (prevType != FieldValue.Type)
            {
                if (prevType == VariableType.FLOAT && FieldValue.Type == VariableType.INT)
                    FieldValue.Value = (int)FieldValue.GetValueAs<float>();
                else if (prevType == VariableType.INT && FieldValue.Type == VariableType.FLOAT)
                    FieldValue.Value = (float)FieldValue.GetValueAs<int>();
                else if (prevType == VariableType.INT && FieldValue.Type == VariableType.STRING)
                    FieldValue.Value = FieldValue.GetValueAs<int>().ToString();
                else if (prevType == VariableType.FLOAT && FieldValue.Type == VariableType.STRING)
                    FieldValue.Value = FieldValue.GetValueAs<float>().ToString();
                else if (prevType == VariableType.STRING && FieldValue.Type == VariableType.INT)
                {
                    int val;
                    if (int.TryParse(FieldValue.GetValueAs<string>(), out val))
                        FieldValue.Value = val;
                    else
                        FieldValue.Value = null;
                }
                else if (prevType == VariableType.STRING && FieldValue.Type == VariableType.FLOAT)
                {
                    float val;
                    if (float.TryParse(FieldValue.GetValueAs<string>(), out val))
                        FieldValue.Value = val;
                    else
                        FieldValue.Value = null;
                }
                else
                    FieldValue.Value = null;
            }

            EditorGUILayout.BeginHorizontal();
            switch(FieldValue.Type)
            {
                case VariableType.FLOAT:
                    FieldValue.Value = EditorGUILayout.FloatField(FieldName, FieldValue.GetValueAs<float>());
                    break;
                case VariableType.INT:
                    FieldValue.Value = EditorGUILayout.IntField(FieldName, FieldValue.GetValueAs<int>());
                    break;
                case VariableType.STRING:
                    FieldValue.Value = EditorGUILayout.TextField(FieldName, FieldValue.GetValueAs<string>());
                    break;
            }
            EditorGUILayout.EndHorizontal();
            
        }
    }
}