using Assets.WiredTools.WiredDialogEngine.Core.Variables;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.DataPersistence
{
    /// <summary>
    /// The class responsible of saving / reading dialog files on the disk, in order to provide data persistence to the
    /// dialogs. All disk access are cross-platforms as this class makes use of the Unity <see cref="PlayerPrefs"/>
    /// class to save the data.
    /// </summary>
    public static class DialogFilesManager
    {
        private const string VariablesPrefKeyCst = "dialogengine:variables";
        private const string FirstLoadCst = "dialogengine:firstload";

        /// <summary>
        /// Returns the <see cref="VariablesContainer"/> containing the dialog variables from the disk. These variables can be modified at
        /// runtime, unlike the dialog constants.
        /// </summary>
        /// <returns>Returns the <see cref="VariablesContainer"/> containing the dialog variables from the disk. These variables can be modified at
        /// runtime, unlike the dialog constants.</returns>
        /// <exception cref="T:System.Exception">Throw an exception when the <see cref="PlayerPrefs"/> key used 
        /// to save the variables hasn't been found</exception>
        public static VariablesContainer GetVariablesFromDisk()
        {
            if (!PlayerPrefs.HasKey(VariablesPrefKeyCst))
                throw new Exception("The PlayersPref key used by the DialogEngine hasn't been found. Unable to get the variables from the disk. \n" +
                    " Make sure you have already saved the variables at least one time before use the GetVariablesFromDisk() function.");

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string varsJson = PlayerPrefs.GetString(VariablesPrefKeyCst);

            VariablesContainer vars = JsonConvert.DeserializeObject<VariablesContainer>(varsJson, settings);
            //VariablesContainer vars = JsonUtility.FromJson<VariablesContainer>(varsJson);
            return vars;
        }

        /// <summary>
        /// Save the variables contained in the specified <see cref="VariablesContainer"/> on the disk in order to retrieve them later,
        /// using the <see cref="GetVariablesFromDisk"/> function.
        /// </summary>
        /// <param name="currentContainer"></param>
        public static void SaveVariables(VariablesContainer currentContainer)
        {
            // string varsJson = JsonUtility.ToJson(currentContainer);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string varsJson = JsonConvert.SerializeObject(currentContainer, settings);


            PlayerPrefs.SetString(VariablesPrefKeyCst, varsJson);
            PlayerPrefs.Save();
#if (ENABLE_DEBUG_DIALOG)
            
            StringBuilder sb = new StringBuilder("Saved the Wired Dialog Engine variables. Variables are:" + Environment.NewLine);
            foreach (Variable var in currentContainer.IndependantVariables)
            {
                if (!(((object)var) == null) && !var.Equals(null))
                    sb.Append(var.VariableName + ": '" + var.Value + "'" + Environment.NewLine);
            }
            Debug.Log(sb.ToString());
#endif
        }

        /// <summary>
        /// Returs that the DialogEngine is running for the first time. Warning : it also works in editor.
        /// </summary>
        /// <returns>Returs that the DialogEngine is running for the first time.</returns>
        public static bool FirstLoad()
        {
            return !PlayerPrefs.HasKey(FirstLoadCst); //If the key doesn't exists, then it is the first load
        }

        /// <summary>
        /// Set that wether the DialogEngine is running for the first time or not. 
        /// </summary>
        /// <param name="value"></param>
        public static void SetFirstLoad(bool value)
        {
            if (value && PlayerPrefs.HasKey(FirstLoadCst)) // If first load
                PlayerPrefs.DeleteKey(FirstLoadCst);
            else if (!value) // Otherwise setup the key
                PlayerPrefs.SetInt(FirstLoadCst, 1);
            PlayerPrefs.Save();
        }
    }
}