using Assets.WiredTools.WiredDialogEngine.Core.Replies;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Actors
{
    [AddComponentMenu("Wired Dialog Engine/Dialog actor")]
    [RequireComponent(typeof(AudioSource))]
    public class DialogActor : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField]
        private ActorIdentifier identifier;
        public ActorIdentifier Identifier
        {
            get
            {
                return identifier;
            }
            set
            {
                identifier = value;
            }
        }

        [SerializeField]
        private bool useAnimations;
        public bool UseAnimations
        {
            get
            {
                return useAnimations;
            }
            set
            {
                useAnimations = value;
            }
        }

        [SerializeField]
        private Animator animator;
        public Animator Animator
        {
            get
            {
                return animator;
            }
        }

        /// <summary>
        /// Is this actor currently saying a reply of a dialog?
        /// </summary>
        public bool PlayingDialog { get; private set; }

        private Coroutine playReplyCoroutine;

        private void Awake()
        {
            identifier.RuntimeAttachedObject = gameObject;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (useAnimations)
            {
                animator = GetComponent<Animator>();
                useAnimations = animator != null;
            }
        }

        /// <summary>
        /// Starts saying the reply by this <see cref="DialogActor"/>, 
        /// and returns the total length of the audio clips corresponding to the reply in the specified language if it isn't text only.
        /// If it is, returns 0.
        /// </summary>
        /// <param name="reply">The <see cref="Reply"/> to say.</param>
        /// <returns>Returns the length of the audio clip corresponding to the reply in the current language. Returns 0 if it is text only.</returns>
        public float SayReply(Reply reply, DialogPlayer associatedPlayer)
        {
            LocalizedReply currentLanguageReply = reply.GetReplyForLanguage(DialogEngineRuntime.Instance.DialogLanguage);

            if (currentLanguageReply == null)
                throw new Exception("The language you are trying to use is not defined in the reply you are trying to say. Reply name: " + reply.name);

            if (playReplyCoroutine != null)
                StopCoroutine(playReplyCoroutine);

            playReplyCoroutine = StartCoroutine(PlayReplyCoroutine(currentLanguageReply, associatedPlayer, reply.TextOnly));

            if (reply.TextOnly)
                return GetMutedReplyPlayingTime(currentLanguageReply);
            return currentLanguageReply.CompleteLength;
        }

        private float GetMutedReplyPlayingTime(LocalizedReply lr)
        {
            float time = 0;
            for(int i = 0; i < lr.SubParts.Count; i++)
            {
                time += lr.SubParts[i].DelayBeforePlaying;
                time += 0.1f;
            }
            return time;
        }

        public float SayReply(Reply reply, DialogPlayer associatedPlayer, string forcedLanguage)
        {
            LocalizedReply currentLanguageReply = reply.GetReplyForLanguage(forcedLanguage);

            if (currentLanguageReply == null)
                throw new Exception("The language you are trying to use is not defined in the reply you are trying to say.");

            if (playReplyCoroutine != null)
                StopCoroutine(playReplyCoroutine);

            playReplyCoroutine = StartCoroutine(PlayReplyCoroutine(currentLanguageReply, associatedPlayer, reply.TextOnly));

            if (reply.TextOnly)
                return 0;
            return currentLanguageReply.CompleteLength;
        }

        /// <summary>
        /// The coroutine responsible of playing the reply: it plays all the reply parts, waits the user-specified time
        /// between each part etc.
        /// </summary>
        /// <param name="localizedReply">The reply in the current language.</param>
        /// <param name="associatedPlayer">The player of the reply.</param>
        /// <param name="textOnly">Is this reply text only?</param>
        private IEnumerator PlayReplyCoroutine(LocalizedReply localizedReply, DialogPlayer associatedPlayer, bool textOnly)
        {
            PlayingDialog = true;

            for (int i = 0; i < localizedReply.SubParts.Count; i++)
            {
                // Get the current reply
                ReplyPart currentPart = localizedReply.SubParts[i];

                // Set the current audio file
                if (!textOnly)
                    audioSource.clip = currentPart.Audio;

                // Wait the specified amount of time before saying the reply and then play it
                yield return new WaitForSeconds(currentPart.DelayBeforePlaying);

                // Show the current subtitles
                if (associatedPlayer != null)
                    associatedPlayer.CurrentReplySubtitles = currentPart.Subtitles; // Set the subtitles AFTER waiting for muted dialogs

                if (!textOnly)
                {
                    audioSource.PlayOneShot(currentPart.Audio);
                    // Then wait for the end of this reply part
                    yield return new WaitForSeconds(currentPart.Audio.length);
                    PlayingDialog = false;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                    PlayingDialog = false;
                }
            }
        }
    }
}