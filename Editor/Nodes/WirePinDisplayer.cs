using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public class WirePinDisplayer
    {
        protected const string ClearConnectionCmd = "ClearConnection";
        protected const string MakeConnectionCmd = "MakeConnection";

        public Rect InteractionRect { get; private set; }
        public WirePin DisplayedWirePin { get; set; }
        public WireNodeDisplayer OwnerDisplayer { get; private set; }

        public WirePinDisplayer(WireNodeDisplayer owner, WirePin displayedWirePin)
        {
            OwnerDisplayer = owner;
            DisplayedWirePin = displayedWirePin;
            DisplayedWirePin.WirePinDisplayer = this;
        }

        #region Rendering

        /// <summary>
        /// Render the pin with the the interaction rect BEFORE the value displayer.
        /// </summary>
        public virtual void DrawAsInput()
        {
            DrawInteractionPin();
            GUILayout.Label(new GUIContent(DisplayedWirePin.PinName));
        }

        /// <summary>
        /// Render the pin with the the interaction rect AFTER the value displayer.
        /// </summary>
        public virtual void DrawAsOutput()
        {
            GUILayout.Label(new GUIContent(DisplayedWirePin.PinName));
            DrawInteractionPin();
        }

        /// <summary>
        /// Draw the pin where the user click to create the connections.
        /// </summary>
        private void DrawInteractionPin()
        {
            if (DisplayedWirePin.IsConnected)
                GUILayout.Label(DialogEditor.PinActivatedTexture, GUILayout.Width(20), GUILayout.Height(20));
            else
                GUILayout.Label(DialogEditor.PinNonActivatedTexture, GUILayout.Width(20), GUILayout.Height(20));
            if (Event.current.type == EventType.Repaint)
                InteractionRect = GUILayoutUtility.GetLastRect();

            //EditorGUI.DrawRect(InteractionRect, Color.blue);
        }

        #endregion

        public virtual void OnRightClick()
        {
            GenericMenu menu = new GenericMenu();
            if (DisplayedWirePin.PinType == WirePinType.INPUT)
            {
                menu.AddItem(new GUIContent("Clear connection"), false, ContextMenuCallback, ClearConnectionCmd);
            }
            else
            {
                menu.AddItem(new GUIContent("Make connection"), false, ContextMenuCallback, MakeConnectionCmd);
                menu.AddItem(new GUIContent("Clear connection"), false, ContextMenuCallback, ClearConnectionCmd);
            }
            menu.ShowAsContext();
        }

        protected virtual void ContextMenuCallback(object obj)
        {
            string command = (string)obj;
            switch (command)
            {
                case ClearConnectionCmd:
                    DisplayedWirePin.Disconnect(true);
                    break;
                case MakeConnectionCmd:
                    if (!DialogEditor.Instance.ConnectionMaker.MakingConnection)
                        DialogEditor.Instance.ConnectionMaker.BeginConnection(DisplayedWirePin);
                    break;
                    // Here go other menu items
            }
        }

        public virtual void OnLeftClick()
        {
            if (DisplayedWirePin.PinType == WirePinType.INPUT && DialogEditor.Instance.ConnectionMaker.MakingConnection)
                DialogEditor.Instance.ConnectionMaker.TryFinalizingConnection(DisplayedWirePin);
        }
    }
}