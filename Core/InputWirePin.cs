using System;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public class InputWirePin : WirePin
    {
        public override WirePinType PinType
        {
            get
            {
                return WirePinType.INPUT;
            }
        }

        public InputWirePin()
        {
            // Used by NewtonSoft JSON.NET
        }

        public InputWirePin(WireNode owner, WireDialog associatedDialog) : base(owner, associatedDialog)
        {

        }
    }
}