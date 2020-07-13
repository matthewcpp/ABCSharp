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
            CustomBar,
            Chord,
            MultiMeasureRest,
            Note,
            Rest,
            TimeSignature,
            LineBreak,
            Key
        }

        public Type type { get; }

        public int id { get; }
        
        private static int itemId = 0;

        public Item(Type t)
        {
            type = t;
            id = ++itemId;
        }
    }
}
