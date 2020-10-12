using Assets.WiredTools.WiredDialogEngine.LipSync;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    [CustomEditor(typeof(BSLipSync))]
    public class BSLipSyncInspector : UnityEditor.Editor
    {
        [SerializeField]
        private Texture2D resetButton;

        private BSLipSync lipSync;
        private bool foldOutEmotions;
        private bool foldOutLipSync;
        private bool foldOutLivePreview;
        private BSFacePoseDisplayer poseDisplayer = new BSFacePoseDisplayer();
        private AudioFaceType currentLipSyncDisplay = AudioFaceType.NONE;
        private Emotion currentEmotionDisplay = Emotion.Neutral;
        private GraphDrawer graphDrawer;
        private float stretching = 15;

        public void OnEnable()
        {
            graphDrawer = new GraphDrawer(64);
        }

        public override void OnInspectorGUI()
        {
            lipSync = (BSLipSync)target;
            DrawGeneralSettings();

            if (lipSync.MeshRenderer == null)
            {
                EditorGUILayout.LabelField("You must select the MeshRenderer to configure the LipSync system.");
            }
            else
            {
                if (EditorApplication.isPlaying)
                {
                    DrawLivePreview();
                }
                DrawEmotionsSettings();
                DrawLipSyncPoseSettings();
            }
            EditorUtility.SetDirty(lipSync);
        }

        public void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
            {
                DrawLivePreview();
            }
        }

        private void DrawGeneralSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            lipSync.MeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                new GUIContent("Target Renderer", "The SkinnedMeshRenderer used to display the blendshapes."),
                lipSync.MeshRenderer,
                typeof(SkinnedMeshRenderer),
                true
            );

            lipSync.SolverFrequency = Mathf.Clamp(EditorGUILayout.FloatField("Solver frequency", lipSync.SolverFrequency), 1, 100);
            lipSync.SoundSource = (AudioSource)EditorGUILayout.ObjectField(
                new GUIContent("Audio source", "The Unity audio source that will be used to synchronize the lips and the sound."),
                lipSync.SoundSource,
                typeof(AudioSource),
                true
                );

            lipSync.BlendingTime = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Blending time", "Time needed to blend between two lip poses."), lipSync.BlendingTime), 0.001f, 50f);
            lipSync.BlendingStep = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Blending step", "Time between two blending steps. The more there are, the better it looks, but it impacts on performances. Affects both lipsync and emotions."), lipSync.BlendingStep), 0.00001f, 50f);
            lipSync.AudioSensitivity = Mathf.Clamp(
                EditorGUILayout.FloatField(new GUIContent("Audio sensitivity", "Put a greater value to make the system react to small sound variations."), lipSync.AudioSensitivity), 0.01f, 10000f);

            EditorGUILayout.Separator();

            lipSync.EmotionBlendingTime = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Emotion blending time", "Time needed to blend between two emotions."), lipSync.EmotionBlendingTime), 0.001f, 50f);
            lipSync.EmotionIntensity = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Emotion intensity", "How much the emotions will be strong on your character's face."), lipSync.EmotionIntensity), 0.001f, 1f);
            lipSync.CurrentEmotion = (Emotion)EditorGUILayout.EnumPopup(new GUIContent("Current emotion"), lipSync.CurrentEmotion);

            EditorGUILayout.EndVertical();
        }

        private void DrawLivePreview()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldOutLivePreview = EditorGUILayout.Foldout(foldOutLivePreview, new GUIContent("Live preview"));

            if (foldOutLivePreview)
            {
                stretching = EditorGUILayout.Slider(
                    new GUIContent("Graph stretching", "Resize the lines of the graph. Only a visualization feature, it doesn't have any impact on the game."),
                    stretching, 1, 100);
                graphDrawer.AddValue(lipSync.CurrentVolume);
                graphDrawer.DrawGraph(stretching);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEmotionsSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foldOutEmotions = EditorGUILayout.Foldout(foldOutEmotions, new GUIContent("Emotions settings"));
            EditorGUI.indentLevel++;
            if (foldOutEmotions)
            {
                currentEmotionDisplay = (Emotion)EditorGUILayout.EnumPopup(new GUIContent("Emotion", "The emotion you want to edit."), currentEmotionDisplay);
                BSFacePose pose = poseDisplayer.Draw(lipSync.GetPoseFor(currentEmotionDisplay), lipSync);
                lipSync.SetEmotionPose(currentEmotionDisplay, pose);
            }

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void DrawLipSyncPoseSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foldOutLipSync = EditorGUILayout.Foldout(foldOutLipSync, new GUIContent("LipSync settings"));
            EditorGUI.indentLevel++;
            if (foldOutLipSync)
            {
                bool notNone = false;

                // SILENCE
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.SILENCE, new GUIContent("Silence pose", "The facial pose to give to your character whenever they don't talk.")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.SilencePose, lipSync);
                    currentLipSyncDisplay = AudioFaceType.SILENCE;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }

                // WHISPERING
                EditorGUILayout.BeginHorizontal();
                lipSync.WhisperingThreshold = EditorGUILayout.Slider(new GUIContent("Whispering threshold"), lipSync.WhisperingThreshold, 0, 0.1f);
                if (GUILayout.Button(new GUIContent("=", "Reset"), GUILayout.Width(20)))
                {
                    Undo.RecordObject(lipSync, "Reset _wsptshld");
                    lipSync.WhisperingThreshold = 1 / 500f;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.WHISPERING, new GUIContent("Whispering pose", "The facial pose to give to your character whenever they are whispering.")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.WhisperingPose, lipSync);
                    currentLipSyncDisplay = AudioFaceType.WHISPERING;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }

                // MURMURING
                EditorGUILayout.BeginHorizontal();
                lipSync.MurmuringThreshold = EditorGUILayout.Slider(new GUIContent("Murmuring threshold"), lipSync.MurmuringThreshold, 0, 0.3f);
                if (GUILayout.Button(new GUIContent("=", "Reset"), GUILayout.Width(20)))
                {
                    Undo.RecordObject(lipSync, "Reset _mrmthsld");
                    lipSync.MurmuringThreshold = 1 / 400f;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.MURMURING, new GUIContent("Murmuring pose", "The facial pose to give to your character whenever they are murmuring (louder than whispering).")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.MurmuringPose, lipSync);
                    currentLipSyncDisplay = AudioFaceType.MURMURING;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }

                // NORMAL
                EditorGUILayout.BeginHorizontal();
                lipSync.NormalThreshold = EditorGUILayout.Slider(new GUIContent("Normal threshold"), lipSync.NormalThreshold, 0, 0.8f);
                if (GUILayout.Button(new GUIContent("=", "Reset"), GUILayout.Width(20)))
                {
                    Undo.RecordObject(lipSync, "Reset _nrmthsld");
                    lipSync.NormalThreshold = 0.004f;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.NORMAL, new GUIContent("Normal pose", "The facial pose to give to your character whenever they he talks at a normal level.")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.NormalPose, lipSync);
                    currentLipSyncDisplay = AudioFaceType.NORMAL;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }

                // SHOUTING
                EditorGUILayout.BeginHorizontal();
                lipSync.ShoutingThreshold = EditorGUILayout.Slider(new GUIContent("Shouting threshold"), lipSync.ShoutingThreshold, 0, 1f);
                if (GUILayout.Button(new GUIContent("=", "Reset"), GUILayout.Width(20)))
                {
                    Undo.RecordObject(lipSync, "Reset _shtthsld");
                    lipSync.ShoutingThreshold = 3 / 500f;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.SHOUTING, new GUIContent("Shouting pose", "The facial pose to give to your character whenever they are bending your ears!")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.ShoutingPose, lipSync);
                    currentLipSyncDisplay = AudioFaceType.SHOUTING;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }
                if (!notNone)
                {
                    currentLipSyncDisplay = AudioFaceType.NONE;
                    ClearModel();
                }
            }

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void ClearModel()
        {
            int modelBlendshapesCount = lipSync.MeshRenderer.sharedMesh.blendShapeCount;
            for (int i = 0; i < modelBlendshapesCount; i++)
                lipSync.MeshRenderer.SetBlendShapeWeight(i, 0);
        }
    }
}