using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Inspectors
{
    public class GraphDrawer
    {
        public float[] Values { get; private set; }
        public int Size { get; private set; }

        private const float AudioWindowSize = 200f;
        private Vector2 lastFieldRect;
        private float usableWidth;
        private Color audioGraphLineColor;

        public GraphDrawer(int size)
        {
            Size = size;
            Values = new float[size];

            Color backgroundColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
            audioGraphLineColor = backgroundColor * 0.7f;
            audioGraphLineColor.a = 0.2f;
        }

        /// <summary>
        /// Adds a value to the graph drawer, and shifts its value to the left (to display it on the graph).
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void AddValue(float value)
        {
            // Shift the values to the left
            Array.Copy(Values, 1, Values, 0, Values.Length - 1);
            // Add the new value at the end
            Values[Size - 1] = value;
        }

        public void DrawGraph(float stretching = 30)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("An active scene window is needed so that the audio data can be correctly updated.");
            EditorGUILayout.EndHorizontal();

            lastFieldRect = GUILayoutUtility.GetLastRect().position;
            lastFieldRect.y += 200;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(AudioWindowSize));

            float max = 0;
            float min = float.PositiveInfinity;

            // Get the max - min
            for (int i = 0; i < Size; i++)
            {
                float curr = Values[i];
                if (curr < min)
                    min = curr;
                if (curr > max)
                    max = curr;
            }

            float extent = max - min;
            // Hard way to get the Unity inspector width
            float contextWidth = (float)typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
            usableWidth = contextWidth - 33;
            float rectWidth = usableWidth / Size;

            // Draw the audio rects
            for (int i = 0; i < Size; i++)
            {
                float currData = Values[i];
                float t = currData / extent;

                Color rectColor = Color.Lerp(Color.cyan, Color.red, t);

                float posX = lastFieldRect.x + i * rectWidth + 1;
                float posY = lastFieldRect.y;

                float sizeX = rectWidth - 1;
                float sizeY = -(currData * stretching + 1);

                Rect rect = new Rect(posX, posY, sizeX, sizeY);
                EditorGUI.DrawRect(rect, rectColor);
                EditorGUI.LabelField(rect, new GUIContent(TruncateDecimal(currData, 2).ToString()));
            }

            // Draw the horizontal lines
            for (int i = 0; i < AudioWindowSize - 15; i += 15)
            {
                Rect rect = new Rect();
                rect.Set(
                    x: lastFieldRect.x,
                    y: lastFieldRect.y - i,
                    width: usableWidth,
                    height: 1
                    );
                EditorGUI.DrawRect(rect, audioGraphLineColor);
            }

            GUILayout.Space(180);
            EditorGUILayout.EndVertical();
        }

        private float TruncateDecimal(float value, int precision)
        {
            float step = (float)Math.Pow(10, precision);
            float tmp = (float)Math.Truncate(step * value);
            return tmp / step;
        }

        /*
        public static string ToString(float[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            for(int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i]).Append("; ");
            }
            sb.Append(" }");
            return sb.ToString();
        }*/
    }
}