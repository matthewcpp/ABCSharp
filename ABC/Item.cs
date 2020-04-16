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

    public class BarItem : Item
    {
        public Bar bar {get;}

        public BarItem(Bar b) : base(Type.Bar)
        {
            bar = b;
        }
    }

    public class TimeSignatureItem : Item
    {
        public string timeSignature { get; }

        public TimeSignatureItem(string ts) : base(Type.TimeSignature)
        {
            timeSignature = ts;
        }
    }

    public class MultiMeasureRestItem : Item
    {
        public MultiMeasureRest rest { get; }

        public MultiMeasureRestItem(MultiMeasureRest rest) : base(Type.MultiMeasureRest)
        {
            this.rest = rest;
        }
    }
}
