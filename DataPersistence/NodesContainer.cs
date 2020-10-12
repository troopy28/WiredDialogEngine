using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Comparation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Data;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Talking;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Animation;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes;
using Assets.WiredTools.WiredDialogEngine.Core;
using Assets.WiredTools.WiredDialogEngine.Core.Nodes.FlowControl;

namespace Assets.WiredTools.WiredDialogEngine.DataPersistence
{
    [Serializable]
    public class NodesContainer
    {
        [HideInInspector]
        [SerializeField]
        private List<EqualityWireNode> equalityNodes;
        [HideInInspector]
        [SerializeField]
        private List<GreaterWireNode> greaterNodes;
        [HideInInspector]
        [SerializeField]
        private List<InequalityWireNode> inequalityNodes;
        [HideInInspector]
        [SerializeField]
        private List<LesserWireNode> lesserNodes;
        [HideInInspector]
        [SerializeField]
        private List<ConstantWireNode> constantNodes;
        [HideInInspector]
        [SerializeField]
        private List<SayReplyWireNode> sayReplyNodes;
        [HideInInspector]
        [SerializeField]
        private List<BranchWireNode> branchNodes;
        [HideInInspector]
        [SerializeField]
        private List<SetAnimatorVariableWireNode> setAnimVarNodes;
        [HideInInspector]
        [SerializeField]
        private List<GetAnimatorVariableWireNode> getAnimVarNodes;
        [HideInInspector]
        [SerializeField]
        private List<ReunionWireNode> reunionNodes;
        [HideInInspector]
        [SerializeField]
        private List<GetVariableWireNode> getVarNodes;
        [HideInInspector]
        [SerializeField]
        private List<ChooseReplyWireNode> chooseReplyNodes;
        // TODO. See below
        private List<GetVariableWireNode> setVarNodes; // TODO : SET VARIABLES AND NOT GET

        public List<EqualityWireNode> EqualityNodes
        {
            get
            {
                return equalityNodes;
            }

            set
            {
                equalityNodes = value;
            }
        }
        public List<GreaterWireNode> GreaterNodes
        {
            get
            {
                return greaterNodes;
            }

            set
            {
                greaterNodes = value;
            }
        }
        public List<InequalityWireNode> InequalityNodes
        {
            get
            {
                return inequalityNodes;
            }

            set
            {
                inequalityNodes = value;
            }
        }
        public List<LesserWireNode> LesserNodes
        {
            get
            {
                return lesserNodes;
            }

            set
            {
                lesserNodes = value;
            }
        }
        public List<ConstantWireNode> ConstantNodes
        {
            get
            {
                return constantNodes;
            }

            set
            {
                constantNodes = value;
            }
        }
        public List<SayReplyWireNode> SayReplyNodes
        {
            get
            {
                return sayReplyNodes;
            }

            set
            {
                sayReplyNodes = value;
            }
        }
        public List<BranchWireNode> BranchNodes
        {
            get
            {
                return branchNodes;
            }

            set
            {
                branchNodes = value;
            }
        }
        public List<SetAnimatorVariableWireNode> SetAnimVarNodes
        {
            get
            {
                return setAnimVarNodes;
            }

            set
            {
                setAnimVarNodes = value;
            }
        }
        public List<GetAnimatorVariableWireNode> GetAnimVarNodes
        {
            get
            {
                return getAnimVarNodes;
            }

            set
            {
                getAnimVarNodes = value;
            }
        }
        public List<ReunionWireNode> ReunionNodes
        {
            get
            {
                return reunionNodes;
            }

            set
            {
                reunionNodes = value;
            }
        }
        public List<GetVariableWireNode> GetVarNodes
        {
            get
            {
                return getVarNodes;
            }

            set
            {
                getVarNodes = value;
            }
        }
        public List<ChooseReplyWireNode> ChooseReplyNodes
        {
            get
            {
                return chooseReplyNodes;
            }

            set
            {
                chooseReplyNodes = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private DialogBeginningWireNode beginning;
        public DialogBeginningWireNode DialogBeginning
        {
            get
            {
                return beginning;
            }
            set
            {
                beginning = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private List<PinConnection> connections;
        public List<PinConnection> Connections
        {
            get
            {
                return connections;
            }
            set
            {
                connections = value;
            }
        }

        public List<WireNode> Nodes { get; set; }


        public void Initialize()
        {
            Nodes = new List<WireNode>();
            Connections = new List<PinConnection>();

            if (Application.isEditor)
            {
                if (equalityNodes == null)
                    equalityNodes = new List<EqualityWireNode>();

                if (greaterNodes == null)
                    greaterNodes = new List<GreaterWireNode>();

                if (lesserNodes == null)
                    lesserNodes = new List<LesserWireNode>();

                if (inequalityNodes == null)
                    inequalityNodes = new List<InequalityWireNode>();

                if (constantNodes == null)
                    constantNodes = new List<ConstantWireNode>();

                if (sayReplyNodes == null)
                    sayReplyNodes = new List<SayReplyWireNode>();

                if (branchNodes == null)
                    branchNodes = new List<BranchWireNode>();

                if (setAnimVarNodes == null)
                    setAnimVarNodes = new List<SetAnimatorVariableWireNode>();

                if (getAnimVarNodes == null)
                    getAnimVarNodes = new List<GetAnimatorVariableWireNode>();

                if (reunionNodes == null)
                    reunionNodes = new List<ReunionWireNode>();

                if (getVarNodes == null)
                    getVarNodes = new List<GetVariableWireNode>();

                if (chooseReplyNodes == null)
                    chooseReplyNodes = new List<ChooseReplyWireNode>();
            }
        }

        #region Dialog saving

        public void Save()
        {
            SaveNodes();
            SaveConnections(); // Do this AFTER the nodes saving
        }

        private void SaveNodes()
        {
            if (Nodes != null)
            {
                foreach (WireNode node in Nodes)
                {
                    if (node is EqualityWireNode)
                        equalityNodes.Add(node as EqualityWireNode);
                    else if (node is GreaterWireNode)
                        greaterNodes.Add(node as GreaterWireNode);
                    else if (node is InequalityWireNode)
                        inequalityNodes.Add(node as InequalityWireNode);
                    else if (node is LesserWireNode)
                        lesserNodes.Add(node as LesserWireNode);
                    else if (node is ConstantWireNode)
                        constantNodes.Add(node as ConstantWireNode);
                    else if (node is SayReplyWireNode)
                        sayReplyNodes.Add(node as SayReplyWireNode);
                    else if (node is BranchWireNode)
                        branchNodes.Add(node as BranchWireNode);
                    else if (node is SetAnimatorVariableWireNode)
                        setAnimVarNodes.Add(node as SetAnimatorVariableWireNode);
                    else if (node is GetAnimatorVariableWireNode)
                        getAnimVarNodes.Add(node as GetAnimatorVariableWireNode);
                    else if (node is ReunionWireNode)
                        reunionNodes.Add(node as ReunionWireNode);
                    else if (node is GetVariableWireNode)
                        getVarNodes.Add(node as GetVariableWireNode);
                    else if (node is ChooseReplyWireNode)
                        chooseReplyNodes.Add(node as ChooseReplyWireNode);
                }
                Nodes.Clear();
            }
            Debug.Log(JsonUtility.ToJson(sayReplyNodes, true));
        }

        private void SaveConnections()
        {
            // TODO : OPTIMIZATION
            List<PinConnection> pinConnections = new List<PinConnection>();

            foreach (WireNode node in equalityNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in greaterNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in inequalityNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in lesserNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in constantNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in sayReplyNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in branchNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in setAnimVarNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in getAnimVarNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in reunionNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in getVarNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            foreach (WireNode node in chooseReplyNodes)
            {
                foreach (WirePin pin in node.Inputs)
                {
                    if (pin.IsConnected)
                    {
                        WirePin other = pin.GetConnectedPin();
                        PinConnection connection = new PinConnection()
                        {
                            PinOne = pin.WirePinId,
                            PinTwo = other.WirePinId
                        };
                        pinConnections.Add(connection);
                    }
                }
            }

            connections = pinConnections;
        }

        #endregion
    }
}
