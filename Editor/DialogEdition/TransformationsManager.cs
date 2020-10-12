using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class DialogEditorState
    {
        public Vector2 MousePosition { get; set; }
        public Vector2 LookPosition { get; set; }
        public Vector2 LastMousePosition { get; set; }
        public Vector2 PanOffset { get; set; }
        public bool Panning { get; set; }
    }

    public class TransformationsManager
    {
        private DialogEditor editor;
        private const float MaximumMove = 10f;
        public DialogEditorState State { get; private set; }

        public TransformationsManager(DialogEditor editor)
        {
            this.editor = editor;
            State = new DialogEditorState();
            State.PanOffset = Vector2.zero;
            State.LookPosition = editor.position.center;
        }

        public void ManagePan(Event currentEvent) // Called after ManageZoom()
        {
            if (!State.Panning && currentEvent.type == EventType.MouseDown && currentEvent.button == 2)
                State.Panning = true;
            else if (State.Panning && currentEvent.type == EventType.MouseUp && currentEvent.button == 2)
                State.Panning = false;

            State.MousePosition = currentEvent.mousePosition;

            if (State.Panning)
            {
                /*if ((State.MousePosition - State.LastMousePosition).x > MaximumMove || (State.LastMousePosition - State.LastMousePosition).y > MaximumMove ||
                    (State.MousePosition - State.LastMousePosition).x < -MaximumMove || (State.MousePosition - State.LastMousePosition).y < -MaximumMove)
                    State.LastMousePosition = State.MousePosition;
                    */
                Vector2 panMove = (State.MousePosition - State.LastMousePosition);
                float xMove = panMove.x;
                float yMove = panMove.y;

                State.PanOffset += panMove;

                foreach (WireNodeDisplayer nodeDisplayer in editor.GraphEditor.NodeDisplayers)
                {
                    nodeDisplayer.WindowRect.x += xMove;
                    nodeDisplayer.WindowRect.y += yMove;
                }
                editor.Repaint();
            }
            State.LastMousePosition = State.MousePosition;
        }

        public void ManageZoom(Event currentEvent) // Called before ManagePan()
        {
            if (currentEvent.type != EventType.ScrollWheel)
                return;
            State.LookPosition = editor.position.center;
            float scroll = Event.current.delta.y;
            Vector2 clampedZoom = Vector2.ClampMagnitude(Vector2.one * scroll, 50);
            GUIUtility.ScaleAroundPivot(clampedZoom, State.MousePosition);
            Event.current.Use();
        }
    }
}