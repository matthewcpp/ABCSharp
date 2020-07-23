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
            DoubleLine,
            Final,
            RepeatStart,
            RepeatEnd,
            RepeatEndStart,
            Start
        }

        public Kind kind { get;}

        public Bar(Kind t) : base(Item.Type.Bar)
        {
            kind = t;
        }
    }

    public class CustomBar : Item
    {
        public string str { get; set; }

        public CustomBar(string str) : base(Item.Type.CustomBar)
        {
            this.str = str;
        }
    }
}
