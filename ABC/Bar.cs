using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Bar : Item
    {
        public enum Type
        {
            Line
        }

        public Type type { get;}

        public Bar(Type t) : base(Item.Type.Bar)
        {
            type = t;
        }
    }
}
