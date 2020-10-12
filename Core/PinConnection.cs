using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public class PinConnection
    {
        [HideInInspector]
        [SerializeField]
        private uint pinOne;
        public uint PinOne
        {
            get
            {
                return pinOne;
            }

            set
            {
                pinOne = value;
            }
        }
        [HideInInspector]
        [SerializeField]
        private uint pinTwo;
        public uint PinTwo
        {
            get
            {
                return pinTwo;
            }

            set
            {
                pinTwo = value;
            }
        }
    }
}