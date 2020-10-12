using Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Interaction
{
    [AddComponentMenu("Wired Dialog Engine/Simple dialog overlay")]
    public class SimpleDialogOverlay : AbstractDialogOverlay
    {
        [SerializeField]
        private List<Button> choices;

        public void Start()
        {
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }

        public override void HandleMakeChoice(Choice choice)
        {
            CurrentPlayer = choice.DialogPlayer;

            try
            {
                for (int i = 0; i < choice.Choices.Count; i++)
                {
                    choices[i].gameObject.SetActive(true);
                    choices[i].GetComponentInChildren<Text>().text = choice.Choices[i].GetReplyForLanguage(DialogEngineRuntime.Instance.DialogLanguage).ChoiceText;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("An error occurred while processing the choice " + choice.ToString() + " on the dialog player " + choice.DialogPlayer.name + ". " +
                    "Make sure the replies and the choices are correctly setup in your dialog.");
#if (ENABLE_DEBUG_DIALOG)
                Debug.LogError("Details: " + ex.ToString());
#endif
            }
        }

        public void OnChooseOne()
        {
            CurrentPlayer.ProvideChoiceAnswer(0);
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }

        public void OnChooseTwo()
        {
            CurrentPlayer.ProvideChoiceAnswer(1);
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }

        public void OnChooseThree()
        {
            CurrentPlayer.ProvideChoiceAnswer(2);
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }

        public void OnChooseFour()
        {
            CurrentPlayer.ProvideChoiceAnswer(3);
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }

        public void OnChooseFive()
        {
            CurrentPlayer.ProvideChoiceAnswer(4);
            foreach (Button btn in choices)
                btn.gameObject.SetActive(false);
        }
    }
}