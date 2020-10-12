using Assets.WiredTools.WiredDialogEngine.LipSync;
using System.Diagnostics; // Stopwatch
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    [CustomEditor(typeof(BBLipSync))]
    public class BBLipSyncInspector : UnityEditor.Editor
    {
        [SerializeField]
        private Texture2D resetButton;

        private BBLipSync lipSync;
        private bool foldOutEmotions;
        private bool foldOutLipSync;
        private bool foldOutLivePreview;
        private BBFacePoseDisplayer poseDisplayer = new BBFacePoseDisplayer();
        private AudioFaceType currentLipSyncDisplay = AudioFaceType.NONE;
        private Emotion currentEmotionDisplay = Emotion.Neutral;
        private GraphDrawer graphDrawer;
        private float stretching = 15;
        private float skeletonTextDistance = 1.5f;
        private bool drawSkeleton;
        private bool autoPose;
        public bool showDefaultPose;
        private Vector2 defaultPoseScroll;

        public BBLipSync TargetLipSync
        {
            get
            {
                return lipSync;
            }
        }

        public bool AutoPose
        {
            get
            {
                return autoPose;
            }

            set
            {
                autoPose = value;
            }
        }

        public void OnEnable()
        {
            graphDrawer = new GraphDrawer(64);
        }

        public override void OnInspectorGUI()
        {
            lipSync = (BBLipSync)target;
            DrawGeneralSettings();

            if (EditorApplication.isPlaying)
            {
                DrawLivePreview();
            }
            DrawEmotionsSettings();
            DrawLipSyncPoseSettings();

            EditorUtility.SetDirty(lipSync);
        }

        public void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
            {
                DrawLivePreview();
            }
            else
            {
                if (drawSkeleton)
                {
                    Transform selectedBone = Selection.activeTransform;
                    if (selectedBone != null)
                    {
                        Transform parentBone = selectedBone.parent;
                        if (parentBone != null)
                        {
                            Handles.DrawLine(selectedBone.position, parentBone.position);
                            Color prev = Handles.color;
                            Handles.color = Color.magenta;
                            Handles.DrawWireCube(selectedBone.position, selectedBone.localScale * 0.005f);
                            Handles.color = prev;
                            foreach (Transform child in parentBone)
                            {
                                if (child != selectedBone)
                                {
                                    Handles.DrawDottedLine(child.position, parentBone.position, 3);
                                    Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                                    if (Vector3.Distance(cameraPosition, child.position) < skeletonTextDistance)
                                        Handles.Label(child.position, child.name);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void DrawGeneralSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            lipSync.SolverFrequency = Mathf.Clamp(EditorGUILayout.FloatField("Solver frequency", lipSync.SolverFrequency), 1, 100);
            lipSync.SoundSource = (AudioSource)EditorGUILayout.ObjectField(
                new GUIContent("Audio source", "The Unity audio source that will be used to synchronize the lips and the sound."),
                lipSync.SoundSource,
                typeof(AudioSource),
                true
                );

            lipSync.BlendingTime = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Blending time", "Time needed to blend between two poses."), lipSync.BlendingTime), 0.001f, 50f);
            lipSync.BlendingStep = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Blending step", "Time between two blending steps. The more there are, the better it looks, but it impacts on performances."), lipSync.BlendingStep), 0.00001f, 50f);
            lipSync.AudioSensitivity = Mathf.Clamp(
                EditorGUILayout.FloatField(new GUIContent("Audio sensitivity", "Put a greater value to make the system react to small sound variations."), lipSync.AudioSensitivity), 0.01f, 10000f);

            EditorGUILayout.Separator();

            lipSync.EmotionBlendingTime = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Emotion blending time", "Time needed to blend between two emotions."), lipSync.EmotionBlendingTime), 0.001f, 50f);
            lipSync.EmotionIntensity = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Emotion intensity", "How much the emotions will be strong on your character's face."), lipSync.EmotionIntensity), 0.001f, 1f);
            lipSync.CurrentEmotion = (Emotion)EditorGUILayout.EnumPopup(new GUIContent("Current emotion"), lipSync.CurrentEmotion);

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            showDefaultPose = EditorGUILayout.Foldout(showDefaultPose, new GUIContent("Default pose", "The pose of your character if no pose is to be played. Often the base look of your character."));
            if (GUILayout.Button(new GUIContent("Get from prefab", "Get the default pose from the prefab pose. It must contain every bone that can be used during an animation.")))
            {
                GetDefaultPoseFromPrefab();
            }
            EditorGUILayout.EndHorizontal();

            if (showDefaultPose)
            {
                defaultPoseScroll = EditorGUILayout.BeginScrollView(defaultPoseScroll);
                EditorGUI.indentLevel++;
                lipSync.DefaultPose = poseDisplayer.Draw(lipSync.DefaultPose, lipSync, this);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndScrollView();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            drawSkeleton = EditorGUILayout.Toggle(new GUIContent("Draw skeleton"), drawSkeleton);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            skeletonTextDistance = EditorGUILayout.FloatField(new GUIContent("Skeleton text distance"), skeletonTextDistance);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            autoPose = EditorGUILayout.Toggle(new GUIContent("Auto pose", "Automatically change the face pose bones to match with the modifications you do in the scene."), autoPose);
            EditorGUILayout.EndHorizontal();

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

        private void GetDefaultPoseFromPrefab()
        {
            // Just show the window. It will manage furnishing the correct pose when the user will click OK.
            BBGetDefaultPoseWindow pose = BBGetDefaultPoseWindow.CreateWindow(this);
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
                BBFacePose pose = poseDisplayer.Draw(lipSync.GetPoseFor(currentEmotionDisplay), lipSync, this);
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
                EditorGUI.BeginChangeCheck();
                // SILENCE
                if (EditorGUILayout.Foldout(currentLipSyncDisplay == AudioFaceType.SILENCE, new GUIContent("Silence pose", "The facial pose to give to your character whenever they don't talk.")))
                {
                    EditorGUI.indentLevel++;
                    poseDisplayer.Draw(lipSync.SilencePose, lipSync, this);
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
                    poseDisplayer.Draw(lipSync.WhisperingPose, lipSync, this);
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
                    poseDisplayer.Draw(lipSync.MurmuringPose, lipSync, this);
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
                    poseDisplayer.Draw(lipSync.NormalPose, lipSync, this);
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
                    poseDisplayer.Draw(lipSync.ShoutingPose, lipSync, this);
                    currentLipSyncDisplay = AudioFaceType.SHOUTING;
                    notNone = true;
                    EditorGUI.indentLevel--;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    poseDisplayer.Clear();
                    ClearModel();
                }

                if (!notNone)
                {
                    currentLipSyncDisplay = AudioFaceType.NONE;
                    ClearModel();
                    poseDisplayer.Clear();
                }
            }

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void ClearModel()
        {
            if (lipSync.DefaultPose.BonePoses.Count > 0)
            {
                lipSync.DefaultPose.BonePoses.Where(bone => bone != null && bone.BoneTransform != null).ToList().ForEach(bone =>
                {
                    bone.BoneTransform.localPosition = bone.LocalPosition;
                    bone.BoneTransform.localRotation = bone.LocalRotation;
                });
            }
            else
            {
                EditorStyles.label.wordWrap = true;
                Color prev = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Error: the default face pose is not defined for " + lipSync.gameObject.name + ". Strange behaviours may occur.");
                EditorGUILayout.EndHorizontal();
                GUI.color = prev;
            }
        }
    }
}