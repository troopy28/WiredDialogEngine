using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public abstract class WirePin
    {
        public uint OwnerId { get; set; }
        public uint WirePinId { get; set; }
        public Type DataType { get; set; }
        public abstract WirePinType PinType { get; }
        public uint ConnectedPinId { get; set; }
        public object Value { get; set; }
        public string PinName { get; set; }
        public List<string> RefusedTypes { get; set; }
        public List<string> RefusedNodes { get; set; }

        [JsonIgnore]
        public bool IsConnected
        {
            get
            {
                return ConnectedPinId != 0;
            }
        }
        [JsonIgnore]
        public object WirePinDisplayer { get; set; }
        [JsonIgnore]
        public WireDialog AssociatedDialog { get; set; }

        protected WirePin()
        {
            // Used by NewtonSoft JSON.NET
        }

        protected WirePin(WireNode owner, WireDialog associatedDialog)
        {
            SetOwner(owner);
            RefusedTypes = new List<string>();
            RefusedNodes = new List<string>();
            WirePinId = associatedDialog.GetNextWirePinId();
            AssociatedDialog = associatedDialog;

            if (!AssociatedDialog.Pins.ContainsKey(WirePinId))
                AssociatedDialog.Pins.Add(WirePinId, this);

        }

        public virtual bool Connect(WirePin other, bool hasInitiative)
        {
            bool differentPinTypes = other.PinType != PinType;
            bool notRefusedType = !RefusedTypes.Contains(other.DataType.Name);
            bool dataTypesCorresponding = other.DataType == DataType || other.DataType.IsSubclassOf(DataType) || DataType.IsSubclassOf(other.DataType);
            bool differentOwners = other.GetOwner() != GetOwner();
            Type otherType = other.GetOwner().GetType();
            bool acceptedNode = !RefusedNodes.Contains(otherType.Name) && !other.RefusedNodes.Contains(GetOwner().GetType().Name);

            if (differentPinTypes && notRefusedType && dataTypesCorresponding && differentOwners && acceptedNode)
            {
                if (GetConnectedPin() != null) // If already connected, disconnect from the other
                    Disconnect(true);

                ConnectedPinId = other.WirePinId;

                if (hasInitiative)
                    GetConnectedPin().Connect(this, false); // We connect the other node to this one, but without the initiative    

                return true;
            }
            ConnectedPinId = 0;
            return false;
        }

        public virtual void Disconnect(bool hasInitiative)
        {
            if (ConnectedPinId == 0)
                return;
            if (hasInitiative)
                GetConnectedPin().Disconnect(false);
            ConnectedPinId = 0;
        }

        public virtual T GetValue<T>()
        {
            return (T)Value;
        }

        public WireNode GetOwner()
        {
            return AssociatedDialog.NodesDictionary[OwnerId];
        }

        protected void SetOwner(WireNode value)
        {
            if (value != null)
                OwnerId = value.WireNodeId;
        }

        /// <summary>
        /// Returns the <see cref="WirePin"/> connected to this <see cref="WirePin"/>.
        /// </summary>
        /// <returns>Returns the <see cref="WirePin"/> connected to this <see cref="WirePin"/>.</returns>
        public WirePin GetConnectedPin()
        {
            if (!IsConnected)
                return null;
            return AssociatedDialog.Pins[ConnectedPinId];
        }
    }
}