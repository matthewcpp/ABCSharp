using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Note : IComparable<Note>
    {
        public Value value { get; set; }
        public Accidental accidental { get; set; }

        public Length length { get; set; }

        public Note(Value v = Value.C4, Length l = Length.Eighth, Accidental acc = Accidental.Unspecified)
        {
            value = v;
            length = l;
            accidental = acc;
        }

        public enum Length
        {
            Whole = 1,
            Half = 2,
            Quarter = 4,
            Eighth = 8,
            Sixteenth = 16
        }

        public enum Value
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
                   value == note.value &&
                   accidental == note.accidental &&
                   length == note.length;
        }

        public override int GetHashCode()
        {
            var hashCode = 838805952;
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            hashCode = hashCode * -1521134295 + accidental.GetHashCode();
            hashCode = hashCode * -1521134295 + length.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{value.ToString()}, {accidental.ToString()}, {length.ToString()}";
        }

        public int CompareTo(Note other)
        {
            return value.CompareTo(other.value);
        }
    }

}
