using Assets.WiredTools.WiredDialogEngine.Core.Variables;

namespace Assets.WiredTools.WiredDialogEngine.Core.Nodes.Variables
{
    public enum AnimatorVariableType
    {
        FLOAT,
        INT,
        BOOL,
        TRIGGER
    }

    public static class AnimatorVariableTypeExt
    {
        public static AnimatorVariableType Copy(this AnimatorVariableType var)
        {
            switch (var)
            {
                case AnimatorVariableType.FLOAT:
                    return AnimatorVariableType.FLOAT;
                case AnimatorVariableType.INT:
                    return AnimatorVariableType.INT;
                case AnimatorVariableType.BOOL:
                    return AnimatorVariableType.BOOL;
                default:
                    return AnimatorVariableType.TRIGGER;
            }
        }

        /// <summary>
        /// Returns that the type of the specified variable corresponds to this <see cref="AnimatorVariableType"/>.
        /// </summary>
        /// <param name="var"></param>
        /// <param name="other">The type to check.</param>
        /// <returns>Returns that the type of the specified variable corresponds to this <see cref="AnimatorVariableType"/>.</returns>
        public static bool Correspond(this AnimatorVariableType var, VariableType other)
        {
            switch(var)
            {
                case AnimatorVariableType.FLOAT:
                    return other == VariableType.FLOAT;
                case AnimatorVariableType.INT:
                    return other == VariableType.INT;

                case AnimatorVariableType.TRIGGER:
                case AnimatorVariableType.BOOL:
                default:
                    return false;
            }
        }
    }
}
