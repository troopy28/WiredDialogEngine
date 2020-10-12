using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    [Serializable]
    public class BSBlendshape
    {
        [SerializeField]
        public int Index;
        [SerializeField]
        public float Value;

        public BSBlendshape()
        {

        }

        public BSBlendshape(int index, float value)
        {
            Index = index;
            Value = value;
        }
    }
}