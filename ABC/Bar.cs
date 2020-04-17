using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Bar : Item
    {
        public enum Kind
        {
            Line
        }

        public Kind kind { get;}

        public Bar(Kind t) : base(Item.Type.Bar)
        {
            kind = t;
        }
    }
}
