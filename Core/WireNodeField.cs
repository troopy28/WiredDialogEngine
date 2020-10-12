using System;

namespace Assets.WiredTools.WiredDialogEngine.Core
{
    [Serializable]
    public class WireNodeField<T>
    {
        public WireNode Owner { get; protected set; }
        public Type DataType { get; private set; }


        public WireNodeField(WireNode owner)
        {
            Owner = owner;
            DataType = typeof(T);
        }
    }
}