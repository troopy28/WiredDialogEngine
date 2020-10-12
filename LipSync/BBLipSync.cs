using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.LipSync
{
    /// <summary>
    /// Bone based lipsync.
    /// </summary>
    [AddComponentMenu("Wired Dialog Engine/Bones based LipSync")]
    public class BBLipSync : MonoBehaviour
    {
        public float SolverFrequency = 25f;

        public AudioSource SoundSource;

        private const int SampleSize = 64;
        private Coroutine solverCoroutine;
        private Coroutine emotionCoroutine;
        private float[] sample;
        private float vol;
        private AudioFaceType currentType;
        private AudioFaceType lastType;
        private Emotion lastEmotion;

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

        public BBFacePose DefaultPose;
        public BBFacePose SilencePose;
        public float WhisperingThreshold = 0.0001f;
        public BBFacePose WhisperingPose;
        public float MurmuringThreshold = 0.09f;
        public BBFacePose MurmuringPose;
        public float NormalThreshold = 0.105f;
        public BBFacePose NormalPose;
        public float ShoutingThreshold = 0.12f;
        public BBFacePose ShoutingPose;

        public List<BBLabelledEmotionPose> EmotionPoses;
        public Emotion CurrentEmotion;
        public float EmotionIntensity;

        /// <summary>
        /// The pose currently used. Can be a blending between the last and the next pose if animating.
        /// </summary>
        public BBFacePose CurrentPose { get; private set; }
        /// <summary>
        /// The pose the current blending aims to go to.
        /// </summary>
        public BBFacePose TargetLipSyncPose { get; private set; }
        public bool BlendingLipSync { get; private set; }

        public float CurrentVolume
        {
            get
            {
                return vol;
            }
        }

        private Coroutine blendCoroutine;
        private BBFacePose currentLipsyncOnly;
        private BBFacePose currentEmotionOnly;

        /// <summary>
        /// The pose the current emotion blending aims to go to.
        /// </summary>
        public BBFacePose TargetEmotionPose { get; private set; }

        /// <summary>
        /// The pose that SHOULD be the current one, according to the current emotion.
        /// </summary>
        public BBFacePose CurrentCompleteEmotionPose
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
                Debug.LogError("ERROR: the AudioSource of the Bones Based LipSync of the game object " + gameObject.name + " isn't set. Stopping it.");
                enabled = false;
                return;
            }
            solverCoroutine = StartCoroutine(SolverCoroutine());
            DisplayPoseOnModel(GetPoseFor(CurrentEmotion));
        }

        private IEnumerator BlendEmotionsCoroutine(BBFacePose start, BBFacePose target)
        {
            TargetEmotionPose = target;
            for (float i = 0; i < BlendingTime * 3; i += BlendingStep)
            {
                currentEmotionOnly = BBFacePose.Lerp(start, target, DefaultPose, i / (BlendingTime * 3));
                CurrentPose = MixLipSyncAndEmotion(currentLipsyncOnly, currentEmotionOnly);
                DisplayPoseOnModel(CurrentPose);
                yield return new WaitForSeconds(BlendingStep);
            }
        }

        private IEnumerator BlendLipSyncCoroutine(BBFacePose one, BBFacePose two)
        {
            BlendingLipSync = true;
            TargetLipSyncPose = two;
            for (float i = 0; i < BlendingTime; i += BlendingStep)
            {
                currentLipsyncOnly = BBFacePose.Lerp(one, two, DefaultPose, i / BlendingTime);
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

        private BBFacePose MixLipSyncAndEmotion(BBFacePose lipSync, BBFacePose emotion)
        {
            return BBFacePose.LerpEmotion(lipSync, emotion, DefaultPose, EmotionIntensity);
        }

        private void DisplayPoseOnModel(BBFacePose pose)
        {
            pose.BonePoses.ForEach(bone =>
            {
                bone.BoneTransform.localPosition = bone.LocalPosition;
                bone.BoneTransform.localRotation = bone.LocalRotation;
            });
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

        public void SetEmotionPose(Emotion emotion, BBFacePose pose)
        {
            if (EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).Count() > 0)
                EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).First().Pose = pose;
            else
                EmotionPoses.Add(new BBLabelledEmotionPose()
                {
                    AssociatedEmotion = emotion,
                    Pose = pose
                });
        }

        public BBFacePose GetPoseFor(Emotion emotion)
        {
            if (EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).Count() > 0)
                return EmotionPoses.Where(lPose => lPose.AssociatedEmotion == emotion).First().Pose;
            return null;
        }
    }
}