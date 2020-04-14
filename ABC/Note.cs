using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public struct Note : IComparable<Note>
    {
        public Pitch pitch { get; set; }
        public Accidental accidental { get; set; }

        public Length length { get; set; }

        public Note(Pitch pitch = Pitch.C4, Length length = Length.Eighth, Accidental accidental = Accidental.Unspecified)
        {
            this.pitch = pitch;
            this.length = length;
            this.accidental = accidental;
        }

        public enum Pitch
        {
            A0,
            B0,

            C1,
            D1,
            E1,
            F1,
            G1,
            A1,
            B1,

            C2,
            D2,
            E2,
            F2,
            G2,
            A2,
            B2,

            C3,
            D3,
            E3,
            F3,
            G3,
            A3,
            B3,

            C4,
            D4,
            E4,
            F4,
            G4,
            A4,
            B4,

            C5,
            D5,
            E5,
            F5,
            G5,
            A5,
            B5,

            C6,
            D6,
            E6,
            F6,
            G6,
            A6,
            B6,

            C7,
            D7,
            E7,
            F7,
            G7,
            A7,
            B7,

            C8
        }

        public enum Accidental
        {
            Unspecified,
            Natural,
            Sharp,
            Flat
        }

        public override bool Equals(object obj)
        {
            return obj is Note note &&
                   pitch == note.pitch &&
                   accidental == note.accidental &&
                   length == note.length;
        }

        public override int GetHashCode()
        {
            var hashCode = 838805952;
            hashCode = hashCode * -1521134295 + pitch.GetHashCode();
            hashCode = hashCode * -1521134295 + accidental.GetHashCode();
            hashCode = hashCode * -1521134295 + length.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{pitch.ToString()}, {accidental.ToString()}, {length.ToString()}";
        }

        public int CompareTo(Note other)
        {
            return pitch.CompareTo(other.pitch);
        }
    }

}
