using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public abstract class WireNode
    {
        public string NodeName { get; set; }
        public uint WireNodeId { get; set; }
        public List<InputWirePin> Inputs { get; set; }
        public List<OutputWirePin> Outputs { get; set; }
        public float DisplayerX { get; set; }
        public float DisplayerY { get; set; }

        [JsonIgnore]
        public Vector2 DisplayerPosition
        {
            get
            {
                return new Vector2(DisplayerX, DisplayerY);
            }
            set
            {
                DisplayerX = value.x;
                DisplayerY = value.y;
            }
        }

        protected WireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        protected WireNode(WireDialog associatedDialog)
        {
            if (associatedDialog == null)
                return;

            Inputs = new List<InputWirePin>();
            Outputs = new List<OutputWirePin>();
            WireNodeId = associatedDialog.GetNextWireNodeId();
        }
    }
}