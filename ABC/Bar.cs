using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Bar : Item
    {
        public enum Kind
        {
            Line,
            ThinThinDoubleBar,
            ThinThickDoubleBar,
            ThickThinDoubleBar
        }

        public Kind kind { get;}
        public int startRepeatCount { get; set; } = 0;
        public int endRepeatCount { get; set; } = 0;

        public Bar(Kind t) : base(Item.Type.Bar)
        {
            kind = t;
        }
    }
}
