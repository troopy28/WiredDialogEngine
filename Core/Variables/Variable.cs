using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Variables;
using Assets.WiredTools.WiredDialogEngine.Runtime;
using Newtonsoft.Json;
using System;
using UnityEngine;
using Assets.WiredTools.WiredDialogEngine.DataPersistence;

namespace Assets.WiredTools.WiredDialogEngine.Core.Variables
{
    [Serializable]
    [CreateAssetMenu(fileName = "Variable", menuName = "Wired Dialog Engine/Variable", order = 3)]
    public class Variable : ScriptableObject
    {
        [JsonProperty]
        [SerializeField]
        private string variableName;
        /// <summary>
        /// The unique name of the variable. Two variables cannot have the same or strange behavior
        /// will happen with the dialog interpretation.
        /// </summary>
        [JsonIgnore]
        public string VariableName
        {
            get
            {
                return variableName;
            }

            set
            {
                variableName = value;
            }
        }

        [Tooltip("The unique name of your variable.")]
        [JsonProperty]
        [SerializeField]
        private VariableType type;
        /// <summary>
        /// The type of the variable.
        /// </summary>
        [JsonIgnore]
        public VariableType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// The raw, uncasted value of the variable.
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; }
        [JsonProperty]
        [SerializeField]
        private string savedValue;

        /// <summary>
        /// The first value the variable has, before any value modification in
        /// the dialogs, at the first load of the game.
        /// </summary>
        [SerializeField]
        [JsonProperty]
        public string editorBaseValue;

        [JsonIgnore]
        private bool loaded;

        /// <summary>
        /// Creates a new empty <see cref="Variable"/> object.
        /// </summary>
        public Variable()
        {
            // Used by NewtonSoft JSON.NET
        }

        /// <summary>
        /// Creates a new <see cref="Variable"/> object of the specified type with the specified value.
        /// </summary>
        /// <param name="type">Type of your variable.</param>
        /// <param name="value">Value of your variable.</param>
        public Variable(VariableType type, object value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Saves the variable. Once the variable has been saved using this method, it is still not saved on the disk.
        /// To save it on the disk, you need to call the <see cref="DialogFilesManager.SaveVariables(VariablesContainer)"/>,
        /// and specify the currently used <see cref="VariablesContainer"/>.
        /// </summary>
        /// <param name="runtime">True if the game is not running in the editor.</param>
        public void Save(bool runtime)
        {
            if (Value == null)
                return;
            savedValue = Value.ToString();
            if (!runtime)
                editorBaseValue = savedValue;
            
        }

        /// <summary>
        /// Loads the variable to prepare its use in the game. A variable that isn't loaded does not have a correct value.
        /// </summary>
        /// <param name="runtime">Is this method called at runtime (true) or in by editor (false).</param>
        public void Load(bool runtime)
        {
            if (loaded)
                return;
            if (type == VariableType.FLOAT)
            {
                if (Value == null)
                    Value = default(float);
                float value = (float)Value;

                if (runtime)
                {
                    if (!float.TryParse(savedValue, out value))
                        value = 0;
                }
                else
                {
                    if (!float.TryParse(editorBaseValue, out value))
                        value = 0;
                }

                Value = value;
            }

            else if (type == VariableType.INT)
            {
                if (Value == null)
                    Value = default(int);
                int value = (int)Value;

                if (runtime)
                {
                    if (!int.TryParse(savedValue, out value))
                        value = 0;
                }
                else
                {
                    if (!int.TryParse(editorBaseValue, out value))
                        value = 0;
                }

                Value = value;
            }
            else if (type == VariableType.STRING)
            {
                if (runtime)
                    Value = savedValue;
                else
                    Value = editorBaseValue;
            }
            loaded = true;
        }

        /// <summary>
        /// Unloads the variable (if the variable is loaded). An unloaded variable doesn't have a value.
        /// </summary>
        public void Unload()
        {
            if (!loaded)
                return;
            Value = null;
            loaded = false;
        }

        /// <summary>
        /// Returns the value of your variable in the type you want to. Take care of the type of your variable, or a <see cref="InvalidCastException"/>
        /// will be thrown.
        /// </summary>
        /// <typeparam name="T">The type of the returned variable.</typeparam>
        /// <returns>Returns the value of your variable in the type you want to.</returns>
        public T GetValueAs<T>()
        {
            if (!loaded)
                Load(DialogEngineRuntime.IsRuntime);

            if (Value == null)
                return default(T);
            try
            {
                return (T)Value;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// The recommended way to compare a variable to anything else. 
        /// </summary>
        /// <param name="o">The object to compare to the variable.</param>
        /// <returns>Returns that the other object is equal to the variable.</returns>
        public override bool Equals(object o)
        {
            if (!loaded)
                Load(DialogEngineRuntime.IsRuntime);

            // First check for the different variables types
            if (o is Variable)
            {
                Variable var = (Variable)o;
                if (var.Type != Type)
                    return false;

                if ((var.Value == null && Value != null) ||
                    (var.Value != null && Value == null))
                    return false;
                if (var.Value == null && Value == null)
                    return true;

                return var.Value.ToString().Equals(Value.ToString());
            }
            else if (o is Constant)
            {
                Constant c = (Constant)o;
                if (c.Type != Type)
                    return false;

                if ((c.Value == null && Value != null) ||
                    (c.Value != null && Value == null))
                    return false;
                if (c.Value == null && Value == null)
                    return true;

                return c.Value.ToString().Equals(Value.ToString());
            }
            else if (o is AnimatorVariable)
            {
                AnimatorVariable av = (AnimatorVariable)o;
                if (!av.Type.Correspond(Type)) // If types not correspond, not equal
                    return false;

                if (((av.StoredValue == null || av.StoredValue == string.Empty) && Value != null) ||
                    ((av.StoredValue != null && av.StoredValue != string.Empty) && Value == null))
                    return false;
                if ((av.StoredValue == null || av.StoredValue == string.Empty) && Value == null)
                    return true;

                return av.StoredValue.ToString().Equals(Value.ToString());
            }

            // Then check for the raw types
            else if (o is int)
            {
                if (Type != VariableType.INT)
                    return false;
                return GetValueAs<int>() == (int)o;
            }
            else if (o is float)
            {
                if (Type != VariableType.FLOAT)
                    return false;
                return GetValueAs<float>() == (float)o;
            }
            else if (o is string)
            {
                if (Type != VariableType.STRING)
                    return false;
                return GetValueAs<string>() == (string)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int valueHash = 0;
            int typeHash = 0;
            if (Value != null)
                valueHash = Value.GetHashCode();
            typeHash = Type.GetHashCode();
            return valueHash + typeHash;
        }

        /// <summary>
        /// <para>
        /// Set the next value of the variable that will be saved in the container. Be extremely careful with this.
        /// The Dialog Editor already automatically handle the saving of the variables, so try not to use it
        /// unless you're sure of what you're doing.
        /// </para>
        /// </summary>
        /// <param name="value">The new value to save.</param>
        public void SetSavedValue(string value)
        {
            savedValue = value;
        }

        /// <summary>
        /// Changes the value of the variable at runtime. The value is then ready to save, so a call to the <see cref="Save(bool)"/> method will save the 
        /// new value of the variable. After changing the value, it reloads the variable, so accessing to its parameters may give strange results.
        /// MUST BE CALLED AT RUNTIME, NOT IN THE EDITOR.
        /// </summary>
        /// <param name="stringValue">The value to give to the variable.</param>
        public void SetValueAtRuntime(string stringValue)
        {
            SetSavedValue(stringValue);
            Unload();
            Load(true);
        }

        /// <summary>
        /// Changes the value of the variable at runtime. The value is then ready to save, so a call to the <see cref="Save(bool)"/> method will save the 
        /// new value of the variable. After changing the value, it reloads the variable, so accessing to its parameters may give strange results.
        /// MUST BE CALLED AT RUNTIME, NOT IN THE EDITOR.
        /// </summary>
        /// <param name="intValue">The value to give to the variable.</param>
        public void SetValueAtRuntime(int intValue)
        {
            SetSavedValue(intValue.ToString());
            Unload();
            Load(true);
        }

        /// <summary>
        /// Changes the value of the variable at runtime. The value is then ready to save, so a call to the <see cref="Save(bool)"/> method will save the 
        /// new value of the variable. After changing the value, it reloads the variable, so accessing to its parameters may give strange results.
        /// MUST BE CALLED AT RUNTIME, NOT IN THE EDITOR.
        /// </summary>
        /// <param name="floatValue">The value to give to the variable.</param>
        public void SetValueAtRuntime(float floatValue)
        {
            SetSavedValue(floatValue.ToString());
            Unload();
            Load(true);
        }

        // Below are operators overloading

        #region Two variables

        public static bool operator ==(Variable c1, Variable c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Variable c1, Variable c2)
        {
            return !c1.Equals(c2);
        }

        #endregion

        #region A variable and a constant

        public static bool operator ==(Variable c1, Constant c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Variable c1, Constant c2)
        {
            return !c1.Equals(c2);
        }

        public static bool operator ==(Constant c1, Variable c2)
        {
            return c2.Equals(c1);
        }

        public static bool operator !=(Constant c1, Variable c2)
        {
            return !c2.Equals(c1);
        }

        #endregion

        #region A variable and an animator variable

        public static bool operator ==(Variable c1, AnimatorVariable c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Variable c1, AnimatorVariable c2)
        {
            return !c1.Equals(c2);
        }

        public static bool operator ==(AnimatorVariable c1, Variable c2)
        {
            return c2.Equals(c1);
        }

        public static bool operator !=(AnimatorVariable c1, Variable c2)
        {
            return !c2.Equals(c1);
        }

        #endregion
    }
}