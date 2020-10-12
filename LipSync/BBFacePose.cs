using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    /// <summary>
    /// Bone based lipsync pose.
    /// </summary>
    [Serializable]
    public class BBFacePose
    {
        private class MappedPose
        {
            public Vector3 Pos;
            public Quaternion Rot;
        }

        private class PosePair
        {
            public Vector3 PosOne;
            public Quaternion RotationOne;

            public Vector3 PosTwo;
            public Quaternion RotationTwo;
        }

        public List<BBBonePose> BonePoses;
        public Transform Root;

        public static BBFacePose LerpEmotion(BBFacePose lipSync, BBFacePose emotion, BBFacePose defaultPose, float t)
        {
            if (lipSync == null && emotion != null)
                return emotion;
            else if (emotion == null && lipSync != null)
                return lipSync;
            else if (emotion == null && lipSync == null)
                throw new Exception("Can't lerp the poses: they are both null.");

            BBFacePose result = new BBFacePose()
            {
                BonePoses = new List<BBBonePose>(),
                Root = lipSync.Root // Will maybe generate troubles...
            };

            // Map the poses
            Dictionary<Transform, MappedPose> map = new Dictionary<Transform, MappedPose>();
            foreach (BBBonePose boneOne in lipSync.BonePoses)
            {
                map.Add(boneOne.BoneTransform, new MappedPose()
                {
                    Pos = boneOne.LocalPosition,
                    Rot = boneOne.LocalRotation
                });
            }
            foreach (BBBonePose boneTwo in emotion.BonePoses)
            {
                if (map.ContainsKey(boneTwo.BoneTransform))
                {
                    result.BonePoses.Add(new BBBonePose()
                    {
                        BoneTransform = boneTwo.BoneTransform,
                        LocalPosition = Vector3.Lerp(map[boneTwo.BoneTransform].Pos, boneTwo.LocalPosition, t),
                        LocalRotation = Quaternion.Lerp(map[boneTwo.BoneTransform].Rot, boneTwo.LocalRotation, t)
                    });
                    map.Remove(boneTwo.BoneTransform);
                }
                else // If there is no pose for lipsync associated to this blendshape, then use it.
                {
                    result.BonePoses.Add(new BBBonePose()
                    {
                        BoneTransform = boneTwo.BoneTransform,
                        LocalPosition = boneTwo.LocalPosition,
                        LocalRotation = boneTwo.LocalRotation
                    });
                }
            }

            foreach(KeyValuePair<Transform, MappedPose> kvp in map)
            {
                result.BonePoses.Add(new BBBonePose()
                {
                    BoneTransform = kvp.Key,
                    LocalPosition = kvp.Value.Pos,
                    LocalRotation = kvp.Value.Rot
                });
            }
            return result;
        }

        public static BBFacePose Lerp(BBFacePose one, BBFacePose two, BBFacePose defaultPose, float t)
        {
            if (one == null && two != null)
                return two;
            else if (two == null && one != null)
                return one;
            else if (two == null && one == null)
                throw new Exception("Can't lerp the poses: they are both null.");

            BBFacePose result = new BBFacePose()
            {
                BonePoses = new List<BBBonePose>()
            };

            // Map the poses
            Dictionary<Transform, PosePair> map = new Dictionary<Transform, PosePair>();
            foreach (BBBonePose boneOne in one.BonePoses)
            {
                BBBonePose defaultVal = defaultPose.BonePoses.Where(pose => pose.BoneTransform.Equals(boneOne.BoneTransform)).First();
                map.Add(boneOne.BoneTransform, new PosePair()
                {
                    PosOne = boneOne.LocalPosition,
                    PosTwo = defaultVal.LocalPosition,
                    RotationOne = boneOne.LocalRotation,
                    RotationTwo = defaultVal.LocalRotation,
                });
            }
            foreach (BBBonePose boneTwo in two.BonePoses)
            {
                if (map.ContainsKey(boneTwo.BoneTransform))
                {
                    PosePair pair = map[boneTwo.BoneTransform];
                    pair.PosTwo = boneTwo.LocalPosition;
                    pair.RotationTwo = boneTwo.LocalRotation;
                }
                else
                {
                    BBBonePose defaultVal = defaultPose.BonePoses.Where(pose => pose.BoneTransform.Equals(boneTwo.BoneTransform)).First();
                    map.Add(boneTwo.BoneTransform, new PosePair()
                    {
                        PosOne = defaultVal.LocalPosition,
                        PosTwo = boneTwo.LocalPosition,
                        RotationOne = defaultVal.LocalRotation,
                        RotationTwo = boneTwo.LocalRotation
                    });
                }
            }

            foreach (var entry in map.ToList())
            {
                Transform transform = entry.Key;
                Vector3 posOne = entry.Value.PosOne;
                Vector3 posTwo = entry.Value.PosTwo;
                Quaternion rotOne = entry.Value.RotationOne;
                Quaternion rotTwo = entry.Value.RotationTwo;

                Vector3 finalPos = Vector3.LerpUnclamped(posOne, posTwo, t);
                Quaternion finalRotation = Quaternion.LerpUnclamped(rotOne, rotTwo, t);

                result.BonePoses.Add(new BBBonePose()
                {
                    BoneTransform = transform,
                    LocalPosition = finalPos,
                    LocalRotation = finalRotation
                });
            }

            //Debug.Log(sb.ToString());
            return result;
        }
    }
}