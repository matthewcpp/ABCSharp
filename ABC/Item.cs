using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{

    public abstract class Item
    {
        public enum Type
        {
            Bar,
            Chord,
            MultiMeasureRest,
            Note,
            Rest,
            TimeSignature
        }

        public Type type { get; }

        public Item(Type t)
        {
            type = t;
        }
    }
}
