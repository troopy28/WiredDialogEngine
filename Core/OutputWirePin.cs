using System;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public class OutputWirePin : WirePin
    {
        public override WirePinType PinType
        {
            get
            {
                return WirePinType.OUTPUT;
            }
        }

        public OutputWirePin()
        {
            // Used by NewtonSoft JSON.NET
        }

        public OutputWirePin(WireNode owner, WireDialog associatedDialog) : base(owner, associatedDialog)
        {

        }
    }
}