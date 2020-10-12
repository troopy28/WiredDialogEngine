using Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interaction
{
    public abstract class AbstractDialogOverlay : MonoBehaviour
    {
        public DialogPlayer CurrentPlayer { get; set; }

        [SerializeField]
        private Text subtitlesDisplayer;
        public Text SubtitlesDisplayer
        {
            get
            {
                return subtitlesDisplayer;
            }

            set
            {
                subtitlesDisplayer = value;
            }
        }

        public abstract void HandleMakeChoice(Choice choice);

        public virtual void SendAnswerToDialog(int answer)
        {
            CurrentPlayer.ProvideChoiceAnswer(answer);
        }

        protected virtual void Update()
        {
            if (CurrentPlayer == null || subtitlesDisplayer == null || CurrentPlayer.CurrentReplySubtitles == null)
                return;
            subtitlesDisplayer.text = CurrentPlayer.CurrentReplySubtitles;
        }
    }
}