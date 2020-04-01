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
}
