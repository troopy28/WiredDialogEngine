using Assets.WiredTools.WiredDialogEngine.Editor.Nodes;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class WireGraphEditor
    {
        private List<WireNodeDisplayer> nodeDisplayers;
        public List<WireNodeDisplayer> NodeDisplayers
        {
            get
            {
                return nodeDisplayers;
            }
        }
        private DialogEditor dialogEditor;
        private bool firstDrawing = true;

        public WireGraphEditor(DialogEditor dialogEditor)
        {
            nodeDisplayers = new List<WireNodeDisplayer>();
            this.dialogEditor = dialogEditor;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();

            RenderNodes();

            GUILayout.EndHorizontal();
        }

        public void Test()
        {
        }

        private void RenderNodes()
        {
            if(firstDrawing)
            {
                dialogEditor.BeginWindows();
                for (int i = 0; i < nodeDisplayers.Count; i++)
                {
                    if (nodeDisplayers[i] != null)
                        nodeDisplayers[i].WindowRect = GUI.Window(i, nodeDisplayers[i].WindowRect, DrawWireNode, nodeDisplayers[i].RenderedNode.NodeName);
                }
                dialogEditor.EndWindows(); // End of windows rendering
                firstDrawing = false;
            }

            // First render the curves
            foreach (WireNodeDisplayer wnd in nodeDisplayers)
                wnd.DrawWires();

            // Then render the windows
            dialogEditor.BeginWindows();
            for (int i = 0; i < nodeDisplayers.Count; i++)
            {
                if (nodeDisplayers[i] != null)
                    nodeDisplayers[i].WindowRect = GUI.Window(i, nodeDisplayers[i].WindowRect, DrawWireNode, nodeDisplayers[i].RenderedNode.NodeName);
            }
            dialogEditor.EndWindows(); // End of windows rendering
        }

        private void DrawWireNode(int id)
        {
            nodeDisplayers[id].DrawWindow();
            GUI.DragWindow();
        }

        public static void DrawNodeCurve(Rect start, Rect end, Color color)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(color.r, color.g, color.b, 0.06f);
            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 1);
        }
    }
}