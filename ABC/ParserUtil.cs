using System;
using System.Collections.Generic;

namespace ABC
{
    static class ParserUtil
    {
        public static readonly Dictionary<Length, float> lengthDurations = new Dictionary<Length, float>()
        {
            {Length.Whole, 1.0f}, {Length.Half, 0.5f}, {Length.Quarter, 0.25f}, {Length.Eighth,0.125f}, {Length.Sixteenth, 0.0625f}
        };

        static bool IsNoteMultiplication(string modifierString)
        {
            foreach (char c in modifierString)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        static bool IsShorthandDivision(string modifierString)
        {
            foreach (char c in modifierString)
            {
                if (c != '/')
                    return false;
            }

            return true;
        }

        public static float ParseDurationModifierString(string modifierString)
        {
            if (modifierString == null || modifierString.Length == 0)
                return 1.0f;


            // simple scale of note length: i.e. C2
            if (IsNoteMultiplication(modifierString))
                return float.Parse(modifierString);


            //shorthand division of note length i.e. C//
            if (IsShorthandDivision(modifierString))
                return 1.0f / (1 << modifierString.Length);

            var parts = modifierString.Split('/');
            if (parts.Length == 2)
            {
                float numerator = 1.0f;
                if (parts[0].Length > 0)
                    numerator = float.Parse(parts[0]);

                return numerator / float.Parse(parts[1]);
            }

            throw new FormatException();
        }

        public static bool ParseDuration(float inDuration, out Length length, out int dots)
        {
            length = Length.Unknown;
            dots = 0;

            float lengthValue = 0.0f;

            // figure out what note type the value will be
            for (Length noteLength = Length.Whole; noteLength >= Length.Sixteenth; noteLength--)
            {
                float duration = lengthDurations[noteLength];
                if (inDuration >= duration)
                {
                    length = noteLength;
                    lengthValue = duration;
                    inDuration -= lengthValue;
                    break;
                }
            }

            if (length == Length.Unknown)
                return false;

            float dotValue = lengthValue * 0.5f;
            while (inDuration >= dotValue)
            {
                dots += 1;
                inDuration -= dotValue;
                dotValue *= 0.5f;
            }

            return true;
        }
    }
}