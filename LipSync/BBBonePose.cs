using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    [Serializable]
    public class BBBonePose
    {
        public Transform BoneTransform;
        public Quaternion LocalRotation;
        public Vector3 LocalPosition;

        public BBBonePose Copy()
        {
            return new BBBonePose()
            {
                BoneTransform = BoneTransform,
                LocalPosition = LocalPosition,
                LocalRotation = LocalRotation
            };
        }
    }
}