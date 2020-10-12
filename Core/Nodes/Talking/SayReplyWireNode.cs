using Assets.WiredTools.WiredDialogEngine.Runtime.Actors;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking
{
    [Serializable]
    public class SayReplyWireNode : ExecutionWireNode, ILoadable, IDelayable
    {
        /// <summary>
        /// The reply to say.
        /// </summary>
        [JsonIgnore]
        public Replies.Reply Reply;
        public string ReplyResourcePath { get; set; }

        /// <summary>
        /// The delay to wait after the node has been interpreted, in seconds.
        /// </summary>
        public float Delay;

        /// <summary>
        /// The dialog actor who will say the reply.
        /// </summary>
        [JsonIgnore]
        public ActorIdentifier Target;
        public string TargetResourcePath { get; set; }


        public SayReplyWireNode()
        {
            // Used by Newtonsoft JSON.NET
        }

        public SayReplyWireNode(WireDialog associatedDialog) : base(associatedDialog)
        {

        }

        public void Load()
        {
            Reply = Resources.Load<Replies.Reply>(ReplyResourcePath);
            Target = Resources.Load<ActorIdentifier>(TargetResourcePath);
        }

        public void Unload()
        {
            Resources.UnloadAsset(Reply);
            Resources.UnloadAsset(Target);
        }

        public float GetDelay()
        {
            return Delay;
        }

        // NOTE: the behaviour of the reply-playing is not defined here (as you can see...) but directly in the dialog interpreter. Have a look at it
        // to see how replies have been implemented.
    }
}