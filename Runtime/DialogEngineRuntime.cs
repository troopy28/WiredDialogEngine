using Assets.WiredTools.WiredDialogEngine.DataPersistence;
using Assets.WiredTools.WiredDialogEngine.Runtime.Interaction;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Runtime
{
    [AddComponentMenu("Wired Dialog Engine/Dialog engine runtime")]
    public class DialogEngineRuntime : MonoBehaviour
    {
        public static DialogEngineRuntime Instance { get; private set; }
        /// <summary>
        /// True when called at runtime.
        /// False when called in the editor.
        /// </summary>
        public static bool IsRuntime { get; private set; }

        [SerializeField]
        private string dialogLanguage;
        public string DialogLanguage
        {
            get
            {
                return dialogLanguage;
            }
        }

        [Tooltip("Force the variables system to act like if it was the first time you load the game." +
            " For more information, look at the documentation. UNCHECK IT IF YOU ARE DOING THE PRODUCTION VERSION!!!")]
        [SerializeField]
        private bool forceFirstGameLoad;
        public bool ForceFirstGameLoad
        {
            get
            {
                return forceFirstGameLoad;
            }
            set
            {
                forceFirstGameLoad = value;
            }
        }

        [SerializeField]
        private VariablesContainer variables;
        /// <summary>
        /// The object that contains the variables currently used by the Dialog Engine.
        /// </summary>
        public VariablesContainer VariablesContainer
        {
            get
            {
                return variables;
            }
            set
            {
                variables = value;
            }
        }

        [SerializeField]
        private AbstractDialogOverlay dialogDisplayer;
        public AbstractDialogOverlay DialogDisplayer
        {
            get
            {
                return dialogDisplayer;
            }
            set
            {
                dialogDisplayer = value;
            }
        }

        private void Awake()
        {
            Instance = this;
            IsRuntime = true;
            // If first load, save the vars and set the first load flag to false
            if (DialogFilesManager.FirstLoad() || forceFirstGameLoad)
            {
                DialogFilesManager.SaveVariables(variables);
                DialogFilesManager.SetFirstLoad(false);
            }
            VariablesContainer vars = DialogFilesManager.GetVariablesFromDisk();
            VariablesContainer = vars;
        }

        /// <summary>
        /// Returns the variable container used by the WDE currently.
        /// </summary>
        /// <returns>Returns the variable container used by the WDE currently.</returns>
        public static VariablesContainer GetCurrentVariablesContainer()
        {
            return Instance.variables;
        }
    }
}