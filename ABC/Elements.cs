using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    class Elements
    {
        public static Dictionary<int, int> notes = new Dictionary<int, int>()
        {
            {'c', 7 }, {'d', 8 }, {'e', 9 }, {'f', 10 }, {'g', 11 }, { 'a', 12 }, {'b', 13 },
            {'C', 0 }, {'D', 1 }, {'E', 2 }, {'F', 3 }, {'G', 4 }, { 'A', 5 }, {'B', 6 }
        };

        public static Dictionary<int, Note.Accidental> accidentals = new Dictionary<int, Note.Accidental>()
        {
            { '=', Note.Accidental.Natural }, {'^', Note.Accidental.Sharp }, {'_', Note.Accidental.Flat }
        };

        public static Dictionary<int, int> octaveModifiers = new Dictionary<int, int>()
        {
            { ',', -7 }, { '\'', 7 }
        };

        public static bool IsStartOfNoteStream(int val)
        {
            return Elements.notes.ContainsKey(val) || Elements.accidentals.ContainsKey(val);
        }

        public static int GetNoteOffset(int val)
        {
            if (notes.TryGetValue(val, out int offset))
                return offset;
            else
                throw new ParseException("Invalid note value");
        }

        public static Note.Accidental GetAccidental(int val)
        {
            Note.Accidental accidental = Note.Accidental.Unspecified;
            accidentals.TryGetValue(val, out accidental);
            return accidental;
        }
    }
}
