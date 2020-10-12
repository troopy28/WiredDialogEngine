using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Localization
{
    [Serializable]
    public class LocalizableString
    {
        [SerializeField]
        private string language;
        public string Language
        {
            get
            {
                return language;
            }
        }

        [SerializeField]
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
        }
    }
}
