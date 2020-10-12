using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Replies
{
    [Serializable]
    [CreateAssetMenu(fileName = "Reply", menuName = "Wired Dialog Engine/Reply", order = 1)]
    public class Reply : ScriptableObject
    {
        [SerializeField]
        private List<LocalizedReply> localizedReplies;
        /// <summary>
        /// The list of the replies in the different languages.
        /// </summary>
        public List<LocalizedReply> LocalizedReplies
        {
            get
            {
                return localizedReplies;
            }

            set
            {
                localizedReplies = value;
            }
        }

        [Tooltip("Check it if you don't want to have an audio clip.")]
        [SerializeField]
        private bool textOnly;
        /// <summary>
        /// If true, this reply doesn't have any audio file.
        /// </summary>
        public bool TextOnly
        {
            get
            {
                return textOnly;
            }
            set
            {
                textOnly = value;
            }
        }

        /// <summary>
        /// Returns the count of the replies. It is also the number of the <see cref="LocalizedReply"/> this reply have.
        /// </summary>
        public int LanguagesCount
        {
            get
            {
                return LocalizedReplies.Count;
            }
        }

        /// <summary>
        /// Returns the translation of this reply in the specified language.
        /// </summary>
        /// <param name="language">The language associated to the localized reply to play. Case sensitive.</param>
        /// <returns>Returns the translation of this reply in the specified language.</returns>
        public LocalizedReply GetReplyForLanguage(string language)
        {
            List<LocalizedReply> lst = LocalizedReplies.Where(lang => lang.Language == language).ToList();
            if (lst == null)
                return null;
            if (lst.Count <= 0)
                return null;
            return lst[0];
        }

        /// <summary>
        /// Returns the number of reply-parts there are in the language that has the more reply-parts.
        /// </summary>
        /// <returns></returns>
        public int GetMaxParts()
        {
            int max = 0;
            foreach(LocalizedReply rp in LocalizedReplies)
            {
                int count = rp.SubParts.Count;
                if (count > max)
                    max = count;
            }
            return max;
        }
    }
}