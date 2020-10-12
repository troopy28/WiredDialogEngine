using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    [CustomEditor(typeof(Variable))]
    [CanEditMultipleObjects]
    public class VariableInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Variable var = (Variable)target;
            var.Load(false);

            EditorGUILayout.BeginHorizontal();
            var.VariableName = EditorGUILayout.TextField("Variable name", var.VariableName);
            EditorGUILayout.EndHorizontal();

            VariableType prevType = var.Type.Copy();
            EditorGUILayout.BeginHorizontal();
            var.Type = (VariableType)EditorGUILayout.EnumPopup("Variable type", var.Type);
            EditorGUILayout.EndHorizontal();

            if (prevType != var.Type)
            {
                if (prevType == VariableType.FLOAT && var.Type == VariableType.INT)
                    var.Value = (int)var.GetValueAs<float>();
                else if (prevType == VariableType.INT && var.Type == VariableType.FLOAT)
                    var.Value = (float)var.GetValueAs<int>();
                else if (prevType == VariableType.INT && var.Type == VariableType.STRING)
                    var.Value = var.GetValueAs<int>().ToString();
                else if (prevType == VariableType.FLOAT && var.Type == VariableType.STRING)
                    var.Value = var.GetValueAs<float>().ToString();
                else if (prevType == VariableType.STRING && var.Type == VariableType.INT)
                {
                    int val;
                    if (int.TryParse(var.GetValueAs<string>(), out val))
                        var.Value = val;
                    else
                        var.Value = null;
                }
                else if (prevType == VariableType.STRING && var.Type == VariableType.FLOAT)
                {
                    float val;
                    if (float.TryParse(var.GetValueAs<string>(), out val))
                        var.Value = val;
                    else
                        var.Value = null;
                }
                else
                    var.Value = null;
            }

            EditorGUILayout.BeginHorizontal();
            switch (var.Type)
            {
                case VariableType.FLOAT:
                    var.Value = EditorGUILayout.FloatField("Variable value", var.GetValueAs<float>());
                    break;
                case VariableType.INT:
                    var.Value = EditorGUILayout.IntField("Variable value", var.GetValueAs<int>());
                    break;
                case VariableType.STRING:
                    var.Value = EditorGUILayout.TextField("Variable value", var.GetValueAs<string>());
                    break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Save", "Save any changes made to this variable to the disk.")))
            {
                var.Save(false);
                EditorUtility.SetDirty(var);
                EditorApplication.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();

            var.Save(false);
        }
    }
}