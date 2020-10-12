using UnityEngine;
using UnityEditor;
using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using System.Linq;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;
using System;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Triggers;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class DialogEditor : EditorWindow
    {
        #region Window opening

        public static void CreateWindow()
        {
            if (Instance != null)
                EditorUtility.DisplayDialog("Error", "You can have only one DialogEditor at a time.", "Ok");
            DialogEditor editor = GetWindow<DialogEditor>(false, "Wired Dialog Editor");
            Instance = editor;
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        // Parameter is never used -> Unity parameters, even useless, must be there
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // DO NOT OPEN ANYTHING IF PLAYING, COMPILING OR ABOUT TO PLAY
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return false;
            WireDialog clickedDialog = Selection.activeObject as WireDialog;
            if (clickedDialog != null)
            {
                if (Instance != null)
                {
                    EditorUtility.DisplayDialog("Error", "You can have only one DialogEditor at a time. Please close all DialogEditors before editing this dialog.", "Ok");
                    return false;
                }
                CreateWindow();
                Instance.LoadDialog(clickedDialog);
                return true; // Catch open file
            }
            return false; // Let unity open the file
        }

        #endregion

        #region UI Images

        [SerializeField]
        private Texture2D pinActivatedTexture;
        public static Texture2D PinActivatedTexture
        {
            get
            {
                return Instance.pinActivatedTexture;
            }
        }

        [SerializeField]
        private Texture2D pinNonActivatedTexture;
        public static Texture2D PinNonActivatedTexture
        {
            get
            {
                return Instance.pinNonActivatedTexture;
            }
        }

        [SerializeField]
        private Texture2D backgroundTexture;
        public static Texture2D BackgroundTexture
        {
            get
            {
                return Instance.backgroundTexture;
            }
        }

        #endregion

        private ClickManager clickManager;

        /// <summary>
        /// The instance of the dialog editor. There can be only one instance at a time.
        /// </summary>
        public static DialogEditor Instance { get; private set; }
        /// <summary>
        /// The object managing the visual part of the dialog edition, and the interaction with the user.
        /// </summary>
        public WireGraphEditor GraphEditor { get; private set; }
        /// <summary>
        /// The object that manages the creation of new connections between pins.
        /// </summary>
        public ConnectionMaker ConnectionMaker { get; private set; }
        /// <summary>
        /// The object that manages the movements of the user in the window.
        /// </summary>
        public TransformationsManager TransformationsManager { get; private set; }
        /// <summary>
        /// The object managing the background of the window (the grid).
        /// </summary>
        public Background Background { get; private set; }
        /// <summary>
        /// The menu that is shown whenever the user does a right click on the background of the window (not on a node).
        /// </summary>
        public DialogContextMenu ContextMenu { get; private set; }

        /// <summary>
        /// The dialog being edited by the user.
        /// </summary>
        public WireDialog EditingDialog { get; private set; }

        public delegate void NodeSaving(WireNode node);
        /// <summary>
        /// Called every time a node is saved, after the editor has done his own saving job on this node. Use this to
        /// create your custom saving logic.
        /// </summary>
        public event NodeSaving OnSaveNode;

        /// <summary>
        /// Initializes the dialog editor.
        /// </summary>
        private void Awake() // Like a constructor. Called at start
        {
            GraphEditor = new WireGraphEditor(this);
            clickManager = new ClickManager(this);
            ConnectionMaker = new ConnectionMaker(this);
            TransformationsManager = new TransformationsManager(this);
            Background = new Background();
            ContextMenu = new DialogContextMenu(this);
            GUIScaleUtility.Init();
        }

        /// <summary>
        /// Called every time the editor updates the editor UI. All the drawing logic is here.
        /// </summary>
        private void OnGUI()
        {
            try
            {
                if (Instance == null)
                {
                    Close();
                    return;
                }

                // Absolutely close the window BEFORE entering the play mode, or the dialog could be lost due to the nature of the Unity serialization.
                if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Close();
                    return;
                }

                // First, get the user interaction data
                Event currentEvent = Event.current;
                TransformationsManager.ManagePan(currentEvent);
                TransformationsManager.ManageZoom(currentEvent);
                clickManager.ManageClick();

                // Then draw the background first
                Background.Draw(position, ref backgroundTexture, TransformationsManager.State.PanOffset);

                // If the connection is making a connection (but not finished), the draw the curve between the selected output and the cursor
                if (ConnectionMaker.MakingConnection)
                    ConnectionMaker.DrawMouseCurve();
                // Then draw the nodes
                GraphEditor.Draw();

                // And finally draw the toolbar
                DrawToolbar();
            }
            catch (Exception)
            {
                Debug.Log("An error occurred in the Dialog Editor. Please try to restart it.");
                Close();
            }
        }

        private void Update()
        {
            if (Instance == null)
            {
                Close();
                return;
            }

            // Absolutely close the window BEFORE entering the play mode, or the dialog could be lost due to the nature of the Unity serialization.
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Close();
                return;
            }
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(100)))
                SaveDialog();

            EditorGUILayout.LabelField("Editing dialog: " + EditingDialog.DialogName);
            GUILayout.EndHorizontal();
        }

        public void LoadDialog(WireDialog dialog)
        {
            Debug.Log("Loading a new dialog...");

            try
            {
                GraphEditor.NodeDisplayers.Clear(); // Clear the displayer UI
            }
            catch (Exception)
            {
                Debug.Log("An error occurred. Please reload the Dialog Editor. If this bug always happens, please contact me.");
                Close();
            }

            if (!dialog.Initialized)
            {
                EditingDialog = dialog; // Needed to create the displayer
                DialogBeginningNodeDisplayer beginningDisplayer = DialogBeginningNodeDisplayer.CreateDialogBeginningDisplayer(new Vector2(100, 150));
                DialogBeginningWireNode beginningNode = beginningDisplayer.GetRenderedNodeAs<DialogBeginningWireNode>();
                dialog.Initialize(ref beginningNode);
            }

            dialog.Load();
            EditingDialog = dialog; // Set the new dialog to edit

            foreach (WireNode node in dialog.NodesDictionary.Values)
            {
                if (node is EqualityWireNode)
                    GraphEditor.NodeDisplayers.Add((node as EqualityWireNode).CreateDisplayer());
                else if (node is GreaterWireNode)
                    GraphEditor.NodeDisplayers.Add((node as GreaterWireNode).CreateDisplayer());
                else if (node is InequalityWireNode)
                    GraphEditor.NodeDisplayers.Add((node as InequalityWireNode).CreateDisplayer());
                else if (node is LesserWireNode)
                    GraphEditor.NodeDisplayers.Add((node as LesserWireNode).CreateDisplayer());
                else if (node is ConstantWireNode)
                    GraphEditor.NodeDisplayers.Add((node as ConstantWireNode).CreateDisplayer());
                else if (node is SayReplyWireNode)
                    GraphEditor.NodeDisplayers.Add((node as SayReplyWireNode).CreateDisplayer());
                else if (node is BranchWireNode)
                    GraphEditor.NodeDisplayers.Add((node as BranchWireNode).CreateDisplayer());
                else if (node is SetAnimatorVariableWireNode)
                    GraphEditor.NodeDisplayers.Add((node as SetAnimatorVariableWireNode).CreateDisplayer());
                else if (node is GetAnimatorVariableWireNode)
                    GraphEditor.NodeDisplayers.Add((node as GetAnimatorVariableWireNode).CreateDisplayer());
                else if (node is ChooseReplyWireNode)
                    GraphEditor.NodeDisplayers.Add((node as ChooseReplyWireNode).CreateDisplayer());
                else if (node is ReunionWireNode)
                    GraphEditor.NodeDisplayers.Add((node as ReunionWireNode).CreateDisplayer());
                else if (node is GetVariableWireNode)
                    GraphEditor.NodeDisplayers.Add((node as GetVariableWireNode).CreateDisplayer());
                else if (node is SetVariableValWireNode)
                    GraphEditor.NodeDisplayers.Add((node as SetVariableValWireNode).CreateDisplayer());
                else if (node is DialogBeginningWireNode)
                    GraphEditor.NodeDisplayers.Add((node as DialogBeginningWireNode).CreateDisplayer());
                else if (node is TriggerScriptWireNode)
                    GraphEditor.NodeDisplayers.Add((node as TriggerScriptWireNode).CreateDisplayer());
                else if (node is SetTriggerParamValueWireNode)
                    GraphEditor.NodeDisplayers.Add((node as SetTriggerParamValueWireNode).CreateDisplayer());
            }
            Debug.Log("Loaded");
        }

        private void OnDestroy()
        {
            if (GraphEditor != null && GraphEditor.NodeDisplayers != null)
                GraphEditor.NodeDisplayers.Clear();
            if (EditingDialog != null)
                SaveDialog();
            else
                EditorApplication.SaveAssets();
        }

        public void AddNodeToDialog(WireNodeDisplayer displayer)
        {
            EditingDialog.NodesDictionary.Add(displayer.RenderedNode.WireNodeId, displayer.RenderedNode);
            GraphEditor.NodeDisplayers.Add(displayer);
        }

        public void RemoveNodeFromDialog(WireNodeDisplayer displayer)
        {
            EditingDialog.NodesDictionary.Remove(displayer.RenderedNode.WireNodeId);
            GraphEditor.NodeDisplayers.Remove(displayer);
        }

        private void SaveDialog()
        {
            EditingDialog.Save(ManageNodeSaving);
            EditorUtility.SetDirty(EditingDialog);
            EditorApplication.SaveAssets();
        }

        /// <summary>
        /// Called every time a node is saved.
        /// </summary>
        /// <param name="node"></param>
        private void ManageNodeSaving(WireNode node)
        {
            if (node is SayReplyWireNode)
            {
                SayReplyWireNode replyNode = node as SayReplyWireNode;
                replyNode.ReplyResourcePath = AssetDatabase.GetAssetPath(replyNode.Reply).Replace("Assets/Resources/", "").Replace(".asset", "");
                replyNode.TargetResourcePath = AssetDatabase.GetAssetPath(replyNode.Target).Replace("Assets/Resources/", "").Replace(".asset", "");
            }
            else if (node is SetAnimatorVariableWireNode)
            {
                SetAnimatorVariableWireNode setVarNode = node as SetAnimatorVariableWireNode;
                setVarNode.TargetResourcePath = AssetDatabase.GetAssetPath(setVarNode.TargetActor).Replace("Assets/Resources/", "").Replace(".asset", "");
            }
            else if (node is GetAnimatorVariableWireNode)
            {
                GetAnimatorVariableWireNode getVarNode = node as GetAnimatorVariableWireNode;
                getVarNode.TargetResourcePath = AssetDatabase.GetAssetPath(getVarNode.TargetActor).Replace("Assets/Resources/", "").Replace(".asset", "");
            }

            if (node is ISavable)
                (node as ISavable).Save();

            if (OnSaveNode != null)
                OnSaveNode.Invoke(node);
        }

        public static T InitializeNode<T>(ref T node) where T : WireNode
        {
            if (node is DialogBeginningWireNode)
            {
                if (!node.Outputs.Any(output => output.PinName == "Outgoing activity"))
                {
                    node.Outputs.Add(new OutputWirePin(node, Instance.EditingDialog)
                    {
                        PinName = "Outgoing activity",
                        DataType = WDEngine.ActivityStream
                    });
                }
                node.Inputs.Clear();
            }
            else if (node is ExecutionWireNode)
            {
                if (!node.Inputs.Any(input => input.PinName == "Incoming activity"))
                    node.Inputs.Add(new InputWirePin(node, Instance.EditingDialog)
                    {
                        PinName = "Incoming activity",
                        DataType = WDEngine.ActivityStream
                    });

                if (!node.Outputs.Any(output => output.PinName == "Outgoing activity"))
                    node.Outputs.Add(new OutputWirePin(node, Instance.EditingDialog)
                    {
                        PinName = "Outgoing activity",
                        DataType = WDEngine.ActivityStream
                    });
            }
            else if (node is ComparationWireNode)
            {
                if (!node.Outputs.Any(output => output.PinName == "Result"))
                    node.Outputs.Add(new OutputWirePin(node, Instance.EditingDialog)
                    {
                        PinName = "Result",
                        DataType = typeof(bool)
                    });
            }
            return node;
        }

        /// <summary>
        /// The proper way of closing and destroying the dialog editor window. It saves
        /// the dialog if a dialog is being edited. Do not call this while playing.
        /// </summary>
        public new void Close()
        {
            try
            {
                base.Close();
            }
            catch (Exception) { }

            if (GraphEditor != null && GraphEditor.NodeDisplayers != null)
                GraphEditor.NodeDisplayers.Clear();
            if (EditingDialog != null)
            {
                SaveDialog();
                EditingDialog.Unload();
            }
            else
                EditorApplication.SaveAssets();
        }
    }
}