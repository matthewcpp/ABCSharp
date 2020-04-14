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

    public class NoteItem : Item
    {
        public Note note { get; }

        public NoteItem(Note n) : base(Type.Note)
        {
            note = n;
        }
    }

    public class ChordItem : Item
    {
        public Note[] notes { get;}

        public ChordItem(List<Note> ns) : base(Type.Chord)
        {
            notes = ns.ToArray();
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

    public class RestItem : Item
    {
        public Rest rest { get; }

        public RestItem(Rest rest) : base(Type.Rest)
        {
            this.rest = rest;
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
