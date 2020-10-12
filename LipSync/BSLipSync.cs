using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    /// <summary>
    /// Bone based lipsync.
    /// </summary>
    [AddComponentMenu("Wired Dialog Engine/Blendshapes based LipSync")]
    public class BSLipSync : MonoBehaviour
    {
        public float SolverFrequency = 25f;

        public AudioSource SoundSource;
        public SkinnedMeshRenderer MeshRenderer;

        private const int SampleSize = 64;
        private Coroutine solverCoroutine;
        private Coroutine emotionCoroutine;
        private float[] sample;
        private float vol;
        private AudioFaceType currentType;
        private AudioFaceType lastType;
        private Emotion lastEmotion;
        private Coroutine blendCoroutine;
        private BSFacePose currentLipsyncOnly;
        private BSFacePose currentEmotionOnly;

        /// <summary>
        /// Time needed to blend between two poses.
        /// </summary>
        public float BlendingTime = 0.3f;
        /// <summary>
        /// Time between two blending steps. The more there are, the better it looks, but it impacts on performances.
        /// </summary>
        public float BlendingStep = 0.01f;
        /// <summary>
        /// Put a greater value to make the system react to small sound variations.
        /// </summary>
        public float AudioSensitivity = 1f;

        public float EmotionBlendingTime;

        public BSFacePose SilencePose;
        public float WhisperingThreshold = 0.0001f;
        public BSFacePose WhisperingPose;
        public float MurmuringThreshold = 0.09f;
        public BSFacePose MurmuringPose;
        public float NormalThreshold = 0.105f;
        public BSFacePose NormalPose;
        public float ShoutingThreshold = 0.12f;
        public BSFacePose ShoutingPose;

        public List<BSLabelledEmotionPose> EmotionPoses;
        public Emotion CurrentEmotion;
        public float EmotionIntensity;

        /// <summary>
        /// The pose currently used. Can be a blending between the last and the next pose if animating.
        /// </summary>
        public BSFacePose CurrentPose { get; private set; }
        /// <summary>
        /// The pose the current lipsync blending aims to go to.
        /// </summary>
        public BSFacePose TargetLipSyncPose { get; private set; }
        /// <summary>
        /// The pose the current emotion blending aims to go to.
        /// </summary>
        public BSFacePose TargetEmotionPose { get; private set; }
        public bool BlendingLipSync { get; private set; }
        /// <summary>
        /// The pose that SHOULD be the current one, according to the current emotion.
        /// </summary>
        public BSFacePose CurrentCompleteEmotionPose
        {
            get
            {
                return GetPoseFor(CurrentEmotion);
            }
            set
            {
                SetEmotionPose(CurrentEmotion, value);
            }
        }

        public float CurrentVolume
        {
            get
            {
                return vol;
            }
        }


        public void Awake()
        {
            CurrentPose = SilencePose;
            lastEmotion = CurrentEmotion;
            currentLipsyncOnly = SilencePose;
            currentEmotionOnly = CurrentCompleteEmotionPose;
        }

        public void Start()
        {
            if (SoundSource == null)
            {
                Debug.LogError("ERROR: the AudioSource of the Blendshapes Based LipSync of the game object " + gameObject.name + " isn't set. Stopping it.");
                enabled = false;
                return;
            }
            solverCoroutine = StartCoroutine(SolverCoroutine());
            DisplayPoseOnModel(GetPoseFor(CurrentEmotion));
        }

        private IEnumerator BlendEmotionsCoroutine(BSFacePose start, BSFacePose target)
        {
            TargetEmotionPose = target;
            for (float i = 0; i < EmotionBlendingTime; i += BlendingStep)
            {
                currentEmotionOnly = BSFacePose.Lerp(start, target, i / (EmotionBlendingTime));
                CurrentPose = MixLipSyncAndEmotion(currentLipsyncOnly, currentEmotionOnly);
                DisplayPoseOnModel(CurrentPose);
                yield return new WaitForSeconds(BlendingStep);
            }
        }

        private IEnumerator BlendLipSyncCoroutine(BSFacePose one, BSFacePose two)
        {
            BlendingLipSync = true;
            TargetLipSyncPose = two;
            for (float i = 0; i < BlendingTime; i += BlendingStep)
            {
                currentLipsyncOnly = BSFacePose.Lerp(one, two, i / BlendingTime);
                CurrentPose = MixLipSyncAndEmotion(currentLipsyncOnly, currentEmotionOnly);
                DisplayPoseOnModel(CurrentPose);
                yield return new WaitForSeconds(BlendingStep);
            }
            TargetLipSyncPose = null;

            currentLipsyncOnly = two;
            CurrentPose = MixLipSyncAndEmotion(two, CurrentCompleteEmotionPose);
            DisplayPoseOnModel(CurrentPose);

            BlendingLipSync = false;
        }

        private BSFacePose MixLipSyncAndEmotion(BSFacePose lipSync, BSFacePose emotion)
        {
            return BSFacePose.LerpEmotion(lipSync, emotion, EmotionIntensity);
        }

        private void DisplayPoseOnModel(BSFacePose pose)
        {
            if (MeshRenderer == null)
                return;

            int modelBlendshapesCount = MeshRenderer.sharedMesh.blendShapeCount;
            for (int i = 0; i < modelBlendshapesCount; i++)
            {
                IEnumerable<BSBlendshape> correspondingPoseBlendshapes = pose.Blendshapes.Where(curr => curr.Index == i);
                int actualCount = correspondingPoseBlendshapes.Count();
                // If there is at least one pose
                if (actualCount != 0)
                {
                    // If there are several poses, it's a problem: signal it
                    if (actualCount > 1)
                    {
                        Debug.LogError("DANGER: several blendshapes are defined in your pose with the same index. The first in the list will be used.");
                    }
                    BSBlendshape firstBlendshape = correspondingPoseBlendshapes.First();
                    MeshRenderer.SetBlendShapeWeight(i, firstBlendshape.Value);
                }
                // Otherwise there is no pose
                else
                {
                    MeshRenderer.SetBlendShapeWeight(i, 0);
                }
            }
        }

        private IEnumerator SolverCoroutine()
        {
            while (gameObject.activeInHierarchy)
            {
                if (SoundSource == null)
                    yield return new WaitForSeconds(1 / SolverFrequency);

                float total = 0;
                sample = new float[SampleSize];
                SoundSource.GetSpectrumData(sample, 0, FFTWindow.BlackmanHarris);
                for (int i = 0; i < SampleSize; i++)
                {
                    total += sample[i];
                }
                vol = total / SampleSize;
                vol *= AudioSensitivity;

                if (lastEmotion != CurrentEmotion)
                {
                    if (emotionCoroutine != null)
                    {
                        StopCoroutine(emotionCoroutine);
                        emotionCoroutine = null;
                    }
                    emotionCoroutine = StartCoroutine(BlendEmotionsCoroutine(currentEmotionOnly, GetPoseFor(CurrentEmotion)));
                    Debug.Log("Starting a new blending from " + lastEmotion + " to " + CurrentEmotion);
                    lastEmotion = CurrentEmotion;
                }

                AnalyseAudio();
                yield return new WaitForSeconds(1 / SolverFrequency);
                lastType = currentType;
            }
        }

        private void AnalyseAudio()
        {
            if (vol < WhisperingThreshold)
            {
                // Silence
                currentType = AudioFaceType.SILENCE;
                if (lastType != currentType)
                {
                    if (CurrentPose != SilencePose && TargetLipSyncPose != SilencePose)
                    {
                        if (blendCoroutine != null)
                            StopCoroutine(blendCoroutine);
                        blendCoroutine = StartCoroutine(BlendLipSyncCoroutine(currentLipsyncOnly, SilencePose));
                    }
                }
            }
            else if (vol < MurmuringThreshold)
            {
                // Whispering
                currentType = AudioFaceType.WHISPERING;
                if (lastType != currentType)
                {
                    if (CurrentPose != WhisperingPose && TargetLipSyncPose != WhisperingPose)
                    {
                        if (blendCoroutine != null)
                            StopCoroutine(blendCoroutine);
                        blendCoroutine = StartCoroutine(BlendLipSyncCoroutine(currentLipsyncOnly, WhisperingPose));
                    }
                }
            }
            else if (vol < NormalThreshold)
            {
                // Murmuring
                currentType = AudioFaceType.MURMURING;
                if (lastType != currentType)
                {
                    if (CurrentPose != MurmuringPose && TargetLipSyncPose != MurmuringPose)
                    {
                        if (blendCoroutine != null)
                            StopCoroutine(blendCoroutine);
                        blendCoroutine = StartCoroutine(BlendLipSyncCoroutine(currentLipsyncOnly, MurmuringPose));
                    }
                }
            }
            else if (vol < ShoutingThreshold)
            {
                // Normal
                currentType = AudioFaceType.NORMAL;
                if (lastType != currentType)
                {
                    if (CurrentPose != NormalPose && TargetLipSyncPose != NormalPose)
                    {
                        if (blendCoroutine != null)
                            StopCoroutine(blendCoroutine);
                        blendCoroutine = StartCoroutine(BlendLipSyncCoroutine(currentLipsyncOnly, NormalPose));
                    }
                }
            }
            else
            {
                // Shouting
                currentType = AudioFaceType.SHOUTING;
                if (lastType != currentType)
                {
                    if (CurrentPose != NormalPose && TargetLipSyncPose != NormalPose)
                    {
                        if (blendCoroutine != null)
                            StopCoroutine(blendCoroutine);
                        blendCoroutine = StartCoroutine(BlendLipSyncCoroutine(currentLipsyncOnly, ShoutingPose));
                    }
                }
            }
        }

        public void SetEmotionPose(Emotion emotion, BSFacePose pose)
        {
            if (EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).Count() > 0)
                EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).First().Pose = pose;
            else
                EmotionPoses.Add(new BSLabelledEmotionPose()
                {
                    AssociatedEmotion = emotion,
                    Pose = pose
                });
        }

        public BSFacePose GetPoseFor(Emotion emotion)
        {
            if (EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).Count() > 0)
                return EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).First().Pose;
            return null;
        }
    }
}