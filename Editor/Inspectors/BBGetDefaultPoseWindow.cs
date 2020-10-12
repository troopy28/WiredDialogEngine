using Assets.WiredTools.WiredDialogEngine.LipSync;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    public class BBGetDefaultPoseWindow : EditorWindow
    {
        public static bool CallingRemoving { get; private set; }

        private Transform realArmatureRoot;
        private BBFacePose retrievedPose;
        private BBLipSync lipSync;
        private BBLipSyncInspector lipSyncInspector;
        private BBFacePoseDisplayer poseDisplayer = new BBFacePoseDisplayer();
        private Vector2 currScrollPos;

        public static BBGetDefaultPoseWindow CreateWindow(BBLipSyncInspector lipSyncInspector)
        {
            return GetWindow<BBGetDefaultPoseWindow>(true, "Get default pose").Init(lipSyncInspector);
        }

        public BBGetDefaultPoseWindow Init(BBLipSyncInspector lipSyncInspector)
        {
            lipSync = lipSyncInspector.TargetLipSync;
            this.lipSyncInspector = lipSyncInspector;
            retrievedPose = lipSync.DefaultPose;
            realArmatureRoot = lipSync.DefaultPose != null ? lipSync.DefaultPose.Root : null;
            return this;
        }

        private void OnGUI()
        {
            realArmatureRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Root armature", "The root of the armature (skeleton) of your character."), realArmatureRoot, typeof(Transform), true);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Get default pose")))
            {
                Undo.RecordObject(this, "Retrieve default pose");

                RetrievePose();
            }
            else if (GUILayout.Button("Clear"))
            {
                Undo.RecordObject(this, "Clear default pose");

                realArmatureRoot = null;
                retrievedPose = new BBFacePose()
                {
                    BonePoses = new List<BBBonePose>()
                };
            }
            EditorGUILayout.EndHorizontal();

            if (retrievedPose != null && retrievedPose.BonePoses != null)
            {
                CallingRemoving = true;
                currScrollPos = EditorGUILayout.BeginScrollView(currScrollPos);

                if (retrievedPose.BonePoses.Count > 100)
                    EditorGUILayout.HelpBox("Your skeleton has many bones, leading to heavy computations. Maybe you should try to reduce the amount of bones.", MessageType.Warning);

                poseDisplayer.Draw(retrievedPose, lipSync, lipSyncInspector);
                CallingRemoving = false;
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("No pose currently retrieved", MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Confirm"))
            {
                ConfirmPose();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("If the window was to close without you clicked (after an alt+tab for instance), the editor might be blocked. If so, just press escape to quit the window.", MessageType.Warning);
        }

        private void RetrievePose()
        {
            if (realArmatureRoot == null)
            {
                EditorUtility.DisplayDialog("Undefined armature", "To retrieve the default pose, the armature root must be defined, as all its child will compose the pose.", "Got it!");
                return;
            }
            GameObject lipSyncHolder = lipSync.gameObject;
            GameObject prefab = PrefabUtility.FindPrefabRoot(lipSyncHolder);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Can't find prefab", "Unable to find the prefab of your character. To find the prefab, the LipSync script must be placed on the game object being the root of the prefab.", "Got it!");
                return;
            }

            retrievedPose = new BBFacePose()
            {
                BonePoses = new List<BBBonePose>(),
                Root = realArmatureRoot
            };

            Transform prefabArmatureRoot = null;

            // Try to get the armature if it is the first child of the object, and if it has the same indices and everything
            if (prefab.transform.childCount >= realArmatureRoot.GetSiblingIndex())
            {
                Transform sameIndexTransform = prefab.transform.GetChild(realArmatureRoot.GetSiblingIndex());
                if (sameIndexTransform != null)
                {
                    if (sameIndexTransform.name.Equals(realArmatureRoot.name))
                    {
                        prefabArmatureRoot = sameIndexTransform;
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Armature mismatch", "The prefab armature and the instance armature are not the same. The LipSync system require them to be the same in order to work.", "OK");
                return;
            }

            // If we didn't find the prefab armature root, find it the hard way
            if (prefabArmatureRoot == null)
            {
                foreach (Transform child in prefab.transform)
                {
                    if (child.name == realArmatureRoot.name)
                        prefabArmatureRoot = child;
                    else
                    {
                        if (FindArmatureRoot(child, realArmatureRoot.name, out prefabArmatureRoot))
                            break;
                    }
                }
            }

            if (prefabArmatureRoot == null)
            {
                EditorUtility.DisplayDialog("Unable to find prefab armature root", "The prefab armature and the instance armature are not the same. The LipSync system require them to be the same in order to work. Can't continue.", "OK");
                return;
            }

            retrievedPose.BonePoses.Add(new BBBonePose()
            {
                BoneTransform = realArmatureRoot,
                LocalPosition = prefabArmatureRoot.localPosition,
                LocalRotation = prefabArmatureRoot.localRotation
            });
            BrowseTransform(realArmatureRoot, prefabArmatureRoot);
        }

        private bool FindArmatureRoot(Transform trans, string armatureName, out Transform armature)
        {
            if (trans.name.Equals(armatureName))
            {
                armature = trans;
                return true;
            }
            foreach (Transform child in trans)
            {
                bool found = FindArmatureRoot(child, armatureName, out armature);
                if (found)
                    return true;
            }
            armature = null;
            return false;
        }

        private void BrowseTransform(Transform realBone, Transform prefabBone)
        {
            foreach (Transform child in realBone)
            {
                Transform childPrefab = prefabBone.GetChild(child.GetSiblingIndex());
                retrievedPose.BonePoses.Add(new BBBonePose()
                {
                    BoneTransform = child,
                    LocalPosition = childPrefab.localPosition,
                    LocalRotation = childPrefab.localRotation
                });
                BrowseTransform(child, childPrefab);
            }
        }

        private void ConfirmPose()
        {
            Undo.RecordObject(lipSync, "Set default pose");
            lipSync.DefaultPose = retrievedPose;
            lipSyncInspector.showDefaultPose = true;
            Close();
        }
    }
}