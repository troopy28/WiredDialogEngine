using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Replies
{
    [Serializable]
    public class ReplyPart
    {
        [SerializeField]
        private AudioClip audio;
        public AudioClip Audio
        {
            get
            {
                return audio;
            }
        }

        [SerializeField]
        private string subtitle;
        public string Subtitles
        {
            get
            {
                return subtitle;
            }
        }

        [Tooltip("The delay before saying this part of reply, in seconds.")]
        [SerializeField]
        private float delayBeforePlaying;
        public float DelayBeforePlaying
        {
            get
            {
                return delayBeforePlaying;
            }

            set
            {
                delayBeforePlaying = value;
            }
        }

    }
}