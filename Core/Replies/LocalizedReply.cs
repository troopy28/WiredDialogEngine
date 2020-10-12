using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Replies
{
    [Serializable]
    public class LocalizedReply
    {
        [Tooltip("The language associated to this part.")]
        [SerializeField]
        private string language;
        public string Language
        {
            get
            {
                return language;
            }
        }

        [Tooltip("The text that will be shown to the player if he must choose between several replies.")]
        [SerializeField]
        private string choiceText;
        public string ChoiceText
        {
            get
            {
                return choiceText;
            }
        }

        [SerializeField]
        private List<ReplyPart> subParts;
        public List<ReplyPart> SubParts
        {
            get
            {
                return subParts;
            }
            set
            {
                subParts = value;
            }
        }

        /// <summary>
        /// Returns the total time needed to play all the replies' audio file.
        /// </summary>
        public float CompleteLength
        {
            get
            {
                float length = 0;
                foreach (ReplyPart rp in subParts)
                {
                    length += rp.Audio.length;
                    length += rp.DelayBeforePlaying;
                }
                return length;
            }
        }
    }
}