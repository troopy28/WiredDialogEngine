using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor.DialogEdition
{
    public class Background
    {
        public Rect GridZone { get; set; }

        public Background()
        {
            GridZone = new Rect(-5000, -5000, 5000, 5000);
        }

        /// <summary>
        /// Draws the background of the editor (the grid). The grid isn't unlimited.
        /// </summary>
        /// <param name="windowPosition"></param>
        /// <param name="backgroundTexture"></param>
        /// <param name="panOffset"></param>
        public void Draw(Rect windowPosition, ref Texture2D backgroundTexture, Vector2 panOffset)
        {
            if (Event.current.type == EventType.Repaint)
            {
                int x = (int)GridZone.size.x / backgroundTexture.width + 20;
                int y = (int)GridZone.size.y / backgroundTexture.height + 20;
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        GUI.DrawTexture( // Draw a single square of the grid
                            new Rect(
                                (int)(i * backgroundTexture.width - 500 + GridZone.x + panOffset.x),
                                (int)(j * backgroundTexture.height - 500 + GridZone.y + panOffset.y),
                                backgroundTexture.width,
                                backgroundTexture.height),
                            backgroundTexture);
                    }
                }
            }
        }
    }
}