using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Replies;
using Assets.WiredTools.WiredDialogEngine.DataPersistence;
using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;
using Assets.WiredTools.WiredDialogEngine.Runtime.Interpretation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime
{
    [AddComponentMenu("Wired Dialog Engine/Dialog player")]
    public class DialogPlayer : MonoBehaviour
    {
        [Tooltip("The dialog this player should (and will!) play.")]
        [SerializeField]
        private WireDialog dialog;
        /// <summary>
        /// The dialog this player should (and will) play.
        /// </summary>
        public WireDialog Dialog
        {
            get
            {
                return dialog;
            }
            set
            {
                dialog = value;
            }
        }

        [SerializeField]
        private string forcedLanguage;
        /// <summary>
        /// The language to use, no matter the preferences. Only used if the force language option is used in the *************** TO COMPLETE ***************
        /// </summary>
        public string ForcedLanguage
        {
            get
            {
                return forcedLanguage;
            }
            set
            {
                forcedLanguage = value;
            }
        }

        [Tooltip("The list of the actors needed by the dialog to work.")]
        [SerializeField]
        private List<DialogActor> actors;
        /// <summary>
        /// The list of the actors needed by the dialog to work.
        /// </summary>
        public List<DialogActor> Actors
        {
            get
            {
                return actors;
            }
            set
            {
                actors = value;
            }
        }

        [SerializeField]
        private bool playOnStart = true;
        public bool PlayOnStart
        {
            get { return playOnStart; }
            set { playOnStart = value; }
        }

        [Tooltip("Save the variables at the end of the interpretation of this dialog.")]
        [SerializeField]
        private bool saveVariables = true;
        public bool SaveVariables
        {
            get
            {
                return saveVariables;
            }

            set
            {
                saveVariables = value;
            }
        }


        private DialogInterpreter dialogInterpreter;
        public string CurrentReplySubtitles { get; set; }

        // (Identifier, actor)
        private Dictionary<ActorIdentifier, DialogActor> actorsDic;

        /// <summary>
        /// The index of the reply chosen by the player when he has been proposed a choice and he answered. If equals to "-1", then
        /// there is no choice currently. If equals to "-2", then there is a choice but no answer.
        /// </summary>
        private int choiceMade = NoChoiceCurrently;
        private const int NoChoiceCurrently = -1;
        private const int ChoiceWaitingAnswer = -2;

        /// <summary>
        /// Is this dialog player currently playing a dialog.
        /// </summary>
        public bool Playing { get; private set; }

        /// <summary>
        /// Initializes the dialog player. Starts playing the dialog if the <see cref="PlayOnStart"/> flag is set to true.
        /// </summary>
        protected virtual void Start()
        {
            dialogInterpreter = new DialogInterpreter();
            actorsDic = new Dictionary<ActorIdentifier, DialogActor>();
            if (dialog != null)
                dialogInterpreter.SetWireDialog(dialog);

            foreach (DialogActor actor in actors)
                if (actor != null)
                    actorsDic.Add(actor.Identifier, actor);
                else
                    Debug.LogError("The dialog player " + gameObject.name + " contains a null actor.");

            if (playOnStart)
                StartPlayingDialog(saveVariables);
        }

        /// <summary>
        /// Start playing the dialog. If the dialog is already playing, does nothing. Throws an exception if the dialog is null.
        /// Sets the <see cref="Playing"/> flag to true.
        /// </summary>
        /// <param name="saveModifiedVariables">Should the dialog player save the variables at the end of the dialog?</param>
        public virtual void StartPlayingDialog(bool saveModifiedVariables)
        {
            if (Playing)
                return;

            if (Dialog == null)
            {
                Debug.LogError("The dialog is missing. Please select a dialog before trying to play it.");
                return;
            }

            if (DialogEngineRuntime.Instance.DialogDisplayer == null)
            {
                Debug.LogError("The dialog displayer is not set in the dialog engine runtime. Can't play the dialog.");
                return;
            }

            DialogEngineRuntime.Instance.DialogDisplayer.CurrentPlayer = this;

            StartCoroutine(PlayDialog(saveModifiedVariables));
            Playing = true;
        }

        /// <summary>
        /// Stops playing the dialog. If the dialog isn't playing, does nothing. Sets the <see cref="Playing"/> flag to false.
        /// After all that, if the saveModifiedVariable argument is set to true, saves the variables.
        /// </summary>
        /// <param name="saveModifiedVariables">Should the dialog player save the variable right now?</param>
        public virtual void StopPlayingDialog(bool saveModifiedVariables)
        {
            if (!Playing)
                return;

            StopCoroutine(PlayDialog(saveModifiedVariables));
            Playing = false;

            if (saveModifiedVariables)
                DialogFilesManager.SaveVariables(DialogEngineRuntime.Instance.VariablesContainer);
        }

        /// <summary>
        /// The enumerator associated to the coroutine responsible of the dialog playing process. This coroutine
        /// handles the delay, the choices, and the end of dialogs. You can also override it. To do so, please
        /// refer to the documentation.
        /// </summary>
        /// <param name="saveModifiedVariables">Should the coroutine save the variables after interpreting the last node of the dialog?</param>
        /// <returns></returns>
        public virtual IEnumerator PlayDialog(bool saveModifiedVariables)
        {
#if (ENABLE_DEBUG_DIALOG)
            Debug.Log("Start of the dialog.");
#endif
            float nextDelay;

            yield return new WaitForSeconds(0.1f);
            while (!dialogInterpreter.EndOfDialog) // While we aren't at the end of the dialog
            {
                if (choiceMade == ChoiceWaitingAnswer)
                {
                    yield return new WaitUntil(() => choiceMade != ChoiceWaitingAnswer);
                    dialogInterpreter.ChosenReply = choiceMade;
                }
                choiceMade = NoChoiceCurrently;

                if (ProcessNextNode(out nextDelay))
                {
                    yield return new WaitForSeconds(nextDelay);
                }
                else
                    break;
            }

            if (saveModifiedVariables)
                DialogFilesManager.SaveVariables(DialogEngineRuntime.Instance.VariablesContainer);
            CurrentReplySubtitles = "";
#if (ENABLE_DEBUG_DIALOG)
            Debug.Log("End of the dialog.");
#endif
        }

        /// <summary>
        /// Processes the next node of the dialog.
        /// Returns that the node can be interpreted (that is to say it is neither undefined nor an end of dialog).
        /// </summary>
        /// <param name="nextDelay">The delay to wait before processing the next node, according to the dialog graph.</param>
        /// <returns>Returns that the node can be interpreted (that is to say it is neither undefined nor an end of dialog).</returns>
        public virtual bool ProcessNextNode(out float nextDelay)
        {
            InterpretationResult result = dialogInterpreter.InterpretNextNode();
            float replyLength = 0;
            switch (result.ResultType)
            {
                case InterpretationResultType.UNDEFINED:
                case InterpretationResultType.FLOW_NODE:
                    break;
                case InterpretationResultType.SAY_REPLICA:
                    replyLength = SayReply(result.Reply, result.Target);
                    break;
                case InterpretationResultType.CHOICE:
                    ProcessChoice(result.Choice);
                    break;
            }
            nextDelay = result.Delay + replyLength; // The delay specified in the node + the time needed to say the reply entirely, if the node is a reply node
            return result.ResultType != InterpretationResultType.END_OF_DIALOG && result.ResultType != InterpretationResultType.UNDEFINED;
        }

        /// <summary>
        /// Places this <see cref="DialogPlayer"/> into a state such as it awaits for a choice from the user. It sets the choiceMade
        /// variable to the ChoiceWaitingAnswer constant. It then notify the current dialog overlay that it should display the choice
        /// specified in argument.
        /// </summary>
        /// <param name="choice">The choice the player has to do.</param>
        protected virtual void ProcessChoice(Choice choice)
        {
            choiceMade = ChoiceWaitingAnswer; // We now want an answer, and wait for it
            choice.DialogPlayer = this;
            DialogEngineRuntime.Instance.DialogDisplayer.HandleMakeChoice(choice);
        }

        /// <summary>
        /// Used by the dialog overlays. It informs the dialog player that the player chose the option located at the
        /// specified index. If the dialog player wasn't waiting for an answer, does nothing.
        /// </summary>
        /// <param name="choiceIndex"></param>
        public virtual void ProvideChoiceAnswer(int choiceIndex)
        {
            if (choiceMade != ChoiceWaitingAnswer) // Not waiting for a choice, so don't process this call
                return;
            choiceMade = choiceIndex;
        }

        /// <summary>
        /// Makes the specified target say the specified reply, and returns the length of the audio clip corresponding to the current language.
        /// </summary>
        /// <param name="reply">The reply to say.</param>
        /// <param name="target">The actor that will say the reply.</param>
        /// <returns>Returns the length of the audio clip corresponding to the current language.</returns>
        public virtual float SayReply(Reply reply, ActorIdentifier target)
        {
            if (reply == null)
                throw new System.Exception("ERROR: the reply cannot be null!");
            else if (target == null)
                throw new System.Exception("ERROR: the target cannot be null for the reply " + reply.name);
            else if (!actorsDic.ContainsKey(target))
                throw new System.Exception("ERROR: unable to find the target specified in your dialog for the reply " + reply.name + " in the list" +
                    "of the actors for your dialog. Please make sure all the actors are specified in your DialogPlayer.");

            if (forcedLanguage.Length > 0)
                return actorsDic[target].SayReply(reply, this, forcedLanguage);
            return actorsDic[target].SayReply(reply, this);
        }
    }
}