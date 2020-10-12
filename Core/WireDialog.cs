using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialog", menuName = "Wired Dialog Engine/Dialog", order = 4)]
    public class WireDialog : ScriptableObject
    {
        // Trailer music : Avenza - Never Sober. You didn't except this to be there right? ;-)

        [HideInInspector]
        [SerializeField]
        private uint currWirePinId = 1;

        [HideInInspector]
        [SerializeField]
        private uint currWireNodeId = 1;

        [Tooltip("The UNIQUE name of the dialog.")]
        [SerializeField]
        private string dialogName;
        /// <summary>
        /// The name of the dialog. It doesn't have to be unique. 
        /// </summary>
        public string DialogName
        {
            get
            {
                return dialogName;
            }
        }

        /// <summary>
        /// A map of the unique ID of the wire nodes and the nodes themselves. For serialization reason, the nodes cannot be
        /// saved with a reference to another node (it would lead to a cyclic serialization). A workaround for that is the
        /// nodes store a unique ID, which is then used to find the corresponding node using this map. This way, the connections
        /// can be saved.
        /// </summary>
        public Dictionary<uint, WireNode> NodesDictionary { get; set; }
        private List<WireNode> nodesList;

        /// <summary>
        /// A map of the unique ID of the wire pins and the pins themselves. For serialization reason, the pins cannot be
        /// saved with a reference to another pin (it would lead to a cyclic serialization). A workaround for that is the
        /// pins store a unique ID, which is then used to find the corresponding pin using this map. This way, the connections
        /// can be saved.
        /// </summary>
        public Dictionary<uint, WirePin> Pins { get; set; }

        /// <summary>
        /// The saved dialog. It is serialized by Unity.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string serializedNodes;

        [HideInInspector]
        [SerializeField]
        private bool initialized;
        /// <summary>
        /// Is this dialog initialized? A dialog is automatically initialized by the dialog editor. A non initialized dialog
        /// doesn't have a dialog beginning node, for instance, and this node cannot be created by the user. A non initialized
        /// dialog cannot be used by a dialog designer in the editor.
        /// </summary>
        public bool Initialized
        {
            get
            {
                return initialized;
            }
            private set
            {
                initialized = value;
            }
        }

        /// <summary>
        /// Initializes the maps and the node list. It also creates the serializedNode string, used to save the dialog.
        /// </summary>
        private void Awake()
        {
#if (ENABLE_DEBUG_DIALOG)
            Debug.Log("Awake called on a dialog.");
#endif
            if (initialized)
                return;
#if (ENABLE_DEBUG_DIALOG)
            Debug.Log("Dialog was not initialized. Initializing it.");
#endif
            NodesDictionary = new Dictionary<uint, WireNode>();
            nodesList = new List<WireNode>();
            Pins = new Dictionary<uint, WirePin>();
            if (serializedNodes == null)
                serializedNodes = "";
        }

        /// <summary>
        /// <para>
        /// Call this to CREATE the dialog, the first time you want to use it. This is automatically managed by the 
        /// dialog editor, so use this method only if you are sure of what you are doing. It will erase any content
        /// that could have been set in the dialog, and the add the specified beginning. The boolean 
        /// <see cref="Initialized"/> will be set to "true" so that it will be considered as initialized (which is
        /// the case!) After that, it will save the dialog so that it will be loadable using the <see cref="Load"/>
        /// function. 
        /// </para>
        /// <para>
        /// The beginning node will take an ID of 1 after being passed to this function. This is because as it will
        /// become the new first node of the dialog, it must take an ID of one. The <see cref="WireNode.WireNodeId"/>
        /// field will then be equal to one. Moreover, the output pin of the beginning node also takes an ID of one
        /// as it is the first pin created in this dialog.
        /// </para>
        /// <para>
        /// It is recommended to call the <see cref="UnityEditor.EditorApplication.SaveAssets"/> function in order
        /// to ensure that the dialog asset will be saved after the initialization of the dialog.
        /// </para>
        /// </summary>
        /// <param name="beginning">The node that will be the beginning of all the Wired Logic of the dialog.</param>
        public void Initialize(ref DialogBeginningWireNode beginning)
        {
            currWirePinId = 1;
            currWireNodeId = 1;
            dialogName = string.Empty;
            NodesDictionary = new Dictionary<uint, WireNode>();
            Pins = new Dictionary<uint, WirePin>();

            beginning.WireNodeId = GetNextWireNodeId();
            NodesDictionary.Add(beginning.WireNodeId, beginning);

            beginning.Outputs[0].WirePinId = GetNextWirePinId();
            Pins.Add(beginning.Outputs[0].WirePinId, beginning.Outputs[0]);

            // Do not do anything with the nodesList field, as it will be managed via the Save() function
            // Same thing with the serializedNodes field

            initialized = true;

            // Then save the dialog
            Save(null);
        }

        /// <summary>
        /// Saves the dialog. The savingCallback parameter let you pass a function that will be called every time
        /// a node is saved. This allows you to do some special saving stuff.
        /// </summary>
        /// <param name="savingCallback">Function that will be called every time a node is saved</param>
        public void Save(Action<WireNode> savingCallback = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            nodesList.Clear();
            foreach (WireNode node in NodesDictionary.Values)
            {
                if (savingCallback != null)
                    savingCallback.Invoke(node);
                nodesList.Add(node);
            }

            serializedNodes = JsonConvert.SerializeObject(nodesList, settings);
#if (ENABLE_DEBUG_DIALOG)
            Debug.Log(serializedNodes);
#endif
            if (serializedNodes == null)
                serializedNodes = "";
        }

        /// <summary>
        /// Loads the dialog: reads the asset file to create the nodes, and fills the maps of the nodes and the connections. It then
        /// connects the pins according to the data read from the file.
        /// It then calls the Load() function of all nodes that implement the <see cref="ILoadable"/> interface.
        /// </summary>
        public void Load()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            if (serializedNodes == null)
                serializedNodes = "";
            nodesList = JsonConvert.DeserializeObject<List<WireNode>>(serializedNodes, settings);
            Pins = new Dictionary<uint, WirePin>();

            if (nodesList == null)
                nodesList = new List<WireNode>();

            NodesDictionary = new Dictionary<uint, WireNode>();
            foreach (WireNode node in nodesList)
            {
                if (!NodesDictionary.ContainsKey(node.WireNodeId))
                    NodesDictionary.Add(node.WireNodeId, node);
            }

            foreach (WireNode node in NodesDictionary.Values)
            {
                foreach (WirePin input in node.Inputs)
                {
                    input.AssociatedDialog = this;
                    if (!Pins.ContainsKey(input.WirePinId))
                        Pins.Add(input.WirePinId, input);
                }

                foreach (WirePin output in node.Outputs)
                {
                    output.AssociatedDialog = this;
                    if (!Pins.ContainsKey(output.WirePinId))
                        Pins.Add(output.WirePinId, output);
                }
            }

            foreach (WireNode node in NodesDictionary.Values)
            {
                foreach (WirePin output in node.Outputs)
                    if (output.IsConnected)
                        output.Connect(output.GetConnectedPin(), true);

                foreach (WirePin input in node.Inputs)
                    if (input.IsConnected)
                        input.Connect(input.GetConnectedPin(), true);
            }

            foreach (WireNode node in NodesDictionary.Values)
            {
                if (node is ILoadable)
                    (node as ILoadable).Load();
            }
        }

        /// <summary>
        /// Unloads the dialog: it first calls the <see cref="ILoadable.Unload"/> function of all the nodes implementing the
        /// <see cref="ILoadable"/> interface. Then, it clears the nodes and the pins maps.
        /// </summary>
        public void Unload()
        {
            foreach (WireNode node in NodesDictionary.Values)
            {
                if (node is ILoadable)
                    (node as ILoadable).Unload();
            }
            NodesDictionary.Clear();
            Pins.Clear();
        }

        /// <summary>
        /// Returns the ID to use to create a new WirePin. Then it increments the counter of these ids.
        /// </summary>
        /// <returns>Returns the ID to use to create a new WirePin.</returns>
        public uint GetNextWirePinId()
        {
            return currWirePinId++;
        }

        /// <summary>
        /// Returns the ID to use to create a new WireNode. Then it increments the counter of these ids.
        /// </summary>
        /// <returns>Returns the ID to use to create a new WireNode.</returns>
        public uint GetNextWireNodeId()
        {
            return currWireNodeId++;
        }

        public bool HasCorrectBeginning()
        {
            if (NodesDictionary.Values.OfType<DialogBeginningWireNode>().Any())
            {
                DialogBeginningWireNode beginning = NodesDictionary.Values.OfType<DialogBeginningWireNode>().ElementAt(0);
                if (beginning == null || beginning.Outputs == null ||
                    beginning.Outputs.Any(output => output.PinName == "Outgoing activity" && output.DataType == WDEngine.ActivityStream))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the beginning of the dialog. Is this function returns null, then the dialog is probably corrupted
        /// unless you are doing some veeeeeeery special operations on it.
        /// </summary>
        /// <returns>Returns the beginning of the dialog.</returns>
        public DialogBeginningWireNode GetBeginning()
        {
            if (NodesDictionary.ContainsKey(1))
                return NodesDictionary[1] as DialogBeginningWireNode;
            return null;
        }
    }
}