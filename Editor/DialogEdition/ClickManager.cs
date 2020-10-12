using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class ClickManager
    {
        private DialogEditor dialogEditor;

        public ClickManager(DialogEditor dialogEditor)
        {
            this.dialogEditor = dialogEditor;
        }

        public void ManageClick()
        {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown)
            {
                if (currentEvent.button == 1)
                    OnRightClick(currentEvent);
                else if (currentEvent.button == 0)
                    OnLeftClick(currentEvent);
            }
        }

        private void OnRightClick(Event currEvent)
        {
            Vector2 mousePosGlobalSpace = currEvent.mousePosition;
            WireNodeDisplayer clickedNode = GetClickedNodeDisplayer(mousePosGlobalSpace);
            if (clickedNode != null)
            {
                clickedNode.OnRightClick(mousePosGlobalSpace - clickedNode.WindowRect.position);
                return;
            }
            dialogEditor.ContextMenu.Show(mousePosGlobalSpace);
        }

        private void OnLeftClick(Event currEvent)
        {
            Vector2 mousePosGlobalSpace = currEvent.mousePosition;
            WireNodeDisplayer clickedNode = GetClickedNodeDisplayer(mousePosGlobalSpace);
            if (clickedNode != null)
            {
                clickedNode.OnLeftClick(mousePosGlobalSpace - clickedNode.WindowRect.position);
                return;
            }
            if (dialogEditor.ConnectionMaker.MakingConnection)
                dialogEditor.ConnectionMaker.CancelConnection();
        }

        private WireNodeDisplayer GetClickedNodeDisplayer(Vector2 mousePosGlobalSpace)
        {
            foreach (WireNodeDisplayer wnd in dialogEditor.GraphEditor.NodeDisplayers)
            {
                if (wnd.WindowRect.Contains(mousePosGlobalSpace))
                    return wnd;
            }
            return null;
        }
    }
}