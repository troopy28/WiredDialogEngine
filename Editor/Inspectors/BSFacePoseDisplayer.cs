using Assets.WiredTools.WiredDialogEngine.LipSync;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    public class BSFacePoseDisplayer
    {
        private BSFacePose createdPose;
        private BSLipSync ownerLipSync;

        public BSFacePose Draw(BSFacePose targetPose, BSLipSync lipSync)
        {
            if (targetPose == null)
            {
                targetPose = new BSFacePose()
                {
                    Blendshapes = new List<BSBlendshape>()
                };
            }
            createdPose = targetPose;
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
            if (createdPose.Blendshapes.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button(new GUIContent("Add a blendshape")))
                {
                    Undo.RecordObject(ownerLipSync, "Adding a blendshape.");
                    createdPose.Blendshapes.Add(new BSBlendshape()
                    {
                        Index = createdPose.Blendshapes.Count + 1
                    });
                }
                EditorGUILayout.EndHorizontal();
            }
            // Otherwise, display all the blendshapes
            else
            {
                // Draw the list of the blendshapes to use
                EditorGUILayout.LabelField("Blendshapes");
                EditorGUI.indentLevel++;
                DrawBlendshapesList();
            }
        }

        private void DrawBlendshapesList()
        {
            //BSBlendshape blendshape in createdPose.Blendshapes
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Index", "The index of this blendshape in the skinned mesh renderer."));

            EditorGUILayout.LabelField(new GUIContent("Name", "The name of this blendshape in the skinned mesh renderer."));

            GUILayout.FlexibleSpace();
            // Spaces used to align the header with the table
            EditorGUILayout.LabelField(new GUIContent("Value         ", "The value of this blendshape in this pose. Between 0 and 100."));
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < createdPose.Blendshapes.Count; i++)
            {
                BSBlendshape blendshape = createdPose.Blendshapes[i];
                EditorGUILayout.BeginHorizontal();

                blendshape.Index = Mathf.Clamp(EditorGUILayout.IntField(blendshape.Index, GUILayout.Width(100)), 0, 1000);                  // The index
                ShowBlendshapeNamePicker(blendshape);                                                                                       // The name
                blendshape.Value = EditorGUILayout.Slider(blendshape.Value, 0f, 100f);                                                      // The value
                if (GUILayout.Button(new GUIContent("-", "Remove this blendshape."), GUILayout.Width(20f)))                                 // The remove button
                {
                    RemoveBlendshape(blendshape, i);

                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("+", "Add a blendshape."), GUILayout.Width(20f)))
            {
                Undo.RecordObject(ownerLipSync, "Adding a blendshape.");
                createdPose.Blendshapes.Add(new BSBlendshape()
                {
                    Index = createdPose.Blendshapes.Count + 1
                });
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RemoveBlendshape(BSBlendshape blendshape, int index)
        {
            Undo.RecordObject(ownerLipSync, "Removing blendshape.");
            bool found = false;
            foreach (BSBlendshape other in createdPose.Blendshapes)
            {
                // If we find another BSBlendshape with the same index (targeting the same blendshape), then we apply it
                if (other != blendshape && other.Index == index)
                {
                    ownerLipSync.MeshRenderer.SetBlendShapeWeight(index, other.Value);
                    found = true;
                }
            } // Otherwise, apply a zero value to the blendshape
            if (!found)
            {
                if (ownerLipSync.MeshRenderer != null)
                    ownerLipSync.MeshRenderer.SetBlendShapeWeight(index, 0);
            }

            createdPose.Blendshapes.RemoveAt(index);
        }

        private void DisplayPoseOnModel()
        {
            if (ownerLipSync.MeshRenderer == null || Application.isPlaying) // Don't enable live pose display in play mode...
                return;

            int modelBlendshapesCount = ownerLipSync.MeshRenderer.sharedMesh.blendShapeCount;
            for (int i = 0; i < modelBlendshapesCount; i++)
            {
                IEnumerable<BSBlendshape> correspondingPoseBlendshapes = createdPose.Blendshapes.Where(pose => pose.Index == i);
                int actualCount = correspondingPoseBlendshapes.Count();
                // If there is at least one pose
                if (actualCount != 0)
                {
                    // If there are several poses, it's a problem: signal it
                    if (actualCount > 1 && EditorApplication.isPlaying)
                    {
                        Debug.LogError("DANGER: several blendshapes are defined in your pose with the same index. The first in the list will be used.");
                    }
                    BSBlendshape firstBlendshape = correspondingPoseBlendshapes.First();
                    ownerLipSync.MeshRenderer.SetBlendShapeWeight(i, firstBlendshape.Value);
                }
                // Otherwise there is no pose
                else
                {
                    ownerLipSync.MeshRenderer.SetBlendShapeWeight(i, 0);
                }
            }
        }

        private void ShowBlendshapeNamePicker(BSBlendshape blendshape)
        {
            int modelBlendshapesCount = ownerLipSync.MeshRenderer.sharedMesh.blendShapeCount;
            string[] modelBlendshapesNames = new string[modelBlendshapesCount];
            for (int i = 0; i < modelBlendshapesCount; i++)
            {
                modelBlendshapesNames[i] = GetFormattedName(ownerLipSync.MeshRenderer.sharedMesh.GetBlendShapeName(i));
            }
            blendshape.Index = EditorGUILayout.Popup(blendshape.Index, modelBlendshapesNames);
            //EditorGUILayout.LabelField(GetFormattedName(ownerLipSync.MeshRenderer.sharedMesh.GetBlendShapeName(blendshape.Index)), GUILayout.MinWidth(100));
        }

        private string GetFormattedName(string rawName)
        {
            return rawName.Replace("Facial_Blends.", "");
        }
    }
}