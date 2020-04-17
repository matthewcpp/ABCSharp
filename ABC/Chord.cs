using System.Collections.Generic;

namespace ABC
{
    public class Chord : Duration
    {
        public struct Element
        {
            public readonly Pitch pitch;
            public readonly Accidental accidental;

            public Element(Pitch pitch, Accidental accidental)
            {
                this.pitch = pitch;
                this.accidental = accidental;
            }
        }
        
        public Element[] notes { get; }

        public Chord(Element[] notes) : base(Item.Type.Chord)
        {
            this.notes = notes;
        }

        public static Chord FromNotes(List<Note> notes)
        {
            var chordNotes = new Element[notes.Count];

            for (int i = 0; i < notes.Count; i++)
                chordNotes[i] = new Element(notes[i].pitch, notes[i].accidental);
            
            var chord = new Chord(chordNotes);
            return chord;
        }
    }
}