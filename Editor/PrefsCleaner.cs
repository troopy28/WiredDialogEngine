using UnityEditor;
using UnityEngine;

namespace Assets.WiredTools.WiredDialogEngine.Editor
{
    public static class PrefsCleaner
    {
        private const string VariablesPrefKeyCst = "dialogengine:variables";
        private const string FirstLoadCst = "dialogengine:firstload";

        [MenuItem("Tools/Wired Dialog Engine/Clear WDE variables", false, 1)]
        public static void CleanVars()
        {
            if (EditorUtility.DisplayDialog(
                "Remove WDE variables",
                "Are you sure to want to remove all the stored WDE variables, and the firstLoad check data?",
                "Yeah, of course!",
                "No!"))
            {
                if (PlayerPrefs.HasKey(VariablesPrefKeyCst))
                    PlayerPrefs.DeleteKey(VariablesPrefKeyCst);
                if (PlayerPrefs.HasKey(FirstLoadCst))
                    PlayerPrefs.DeleteKey(FirstLoadCst);
                EditorUtility.DisplayDialog("Success", "The WDE variables, and the firstLoad check data have been removed.", "OK");
            }
        }
    }
}