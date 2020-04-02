using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Bar
    {
        public enum Type
        {
            Line
        }

        public Type type { get; set; }

        public Bar(Type t)
        {
            type = t;
        }
    }
}
