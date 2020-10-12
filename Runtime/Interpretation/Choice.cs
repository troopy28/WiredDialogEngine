using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Replies;
using System.Collections.Generic;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation
{
    public class Choice
    {
        public List<Reply> Choices { get; set; }
        public DialogPlayer DialogPlayer { get; set; }

        public Choice()
        {
            Choices = new List<Reply>();
        }
    }
}
