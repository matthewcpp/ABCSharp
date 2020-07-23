using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    class Elements
    {
        public static readonly Dictionary<int, int> notes = new Dictionary<int, int>()
        {
            {'c', 7 }, {'d', 8 }, {'e', 9 }, {'f', 10 }, {'g', 11 }, { 'a', 12 }, {'b', 13 },
            {'C', 0 }, {'D', 1 }, {'E', 2 }, {'F', 3 }, {'G', 4 }, { 'A', 5 }, {'B', 6 }
        };

        public static readonly Dictionary<int, Accidental> accidentals = new Dictionary<int, Accidental>()
        {
            { '=', Accidental.Natural }, {'^', Accidental.Sharp }, {'_', Accidental.Flat }
        };

        public static readonly Dictionary<int, int> octaveModifiers = new Dictionary<int, int>()
        {
            { ',', -7 }, { '\'', 7 }
        };
        
        public static readonly HashSet<int> rests = new HashSet<int>()
        {
            'z', 'Z', 'x', 'X'
        };
        
        public static readonly Dictionary<string, Length> lengthStrings = new Dictionary<string, Length>()
        {
            {"1", Length.Whole}, {"1/1", Length.Whole}, {"1/2", Length.Half},
            {"1/4", Length.Quarter}, {"1/8", Length.Eighth}, {"1/16", Length.Sixteenth}
        };
        
        public static readonly HashSet<char> barCharacters = new HashSet<char>() {'|', '[', ']'};
        public static readonly HashSet<char> tuneHeaderInfoCharacters = new HashSet<char>() { 'X', 'T', 'C' };

        public static readonly Dictionary<string, Bar.Kind> standardBarTypes = new Dictionary<string, Bar.Kind>()
        {
            {"|", Bar.Kind.Line}, {"||", Bar.Kind.DoubleLine}, 
            {"[|", Bar.Kind.Start}, {"|]", Bar.Kind.Final},
            {"|:", Bar.Kind.RepeatStart}, {":|", Bar.Kind.RepeatEnd}, 
            {":|:", Bar.Kind.RepeatEndStart}, {":||:", Bar.Kind.RepeatEndStart}
        };
        
        public static bool IsStartOfBarItem(char ch)
        {
            return barCharacters.Contains(ch) || ch == ':';
        }

        public static bool IsStartOfNoteStream(int val)
        {
            return notes.ContainsKey(val) || accidentals.ContainsKey(val);
        }

        public static int GetNoteOffset(int val)
        {
            if (notes.TryGetValue(val, out int offset))
                return offset;
            else
                throw new ParseException("Invalid note value");
        }

        public static Accidental GetAccidental(int val)
        {
            Accidental accidental = Accidental.Unspecified;
            accidentals.TryGetValue(val, out accidental);
            return accidental;
        }
    }
}
