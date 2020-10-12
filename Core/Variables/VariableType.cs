using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Variables
{
    [Serializable]
    public enum VariableType
    {
        STRING,
        INT,
        FLOAT
    }

    public static class VariableTypeExt
    {
        public static VariableType Copy(this VariableType var)
        {
            switch(var)
            {
                case VariableType.FLOAT:
                    return VariableType.FLOAT;
                case VariableType.INT:
                    return VariableType.INT;
                default:
                    return VariableType.STRING;
            }
        }
    }
}
