using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes
{
    public abstract class WireNodeDisplayer
    {
        public Rect WindowRect;
        public WireNode RenderedNode { get; private set; }
        public List<WirePinDisplayer> InputPinsDisplayer { get; private set; }
        public List<WirePinDisplayer> OutputPinsDisplayer { get; private set; }

        protected const string RemoveNodeCmd = "RemoveNode";

        protected WireNodeDisplayer(WireNode node)
        {
            InputPinsDisplayer = new List<WirePinDisplayer>();
            OutputPinsDisplayer = new List<WirePinDisplayer>();
            SetRenderedNode(node);
        }

        public virtual void DrawWindow()
        {
            if (RenderedNode == null)
                return;
            int inputsCount = InputPinsDisplayer.Count;
            int outputsCount = OutputPinsDisplayer.Count;
            int rowCount = Math.Max(inputsCount, outputsCount);
            GUILayout.BeginVertical();
            for (int i = 0; i < rowCount; i++)
            {
                GUILayout.BeginHorizontal();

                WirePinDisplayer renderedInput = null;
                if (i < inputsCount)
                    renderedInput = InputPinsDisplayer[i];
                if (renderedInput != null)
                    renderedInput.DrawAsInput();

                WirePinDisplayer renderedOutput = null;
                if (i < outputsCount)
                    renderedOutput = OutputPinsDisplayer[i];
                if (renderedOutput != null)
                {
                    GUILayout.FlexibleSpace();
                    renderedOutput.DrawAsOutput();
                }
                GUILayout.EndHorizontal();
            }
            DoCustomDrawing();

            GUILayout.EndVertical();

            RenderedNode.DisplayerPosition = WindowRect.position;
        }

        public void SetRenderedNode(WireNode node)
        {
            RenderedNode = node;
            if (node == null)
                return;
            InputPinsDisplayer.Clear();
            OutputPinsDisplayer.Clear();
            foreach (InputWirePin input in RenderedNode.Inputs)
            {
                InputPinsDisplayer.Add(new WirePinDisplayer(this, input));
            }

            foreach (OutputWirePin output in RenderedNode.Outputs)
            {
                OutputPinsDisplayer.Add(new WirePinDisplayer(this, output));
            }
        }

        protected abstract void DoCustomDrawing();

        public virtual void DrawWires()
        {
            foreach (WirePinDisplayer input in InputPinsDisplayer) // The wires are rendered by the node having the end of the connection (inputs)
            {
                WirePin inputPin = input.DisplayedWirePin;
                WirePinDisplayer targetDisplayer = (WirePinDisplayer)inputPin.WirePinDisplayer;

                Rect targetRect = new Rect(targetDisplayer.InteractionRect.position + WindowRect.position, targetDisplayer.InteractionRect.size);

                if (inputPin.IsConnected)
                {
                    WirePinDisplayer sourceDisplayer = (WirePinDisplayer)inputPin.GetConnectedPin().WirePinDisplayer;
                    Rect sourceRect = new Rect(
                        sourceDisplayer.InteractionRect.position + sourceDisplayer.OwnerDisplayer.WindowRect.position,
                        targetDisplayer.InteractionRect.size
                        );
                    WireGraphEditor.DrawNodeCurve(sourceRect, targetRect, WDEngine.ClassicWireColor);
                }
            }
        }

        public void OnRightClick(Vector2 mousePosNodeSpace)
        {
            WirePinDisplayer clicked = null;
            foreach (WirePinDisplayer inputs in InputPinsDisplayer)
            {
                if (inputs.InteractionRect.Contains(mousePosNodeSpace))
                    clicked = inputs;
            }
            foreach (WirePinDisplayer outputs in OutputPinsDisplayer)
            {
                if (outputs.InteractionRect.Contains(mousePosNodeSpace))
                    clicked = outputs;
            }
            if (clicked != null)
                clicked.OnRightClick();
            else
            {
                GenericMenu menu = GetContextMenu();
                if (menu != null)
                    menu.ShowAsContext();
            }
        }

        protected virtual GenericMenu GetContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove node"), false, ContextMenuCallback, RemoveNodeCmd);
            return menu;
        }

        protected virtual void ContextMenuCallback(object obj)
        {
            string command = (string)obj;
            switch (command)
            {
                case RemoveNodeCmd:
                    Remove();
                    break;
                    // Here go other menu items
            }
        }

        public virtual void Remove()
        {
            foreach (WirePinDisplayer inputs in InputPinsDisplayer)
                inputs.DisplayedWirePin.Disconnect(true);
            foreach (WirePinDisplayer outputs in OutputPinsDisplayer)
                outputs.DisplayedWirePin.Disconnect(true);

            DialogEditor.Instance.RemoveNodeFromDialog(this);
        }

        public void OnLeftClick(Vector2 mousePosNodeSpace)
        {
            foreach (WirePinDisplayer inputs in InputPinsDisplayer)
            {
                if (inputs.InteractionRect.Contains(mousePosNodeSpace))
                    inputs.OnLeftClick();
            }
            foreach (WirePinDisplayer outputs in OutputPinsDisplayer)
            {
                if (outputs.InteractionRect.Contains(mousePosNodeSpace))
                    outputs.OnLeftClick();
            }
        }

        public T GetRenderedNodeAs<T>() where T : WireNode
        {
            return (T)RenderedNode;
        }
    }
}