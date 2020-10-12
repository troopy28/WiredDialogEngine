using Assets.WiredTools.WiredDialogEngine.LipSync;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    public class BBFacePoseDisplayer
    {
        private BBFacePose createdPose;
        private BBLipSync ownerLipSync;
        private BBLipSyncInspector inspector;

        public void Clear()
        {
            createdPose = null;
        }

        public BBFacePose Draw(BBFacePose targetPose, BBLipSync lipSync, BBLipSyncInspector inspector)
        {
            if (targetPose == null)
            {
                targetPose = new BBFacePose()
                {
                    BonePoses = new List<BBBonePose>()
                };
            }
            if (targetPose != createdPose)
            {
                targetPose.BonePoses.ForEach(bonePose =>
                {
                    if (bonePose != null && bonePose.BoneTransform != null)
                    {
                        bonePose.BoneTransform.localPosition = bonePose.LocalPosition;
                        bonePose.BoneTransform.localRotation = bonePose.LocalRotation;
                    }
                });
            }
            createdPose = targetPose;
            this.inspector = inspector;
            ownerLipSync = lipSync;
            DrawPosingInterface();

            DisplayPoseOnModel();

            return targetPose;
        }

        private void DrawPosingInterface()
        {
            // Display help
            EditorStyles.label.wordWrap = true;

            // If there is no blendshape, then only display a button to add one
            if (createdPose.BonePoses.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button(new GUIContent("Add a bone")))
                {
                    Undo.RecordObject(ownerLipSync, "Adding a bone.");
                    createdPose.BonePoses.Add(new BBBonePose());
                }
                EditorGUILayout.EndHorizontal();
            }
            // Otherwise, display all the blendshapes
            else
            {
                // Draw the list of the blendshapes to use
                EditorGUILayout.LabelField("Bones");
                EditorGUI.indentLevel++;
                DrawBonesList();
            }
        }

        private void DrawBonesList()
        {
            for (int i = 0; i < createdPose.BonePoses.Count; i++)
            {
                BBBonePose bonePose = createdPose.BonePoses[i];

                if (inspector.AutoPose && bonePose != null && bonePose.BoneTransform != null)
                {
                    bonePose.LocalPosition = bonePose.BoneTransform.localPosition;
                    bonePose.LocalRotation = bonePose.BoneTransform.localRotation;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent("-", "Remove this bone."), GUILayout.Width(20f)))                                 // The remove button
                {
                    RemoveBone(bonePose, bonePose.BoneTransform);

                    EditorGUILayout.EndHorizontal();
                    break;
                }
                else if (bonePose != null && bonePose.BoneTransform != null && GUILayout.Button("Copy", GUILayout.Width(50)))
                {
                    CopyPaster.CopiedPose = bonePose.Copy();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                bool transformWasNull = false;
                transformWasNull = bonePose.BoneTransform == null;
                bonePose.BoneTransform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Transform", "The bone (Transform) to update to show the pose."), bonePose.BoneTransform, typeof(Transform), true);                  // The transform    
                if (transformWasNull && bonePose.BoneTransform != null) // Bone was null before the field, and is now filled
                {
                    bonePose.LocalPosition = bonePose.BoneTransform.localPosition;
                    bonePose.LocalRotation = bonePose.BoneTransform.localRotation;
                    Selection.objects = new GameObject[] { bonePose.BoneTransform.gameObject };
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();                                                                         // The name
                bonePose.LocalPosition = EditorGUILayout.Vector3Field(new GUIContent("Position", "The LOCAL position of your bone(relative to its parent)."), bonePose.LocalPosition); // The position
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                // Convert, show, convert back, store the rotation
                Vector3 rotation = bonePose.LocalRotation.eulerAngles;
                rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation", "The LOCAL rotation of your bone(relative to its parent)."), rotation); // The rotation
                bonePose.LocalRotation = Quaternion.Euler(rotation);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (CopyPaster.CopiedPose != null)
            {
                if (GUILayout.Button("Paste"))
                {
                    Undo.RecordObject(ownerLipSync, "Pasting a bone");
                    createdPose.BonePoses.Add(CopyPaster.CopiedPose);
                }
            }
            if (GUILayout.Button(new GUIContent("+", "Add a bone."), GUILayout.Width(20f)))
            {
                Undo.RecordObject(ownerLipSync, "Adding a bone.");
                createdPose.BonePoses.Add(new BBBonePose());
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RemoveBone(BBBonePose bone, Transform transform)
        {
            Undo.RecordObject(ownerLipSync, "Removing bone.");
            bool found = false;
            bool foundNull = false; // True if the corresponding found bone is pointing to a null transform
            foreach (BBBonePose other in createdPose.BonePoses)
            {
                // If we find another BBBonePose with the same target
                if (other != bone && other.BoneTransform == transform) 
                {
                    foundNull = other.BoneTransform == null;
                    if(!foundNull) // Check for null: if two bones point to a null transform, it must not be used
                    {
                        bone.BoneTransform.localPosition = other.LocalPosition;
                        bone.BoneTransform.localRotation = other.LocalRotation;
                        found = true;
                    }         
                }
            }
            if (!found)
            {
                IEnumerable<BBBonePose> col = ownerLipSync.DefaultPose.BonePoses.Where(bp => bp.BoneTransform == transform);
                if (col.Count() > 0)
                {
                    BBBonePose defaultBonePose = col.First();
                    if (defaultBonePose == null)
                        UnityEngine.Debug.LogError("An error occurred in BBFacePoseDsplayer");
                    if (bone != null && bone.BoneTransform != null)
                    {
                        bone.BoneTransform.localPosition = defaultBonePose.LocalPosition;
                        bone.BoneTransform.localRotation = defaultBonePose.LocalRotation;
                    }
                }
                else // When we arrive here, it means we didn't find any bone from the default pose that matches the transform of the bone that has just been removed
                {
                    // Not removing from the default face pose editor AND we didn't found a null bone AND the default face pose has NO bone AT ALL (and not just no bone matching the one deleted)
                    if (!BBGetDefaultPoseWindow.CallingRemoving && !foundNull && ownerLipSync.DefaultPose.BonePoses.Count <= 0) 
                        UnityEngine.Debug.LogError("Danger: the default face pose is not defined for " + ownerLipSync.gameObject.name + " " + ownerLipSync.name + ". Strange behaviours may occur.");
                }
            }
            createdPose.BonePoses.Remove(bone);
        }

        private void DisplayPoseOnModel()
        {
            if (Application.isPlaying)
                return;

            List<BBBonePose> nullPoses = new List<BBBonePose>();
            for (int i = 0; i < createdPose.BonePoses.Count; i++)
            {
                BBBonePose bone = createdPose.BonePoses[i];
                if (bone == null)
                {
                    nullPoses.Add(bone);
                    continue;
                }
                if (bone.BoneTransform == null)
                    continue;
                bone.BoneTransform.localPosition = bone.LocalPosition;
                bone.BoneTransform.localRotation = bone.LocalRotation;
            }
            foreach (BBBonePose nullIndex in nullPoses)
                createdPose.BonePoses.Remove(nullIndex);
        }
    }
}