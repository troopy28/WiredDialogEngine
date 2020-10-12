using System;

namespace Assets.WiredTools.WiredDialogEngine.Core.Variables
{
    public class Constant
    {
        public object Value { get; set; }
        public VariableType Type { get; set; }

        public T GetValueAs<T>()
        {
            if (Value == null)
                return default(T);
            try
            {
                if (Type == VariableType.FLOAT)
                    return (T)(object)float.Parse(Value.ToString());
                else if (Type == VariableType.INT)
                    return (T)(object)int.Parse(Value.ToString());
                else
                    return (T)(object)Value.ToString();
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}