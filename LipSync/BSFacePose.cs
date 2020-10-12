using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    [Serializable]
    public class BSFacePose
    {
        private class ValuePair
        {
            public float PoseOne;
            public float PoseTwo;
        }

        [SerializeField]
        public List<BSBlendshape> Blendshapes;

        /// <summary>
        /// Returns a blendshape that has the specified index. Returns null if there isn't one.
        /// </summary>
        /// <param name="index">The target index.</param>
        public BSBlendshape GetBlendshapeForIndex(int index)
        {
            foreach (BSBlendshape bs in Blendshapes)
            {
                if (bs.Index == index)
                    return bs;
            }
            return null;
        }

        /// <summary>
        /// Returns an exact copy of this object.
        /// </summary>
        public BSFacePose GetCopy()
        {
            return new BSFacePose()
            {
                Blendshapes = new List<BSBlendshape>(Blendshapes)
            };
        }

        public static BSFacePose LerpEmotion(BSFacePose lipSync, BSFacePose emotion, float t)
        {
            if (lipSync == null && emotion != null)
                return emotion;
            else if (emotion == null && lipSync != null)
                return lipSync;
            else if (emotion == null && lipSync == null)
                throw new Exception("Can't lerp the poses: they are both null.");

            BSFacePose result = new BSFacePose()
            {
                Blendshapes = new List<BSBlendshape>()
            };

            Dictionary<int, float> lipsyncPoses = new Dictionary<int, float>();
            foreach (BSBlendshape lsBlendshape in lipSync.Blendshapes)
            {
                lipsyncPoses.Add(lsBlendshape.Index, lsBlendshape.Value);
            }

            foreach (BSBlendshape emBlendshape in emotion.Blendshapes)
            {
                if (lipsyncPoses.ContainsKey(emBlendshape.Index))
                {
                    //lipsyncPoses[emBlendshape.Index] = Mathf.Lerp(lipsyncPoses[emBlendshape.Index], emBlendshape.Value, t);
                    result.Blendshapes.Add(new BSBlendshape()
                    {
                        Index = emBlendshape.Index,
                        Value = Mathf.Lerp(lipsyncPoses[emBlendshape.Index], emBlendshape.Value, t)
                    });
                    lipsyncPoses.Remove(emBlendshape.Index);
                }
                else
                {
                    result.Blendshapes.Add(new BSBlendshape()
                    {
                        Index = emBlendshape.Index,
                        Value = emBlendshape.Value
                    });
                }
            }

            // For each lipsync blendshape that has not been blended with an emotion, just add it
            foreach (KeyValuePair<int, float> kvp in lipsyncPoses)
            {
                result.Blendshapes.Add(new BSBlendshape()
                {
                    Index = kvp.Key,
                    Value = kvp.Value
                });
            }
            lipsyncPoses.Clear();

            return result;
        }

        public static BSFacePose Lerp(BSFacePose one, BSFacePose two, float t)
        {
            if (one == null && two != null)
                return two;
            else if (two == null && one != null)
                return one;
            else if (two == null && one == null)
                throw new Exception("Can't lerp the poses: they are both null.");

            BSFacePose result = new BSFacePose()
            {
                Blendshapes = new List<BSBlendshape>()
            };

            // Map the poses
            Dictionary<int, ValuePair> map = new Dictionary<int, ValuePair>();
            foreach (BSBlendshape bsOne in one.Blendshapes)
            {
                map.Add(bsOne.Index, new ValuePair()
                {
                    PoseOne = bsOne.Value,
                    PoseTwo = 0
                });
            }
            foreach (BSBlendshape bsTwo in two.Blendshapes)
            {
                if (map.ContainsKey(bsTwo.Index))
                {
                    map[bsTwo.Index].PoseTwo = bsTwo.Value;
                }
                else
                {
                    map.Add(bsTwo.Index, new ValuePair()
                    {
                        PoseOne = 0,
                        PoseTwo = bsTwo.Value
                    });
                }
            }

            foreach (var entry in map.ToList())
            {
                int index = entry.Key;
                float valueOne = entry.Value.PoseOne;
                float valueTwo = entry.Value.PoseTwo;
                float finalValue = Mathf.Lerp(valueOne, valueTwo, t);
                result.Blendshapes.Add(new BSBlendshape()
                {
                    Index = index,
                    Value = finalValue
                });
            }

            //Debug.Log(sb.ToString());
            return result;
        }
    }

    public class BSPreset
    {
        public BSFacePose Silence;
        public BSFacePose Whispering;
        public BSFacePose Murmuring;
        public BSFacePose Normal;
        public BSFacePose Shouting;
    }

    public static class BSFacePosePresets
    {
        public static BSPreset MixamoStandardPreset
        {
            get
            {
                return new BSPreset()
                {
                    Silence = new BSFacePose()
                    {
                        Blendshapes = new List<BSBlendshape>()
                        {
                            new BSBlendshape()
                            {
                                Index = 35,
                                Value = 0
                            }
                        }
                    },
                    Whispering = new BSFacePose()
                    {
                        Blendshapes = new List<BSBlendshape>()
                        {
                            new BSBlendshape()
                            {
                                Index = 35
                            }
                        }
                    }
                };
            }
        }
    }
}