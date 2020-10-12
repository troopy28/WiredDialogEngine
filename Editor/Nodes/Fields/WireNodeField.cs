using System;

namespace Assets.WiredTools.WiredDialogEngine.Editor.Nodes.Fields
{
    public abstract class WireNodeField<T>
    {
        public WireNodeDisplayer Owner { get; protected set; }
        public Type DataType { get; protected set; }
        public T FieldValue { get; set; }
        public string FieldName { get; set; }

        protected WireNodeField(WireNodeDisplayer owner)
        {
            Owner = owner;
            DataType = typeof(T);
        }

        public abstract void Draw();
    }
}