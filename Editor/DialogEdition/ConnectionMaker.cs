using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class ConnectionMaker
    {
        private WirePin sourcePin;
        private WirePinDisplayer sourcePinDisplayer;
        private DialogEditor editor;
        public bool MakingConnection { get; private set; }

        public ConnectionMaker(DialogEditor editor)
        {
            this.editor = editor;
        }

        public void BeginConnection(WirePin sourcePin)
        {
            this.sourcePin = sourcePin;
            sourcePinDisplayer = (WirePinDisplayer)sourcePin.WirePinDisplayer;
            MakingConnection = true;
        }

        public void CancelConnection()
        {
            sourcePin = null;
            sourcePinDisplayer = null;
            MakingConnection = false;
        }

        public bool TryFinalizingConnection(WirePin targetPin)
        {
            if (targetPin.Connect(sourcePin, true))
            {
                sourcePin = null;
                sourcePinDisplayer = null;
                MakingConnection = false; // No more making the connection
                return true;
            }
            MakingConnection = true;
            return false;
        }

        public void DrawMouseCurve()
        {
            Vector2 mousePosGlobalSpace = Event.current.mousePosition;
            Rect rect = new Rect(
                sourcePinDisplayer.InteractionRect.position + sourcePinDisplayer.OwnerDisplayer.WindowRect.position,
                sourcePinDisplayer.InteractionRect.size);

            Rect mouseRect = new Rect(mousePosGlobalSpace.x, mousePosGlobalSpace.y, 1, 1);
            WireGraphEditor.DrawNodeCurve(rect, mouseRect, Color.white);

            editor.Repaint();
        }
    }
}