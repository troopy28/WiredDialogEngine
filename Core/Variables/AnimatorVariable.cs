using Assets.WiredTools.WiredDialogEngine.Core.Nodes.Variables;
using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Variables
{
    [Serializable]
    public class AnimatorVariable
    {
        /// <summary>
        /// The name of the animator variable.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The string representation of the value of the variable. This is where the value of the
        /// variable is stored.
        /// </summary>
        public string StoredValue { get; set; }
        /// <summary>
        /// The type of the animator variable.
        /// </summary>
        public AnimatorVariableType Type { get; set; }

        /// <summary>
        /// Returns the value of the animator variable. It tries to cast the value of the variable from its string
        /// representation to the specified type T. If the cast isn't possible, then the default value of T is 
        /// returned.
        /// </summary>
        /// <typeparam name="T">The type of the variable you want to get. T must be either a float, a boolean or an integer.</typeparam>
        /// <returns></returns>
        public T GetValueAs<T>()
        {
            if (StoredValue == null)
                return default(T);
            try
            {
                switch (Type)
                {
                    case AnimatorVariableType.BOOL:
                        return (T)(object)bool.Parse(StoredValue);
                    case AnimatorVariableType.FLOAT:
                        return (T)(object)float.Parse(StoredValue);
                    case AnimatorVariableType.INT:
                        return (T)(object)int.Parse(StoredValue);
                    default:
                        return default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}