using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{

    abstract public class Item
    {
        public enum Type
        {
            Note,
            Chord,
            Bar
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
        public Note[] notes { get; private set; }

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
}
