using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Triggers;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class DialogContextMenu
    {
        private readonly DialogEditor editor;
        private Vector2 position;

        #region Menu constants

        private const string AddSayReplyNodeCmd = "AddSayReplyNode";

        // Comparisons
        private const string AddEqualityNodeCmd = "AddEqualityNode";
        private const string AddInequalityNodeCmd = "AddInequalityNode";
        private const string AddGreaterNodeCmd = "AddGreaterNode";
        private const string AddLesserNodeCmd = "AddLesserNode";

        // Flow control
        private const string AddBranchNodeCmd = "AddBranchNode";
        private const string AddDialogReunionCmd = "AddDialogReunionNode";
        private const string AddChooseReplyNodeCmd = "AddChooseReplyNode";

        // Get data
        private const string AddConstantNodeCmd = "AddConstantNode";
        private const string AddGetVariableCmd = "AddGetVariableNode";
        private const string AddUpdateVariableCmd = "AddUpdateVariable";

        // Animation
        private const string AddSetAnimatorVariableNodeCmd = "AddSetAnimatorVariableValueNode";
        private const string AddGetAnimatorVariableNodeCmd = "AddGetAnimatorVariableValueNode";

        // Script trigger
        private const string AddTriggerScriptNodeCmd = "AddTriggerScriptNode";
        private const string AddSetTriggerParamNodeCmd = "AddSetTriggerParamNode";

        #endregion

        public DialogContextMenu(DialogEditor editor)
        {
            this.editor = editor;
        }

        public void Show(Vector2 position)
        {
            this.position = position;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Say reply"), false, MenuCallback, AddSayReplyNodeCmd);

            menu.AddItem(new GUIContent("Comparison/Equality"), false, MenuCallback, AddEqualityNodeCmd);
            menu.AddItem(new GUIContent("Comparison/Inequality"), false, MenuCallback, AddInequalityNodeCmd);
            menu.AddItem(new GUIContent("Comparison/Greater"), false, MenuCallback, AddGreaterNodeCmd);
            menu.AddItem(new GUIContent("Comparison/Lesser"), false, MenuCallback, AddLesserNodeCmd);

            menu.AddItem(new GUIContent("Flow control/Branch"), false, MenuCallback, AddBranchNodeCmd);
            menu.AddItem(new GUIContent("Flow control/Reunion"), false, MenuCallback, AddDialogReunionCmd);
            menu.AddItem(new GUIContent("Flow control/Choice"), false, MenuCallback, AddChooseReplyNodeCmd);

            menu.AddItem(new GUIContent("Variables/Constant"), false, MenuCallback, AddConstantNodeCmd);
            menu.AddItem(new GUIContent("Variables/Get variable"), false, MenuCallback, AddGetVariableCmd);
            menu.AddItem(new GUIContent("Variables/Update variable"), false, MenuCallback, AddUpdateVariableCmd);

            menu.AddItem(new GUIContent("Animation/Set animator variable"), false, MenuCallback, AddSetAnimatorVariableNodeCmd);
            menu.AddItem(new GUIContent("Animation/Get animator variable"), false, MenuCallback, AddGetAnimatorVariableNodeCmd);

            menu.AddItem(new GUIContent("Trigger/Trigger script"), false, MenuCallback, AddTriggerScriptNodeCmd);
            menu.AddItem(new GUIContent("Trigger/Set trigger param"), false, MenuCallback, AddSetTriggerParamNodeCmd);

            menu.ShowAsContext();
        }


        private void MenuCallback(object value)
        {
            string command = (string)value;

            switch (command)
            {
                case AddSayReplyNodeCmd:
                    AddSayReplyNode();
                    break;
                case AddEqualityNodeCmd:
                    AddEqualityNode();
                    break;
                case AddBranchNodeCmd:
                    AddBranchNode();
                    break;
                case AddInequalityNodeCmd:
                    AddInequalityNode();
                    break;
                case AddConstantNodeCmd:
                    AddConstantNode();
                    break;
                case AddGreaterNodeCmd:
                    AddGreaterNode();
                    break;
                case AddLesserNodeCmd:
                    AddLesserNode();
                    break;
                case AddSetAnimatorVariableNodeCmd:
                    AddSetAnimatorVariableNode();
                    break;
                case AddGetAnimatorVariableNodeCmd:
                    AddGetAnimatorVariableNode();
                    break;
                case AddDialogReunionCmd:
                    AddDialogReunionNode();
                    break;
                case AddGetVariableCmd:
                    AddGetVariableNode();
                    break;
                case AddChooseReplyNodeCmd:
                    AddChooseReplyNode();
                    break;
                case AddUpdateVariableCmd:
                    AddUpdateVariable();
                    break;
                case AddTriggerScriptNodeCmd:
                    AddTriggerScriptNode();
                    break;
                case AddSetTriggerParamNodeCmd:
                    AddSetTriggetParamNode();
                    break;
            }
        }

        private void AddSayReplyNode()
        {
            SayReplyNodeDisplayer newNode = SayReplyNodeDisplayer.CreateSayReplyNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddEqualityNode()
        {
            EqualityNodeDisplayer newNode = EqualityNodeDisplayer.CreateReplyNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddInequalityNode()
        {
            InequalityNodeDisplayer newNode = InequalityNodeDisplayer.CreateReplyNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddBranchNode()
        {
            BranchNodeDisplayer newNode = BranchNodeDisplayer.CreateBranchDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddConstantNode()
        {
            ConstantNodeDisplayer newNode = ConstantNodeDisplayer.CreateConstantNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddGreaterNode()
        {
            GreaterNodeDisplayer newNode = GreaterNodeDisplayer.CreateGreaterNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddLesserNode()
        {
            LesserNodeDisplayer newNode = LesserNodeDisplayer.CreateLesserNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddSetAnimatorVariableNode()
        {
            SetAnimatorVariableNodeDisplayer newNode = SetAnimatorVariableNodeDisplayer.CreateSetAnimatorVariableNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddGetAnimatorVariableNode()
        {
            GetAnimatorVariableNodeDisplayer newNode = GetAnimatorVariableNodeDisplayer.CreateGetAnimatorVariableNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddDialogReunionNode()
        {
            ReunionNodeDisplayer newNode = ReunionNodeDisplayer.CreateReunionNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddGetVariableNode()
        {
            GetVariableNodeDisplayer newNode = GetVariableNodeDisplayer.CreateGetVariableNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddUpdateVariable()
        {
            SetVariableValNodeDisplayer newNode = SetVariableValNodeDisplayer.CreateSetVariableValDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddChooseReplyNode()
        {
            ChooseReplyNodeDisplayer newNode = ChooseReplyNodeDisplayer.CreateChooseReplyNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddTriggerScriptNode()
        {
            TriggerScriptNodeDisplayer newNode = TriggerScriptNodeDisplayer.CreateTriggerScriptNodeDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }

        private void AddSetTriggetParamNode()
        {
            SetTriggerParamValueNodeDisplayer newNode = SetTriggerParamValueNodeDisplayer.CreateSetTriggerParamDisplayer(position);
            editor.AddNodeToDialog(newNode);
        }
    }
}