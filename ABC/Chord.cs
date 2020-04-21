using System;
using System.Collections.Generic;

namespace ABC
{
    public class Chord : Duration
    {
        public struct Element : IComparable<Element>
        {
            public readonly Pitch pitch;
            public readonly Accidental accidental;

            public Element(Pitch pitch, Accidental accidental)
            {
                this.pitch = pitch;
                this.accidental = accidental;
            }
            
            public int CompareTo(Element other)
            {
                return pitch.CompareTo(other.pitch);
            }
        }
        
        public Element[] notes { get; }

        /// <summary>
        /// Creates a new chord made up of the supplied note elements.
        /// </summary>
        /// <param name="notes">An array of chord elements which make up the notes of the chord.  Note that this array will be sorted.</param>
        public Chord(Element[] notes) : base(Item.Type.Chord)
        {
            this.notes = notes;
            Array.Sort(this.notes);
        }

        /// <summary>
        /// Creates a new Chord from a list of notes.
        /// The pitch and accidental will be extracted from each note to make up the elements of the chord.
        /// </summary>
        /// <param name="notes">List of notes which will make up the elements of the Chord.</param>
        /// <returns></returns>
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