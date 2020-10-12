using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime.Actors
{
    [Serializable]
    [CreateAssetMenu(fileName = "Identifier", menuName = "Wired Dialog Engine/Actor Identifier", order = 5)]
    public class ActorIdentifier : ScriptableObject
    {
        [Tooltip("The name of this actor identifier. All dialog actors having this identifier will have this name.")]
        [SerializeField]
        private string actorName;
        /// <summary>
        /// The name of this actor identifier.
        /// </summary>
        public string ActorName
        {
            get
            {
                return actorName;
            }

            set
            {
                actorName = value;
            }
        }

        [JsonIgnore]
        private GameObject runtimeAttachedObject;
        /// <summary>
        /// The <see cref="GameObject"/> that is using this <see cref="ActorIdentifier"/>. Defined at runtime only.
        /// </summary>
        [JsonIgnore]
        public GameObject RuntimeAttachedObject
        {
            get
            {
                return runtimeAttachedObject;
            }

            set
            {
                runtimeAttachedObject = value;
            }
        }

        /// <summary>
        /// Shorthand for calling the GetComponent method on the RuntimeAttachedObject of this <see cref="ActorIdentifier"/>. If
        /// this method is called while not in runtime, then the RuntimeAttachedObject will be null and an exception will
        /// be thrown.
        /// </summary>
        /// <typeparam name="T">The generic type of the component to get.</typeparam>
        /// <returns>Returns the specified component.</returns>
        public T GetComponent<T>()
        {
            return RuntimeAttachedObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns that the animator can be used by the dialog, according to the "Use animations" flag of the dialog actor associated to this dialog idententifier.
        /// If it can, then the output variable "animator" is the animator. Otherwise it is null.
        /// </summary>
        /// <returns>Returns that the animator can be used by the dialog.</returns>
        public bool GetActorAnimator(out Animator animator)
        {
            if (runtimeAttachedObject.GetComponent<DialogActor>().UseAnimations)
            {
                animator = GetComponent<Animator>();
                return true;
            }
            animator = null;
            return false;
        }
    }
}