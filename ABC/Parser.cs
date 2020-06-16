using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ABC
{
    class Parser
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private Tune tune;

        private Voice voice;

        private Length unitNoteLength = Length.Eighth;
        private string timeSignature;

        StreamReader reader;

        int lineNum = 0;
        int index = 0;
        string currentLine;
        private bool parsingTuneBody = false;
        private bool parsingInlineinformationField = false;

        private enum BrokenRhythm { None, DotHalf, HalfDot }

        private BrokenRhythm brokenRhythm = BrokenRhythm.None;
        private int brokenRhythmCount = 0;

        bool beam = false;
        int beamId = 0;

        private enum LineBreakSymbol { None, EOL, DollarSign };
        private List<LineBreakSymbol> lineBreakSymbols = new List<LineBreakSymbol>() { LineBreakSymbol.EOL, LineBreakSymbol.DollarSign };
        Dictionary<string, bool> lineBreaksNeeded = new Dictionary<string, bool>();

        private List<string> decorations = null;

        public Tune Parse(Stream stream)
        {
            reader = new StreamReader(stream);
            tune = new Tune();

            while (reader.Peek() >= 0)
            {
                currentLine = reader.ReadLine();
                lineNum += 1;
                index = 0;
                beam = false;

                if (SkipWhiteSpace()) continue;

                if (IsStartOfInformationField(index))
                    ParseInformationField();
                else
                    ParseTuneBody();
            }

            return tune;
        }

        private static string defaultVoiceIdentifier = "__default__";
        
        void EnsureVoice()
        {
            if (voice != null) return;
            
            voice = new Voice(defaultVoiceIdentifier);
            voice.initialTimeSignature = timeSignature;
            lineBreaksNeeded[voice.identifier] = false;

            tune.voices.Add(voice);
        }

        bool SkipWhiteSpace()
        {
            int startIndex = index;

            while (index < currentLine.Length && Char.IsWhiteSpace(currentLine[index]))
                index += 1;

            if (index != startIndex) beam = false;

            return index == currentLine.Length;
        }


        /// <summary>
        /// Reads the current line until the supplied condition is returned true
        /// </summary>
        /// <param name="condition">function to be run on each character.  If this function returns false processing will cease.</param>
        /// <param name="result">will receive a substring of the current line consisting of the characters read.  if nothing is read will be set to empty string</param>
        /// <returns>boolean value indicating whether the end of the line was reached</returns>
        bool ReadUntil(Func<char, bool> condition, out string result)
        {
            int start = index;
            int length = 0;
            while (index + length < currentLine.Length && !condition(currentLine[index + length]))
                length += 1;

            if (length > 0)
                result = currentLine.Substring(start, length);
            else
                result = string.Empty;

            index += length;

            return index == currentLine.Length;
        }

        void ParseTuneBodyInformationField()
        {
            index += 1; // [
            bool eol = ReadUntil((char c) => { return c == ']'; }, out string inlineInformationField);
            if (eol)
                throw new ParseException($"Unterminated information field at: {lineNum}, {index - inlineInformationField.Length}");

            index += 1; // ] 

            var savedCurrentLine = currentLine;
            var savedIndex = index;

            currentLine = inlineInformationField;
            index = 0;

            parsingInlineinformationField = true;
            ParseInformationField();
            parsingInlineinformationField = false;

            currentLine = savedCurrentLine;
            index = savedIndex;
        }

        static readonly List<char> thinBarChars = new List<char>() { '|', ']'};
        static readonly List<char> thickBarChars = new List<char>() { '[', '|' };

        void ParseBar()
        {
            EnsureVoice();
            CheckForLineBreak();

            Bar.Kind kind;
            int endRepeatCount = 0, startRepeatCount = 0;
            string repeatStr;

            if (currentLine[index] == ':')
            {
                ReadUntil((char c) => { return c != ':'; }, out repeatStr);
                endRepeatCount = repeatStr.Length;
            }

            string barStr = new string(currentLine[index], 1);
            index += 1;

            if (index < currentLine.Length)
            {
                List<char> characters = barStr[0] == '|' ? thinBarChars : thickBarChars;
                if (characters.Contains(currentLine[index]))
                {
                    barStr += currentLine[index];
                    index += 1;
                }
            }

            ReadUntil((char c) => { return c != ':'; }, out repeatStr);
            startRepeatCount = repeatStr.Length;
            
            switch (barStr)
            {
                case "|":
                    kind = Bar.Kind.Line;
                    break;
            
                case "||":
                    kind = Bar.Kind.ThinThinDoubleBar;
                    break;
            
                case "[|":
                    kind = Bar.Kind.ThickThinDoubleBar;
                    break;
            
                case "|]":
                    kind = Bar.Kind.ThinThickDoubleBar;
                    break;
            
                default:
                    throw new ParseException($"Unsupported Bar specification at {lineNum}, {index}");
            }

            var bar = new Bar(kind);
            bar.startRepeatCount = startRepeatCount;
            bar.endRepeatCount = endRepeatCount;
            
            SetDecorationsForItem(bar);
            voice.items.Add(bar);
            beam = false;
        }

        void ParseTuneBody()
        {
            parsingTuneBody = true;
            
            while (index < currentLine.Length)
            {
                if (SkipWhiteSpace()) return;

                if (currentLine[index] == '[')
                {
                    if (Elements.IsStartOfNoteStream(currentLine[index + 1]))
                        ParseChord();
                    else if (IsStartOfInformationField(index + 1))
                        ParseTuneBodyInformationField();
                    else
                        ParseBar();
                }
                else if (Elements.IsStartOfBarItem(currentLine[index]))
                {
                    ParseBar();
                }
                else if (currentLine[index] == '>' || currentLine[index] == '<')
                {
                    ParseBrokenRhythm();
                    continue;
                }
                else if (currentLine[index] == '!')
                {
                    ParseDecorations();
                    continue;
                }
                else if (Elements.IsStartOfNoteStream(currentLine[index]))
                {
                    EnsureVoice();
                    var note = ReadFullNote();
                    UpdateBeam(note);

                    voice.items.Add(note);
                }
                else if (Elements.rests.Contains(currentLine[index]))
                {
                    EnsureVoice();
                    ReadRest();
                    beam = false;
                }
                else
                {
                    throw new ParseException($"Unexpected character: {currentLine[index]} at {lineNum}, {index}");
                }

                UpdateBrokenRhythm();
                EvaluateLineBreak();

                if (decorations != null)
                    throw new ParseException($"Invalid decoration near {lineNum}, {index}");
            }


        }

        void EvaluateLineBreak()
        {
            if (voice == null)
                return;

            if (index == currentLine.Length)
            {
                lineBreaksNeeded[voice.identifier] |= lineBreakSymbols.Contains(LineBreakSymbol.EOL);
            }
            else if (currentLine[index] == '$')
            {
                index += 1;

                if (lineBreakSymbols.Contains(LineBreakSymbol.DollarSign))
                    lineBreaksNeeded[voice.identifier] = true;
            }
        }

        void ParseDecorations()
        {
            decorations = new List<string>();
            
            while (currentLine[index] == '!')
            {
                index += 1;
                bool eol = ReadUntil((char c) => { return c == '!'; }, out string decoration);
                
                if (eol)
                    throw new ParseException($"Unterminated decoration near: {lineNum}, {index}");
                
                decorations.Add(decoration);

                index += 1;
            }
        }

        void ParseBrokenRhythm()
        {
            if (voice == null || voice.items.Count == 0)
                throw new ParseException($"Illegal broken Rhythm marker at {lineNum}, {index}");

            char symbol = currentLine[index];
            brokenRhythm = symbol == '>' ? BrokenRhythm.DotHalf : BrokenRhythm.HalfDot;
            
            ReadUntil((char ch) => { return ch != symbol;}, out string brokenRhythmStr);
            brokenRhythmCount = brokenRhythmStr.Length;
        }

        void UpdateBrokenRhythm()
        {
            if (brokenRhythm == BrokenRhythm.None)
                return;
            
            if (voice.items.Count < 2)
                throw new ParseException($"Unable to apply broken rhythm to item near {lineNum}, {index}");

            var itemA = voice.items[voice.items.Count - 2] as Duration;
            var itemB = voice.items[voice.items.Count - 1] as Duration;
            
            if (itemA == null || itemB == null)
                throw new ParseException($"Unable to apply broken rhythm to item near {lineNum}, {index}");

            Duration halfItem, dotItem;
            if (brokenRhythm == BrokenRhythm.DotHalf)
            {
                dotItem = itemA;
                halfItem = itemB;
            }
            else
            {
                dotItem = itemB;
                halfItem = itemA;
            }
            
            dotItem.dotCount = brokenRhythmCount;

            var duration = ParserUtil.lengthDurations[halfItem.length] * (1.0f / (1 << brokenRhythmCount));
            ParserUtil.ParseDuration(duration, out Length l, out int dots);
            halfItem.length = l;
            halfItem.dotCount = dots;

            brokenRhythm = BrokenRhythm.None;
        }

        void UpdateBeam(Duration item)
        {
            if (item.length <= Length.Eighth)
            {
                if (!beam) // potentially start a new beam
                {
                    beam = true;
                    beamId += 1;
                }
                else
                {
                    // if the previous note has the same value as this one then we can beam it
                    var previousItem = voice.items[voice.items.Count - 1] as Duration;
                    if (previousItem != null && previousItem.length == item.length)
                    {
                        previousItem.beam = beamId;
                        item.beam = beamId;
                    }
                    else
                    {
                        beam = false;
                    }
                }
            }
            else
            {
                beam = false;
            }
        }

        void CheckForLineBreak()
        {
            if (voice == null)
                return;

            bool lineBreakNeeded = false;
            lineBreaksNeeded.TryGetValue(voice.identifier, out lineBreakNeeded);

            if (lineBreakNeeded)
            {
                voice.items.Add(new LineBreak());
                lineBreaksNeeded[voice.identifier] = false;
            }
        }

        void ParseChord()
        {
            EnsureVoice();

            CheckForLineBreak();

            index += 1;
            var notes = new List<Note>();
            float noteDurationModifier = 1.0f;

            do
            {
                if (SkipWhiteSpace())
                    throw new ParseException($"Unterminated chord at {lineNum}, {index}");

                if (!Elements.IsStartOfNoteStream(currentLine[index]))
                    throw new ParseException($"Invalid character in chord at {lineNum}, {index}");
                
                notes.Add(ReadBaseNote());
                
                //only the duration modifier for the first chord will have impact on its overall duration
                float durationModifier = ReadDurationModifier();
                if (notes.Count == 1)
                    noteDurationModifier = durationModifier;

                if (index == currentLine.Length)
                    throw new ParseException($"Unterminated chord at {lineNum}, {index}");

            } while (currentLine[index] != ']');

            index += 1;

            if (notes.Count == 0)
                throw new ParseException($"Encountered empty chord at {lineNum}, {index}");

            var chord = Chord.FromNotes(notes);
            float chordDurationModifier = ReadDurationModifier();
            // spec states that length modifiers inside the chord are multiplied by outside
            float noteDuration = ParserUtil.lengthDurations[unitNoteLength] * (noteDurationModifier * chordDurationModifier);
            
            if (!ParserUtil.ParseDuration(noteDuration, out Length chordLength, out int dotCount))
                throw new ParseException($"Invalid Note duration at {lineNum}, {index}");

            chord.length = chordLength;
            chord.dotCount = dotCount;

            UpdateBeam(chord);
            voice.items.Add(chord);
            SetDecorationsForItem(chord);
        }

        void ReadRest()
        {
            CheckForLineBreak();

            if (char.IsLower(currentLine[index]))
            {
                var rest = new Rest();
                rest.isVisible = currentLine[index] == 'z';
                index += 1;
                ParseLengthValues(rest);
                SetDecorationsForItem(rest);
                voice.items.Add(rest);
            }
            else
            {
                var rest = new MultiMeasureRest();
                rest.isVisible = currentLine[index] == 'Z';
                index += 1;
                ReadUntil((char c) => { return !char.IsDigit(c); }, out string measureCountStr);

                if (measureCountStr.Length > 0)
                {
                    int measureCount = 0;
                    if (int.TryParse(measureCountStr, out measureCount))
                        rest.count = measureCount;
                    else
                        throw new ParseException($"Unable to parse multi measure rest count at {lineNum}, {index - measureCountStr.Length}");

                }
                else
                {
                    rest.count = 1;
                }    
                
                voice.items.Add(rest);
            }
        }

        private Note ReadBaseNote()
        {
            var note = new Note();

            note.accidental = Elements.GetAccidental(currentLine[index]);
            
            if (note.accidental != Accidental.Unspecified)
                index += 1;

            if (index == currentLine.Length)
                throw new ParseException("Invalid note specification");

            // start with Middle C aka 'C'
            int noteValue = (int) Pitch.C4;

            try
            {
                // adjust the value based on the actual note value
                noteValue += Elements.GetNoteOffset(currentLine[index]);
                index += 1;
            }
            catch (ParseException e)
            {
                throw new ParseException($"{e.Message} at {lineNum}, {index}");
            }

            if (index < currentLine.Length)
            {
                // apply any octave modifiers attached to the note
                while (index < currentLine.Length && Elements.octaveModifiers.TryGetValue(currentLine[index], out int modifier))
                {
                    noteValue += modifier;
                    index += 1;
                }
            }

            // ensure final note value is valid
            if (noteValue < (int)Pitch.A0 || noteValue > (int)Pitch.C8)
                throw new ParseException("Invalid note value");

            note.pitch = (Pitch)noteValue;

            return note;
        }

        Note ReadFullNote()
        {
            CheckForLineBreak();
            var note = ReadBaseNote();
            ParseLengthValues(note);
            SetDecorationsForItem(note);

            return note;
        }

        private void SetDecorationsForItem(Item item)
        {
            if (decorations != null)
            {
                tune.decorations[item.id] = decorations;
                decorations = null;
            }
        }

        void ParseLengthValues(Duration duration)
        {
            Length l;
            int dots;
            ParseLength(out l, out dots);
            duration.length = l;
            duration.dotCount = dots;
        }

        float ReadDurationModifier()
        {
            // length modifier will be either '/*' or '\d*/N'
            ReadUntil((char c) => { return !char.IsDigit(c) && c != '/'; }, out string lengthMod);

            try
            {
                return ParserUtil.ParseDurationModifierString(lengthMod);
            }
            catch (FormatException)
            {
                throw new ParseException($"Unable to parse length modifier at {lineNum}, {index}");
            }
        }

        void ParseLength(out Length length, out int dots)
        {
            float noteDuration = ParserUtil.lengthDurations[unitNoteLength];
            noteDuration *= ReadDurationModifier();
            
            if (!ParserUtil.ParseDuration(noteDuration, out length, out dots))
                throw new ParseException($"Invalid Note duration at {lineNum}, {index}");
        }

        void ParseInformationField()
        {
            switch (currentLine[index])
            {
                case 'X':
                    ParseReferenceNumber();
                    break;

                case 'T':
                    ParseTitle();
                    break;

                case 'V':
                    ParseVoiceHeader();
                    break;

                case 'L':
                    ParseUnitNoteLengthInformation();
                    break;
                
                case 'M':
                    ParseTimeSignature();
                    break;
                
                case 'K':
                    ParseKeySignature();
                    break;

                case 'I':
                    ParseInstruction();
                    break;
            }
        }

        void ParseKeySignature()
        {
            index += 2;
            bool eol = SkipWhiteSpace();

            if (eol)
            {
                AddKeySignature(KeySignature.None);
                return;
            }

            if (currentLine.Substring(index).Trim().ToLower() == "none")
            {
                AddKeySignature(KeySignature.None);
                return;
            }

            // get the key name
            string keyName = currentLine.Substring(index++, 1);
            string keyMode = "Major"; // default per spec
            string line = string.Empty;

            // check for a sharp or flat
            if (index < currentLine.Length)
            {
                if (currentLine[index] == '#')
                {
                    keyName += "Sharp";
                    index += 1;
                }
                else if (currentLine[index] == 'b')
                {
                    keyName += "Flat";
                    index += 1;
                }
            }

            // rest of the line ignores whitespace and is case insensitive
            if (index < currentLine.Length)
            {
                for (int i = index; i < currentLine.Length; i++)
                {
                    if (char.IsWhiteSpace(currentLine[i])) continue;
                    line += char.ToLower(currentLine[i]);
                }

                // check the mode.
                if (line.Length > 3)
                    line = line.Substring(0, 3);

                if (line.Length > 0)
                {
                    if (line == "maj")
                        keyMode = "Major";
                    else if (line == "m" || line == "min")
                        keyMode = "Minor";
                    else
                        throw new ParseException($"Unexpected Key Signature mode: {line} near {lineNum}, {index}");
                }
            }
            
            keyName += keyMode;
            index = currentLine.Length;

            try
            {
                var keySignature = (KeySignature) Enum.Parse(typeof(KeySignature), keyName);
                AddKeySignature(keySignature);
            }
            catch (ArgumentException)
            {
                throw new ParseException($"Invalid Key Signature value: {keyName} near {lineNum}, {index}");
            }
        }

        void AddKeySignature(KeySignature keySignature)
        {
            EnsureVoice();

            if (parsingInlineinformationField)
            {
                voice.items.Add(new Key(keySignature));
            }
            else
            {
                foreach (var voice in tune.voices)
                {
                    if (voice.items.Count == 0)
                        voice.initialKey = keySignature;
                    else
                        voice.items.Add(new Key(keySignature));
                }
            }
        }

        void ParseTimeSignature()
        {
            index += 2;
            string timeSignatureStr = currentLine.Substring(index).Trim();

            if (timeSignatureStr.ToLower() == "none")
                timeSignatureStr = string.Empty;

            if (parsingInlineinformationField)
            {
                voice.items.Add(new TimeSignature(timeSignatureStr));
            }
            else
            {
                timeSignature = timeSignatureStr;
                foreach (var v in tune.voices)
                {
                    if (v.items.Count > 0)
                        v.items.Add(new TimeSignature(timeSignature));
                    else
                        v.initialTimeSignature = timeSignature;
                }
            }
        }

        void ParseTitle()
        {
            if (parsingTuneBody)
                throw new ParseException($"Title should not be set in tune body at {lineNum}, {index}");
            
            index += 2;
            tune.title = currentLine.Substring(index);
        }

        void ParseUnitNoteLengthInformation()
        {
            index += 2;

            if (SkipWhiteSpace())
                throw new ParseException($"Unterminated Note Length field at {lineNum}, {index}");

            var lengthInfo = currentLine.Substring(index).TrimEnd();
            if (!Elements.lengthStrings.TryGetValue(lengthInfo, out Length length))
                throw new ParseException($"Unrecognized note length: {lengthInfo} at {lineNum}, {index - lengthInfo.Length}");

            unitNoteLength = length;
        }

        void ParseLinebreakInstruction()
        {
            if (parsingTuneBody)
                throw new ParseException("Line break instructions are not allowed when parsing tune body");

            lineBreakSymbols.Clear();
            bool eol = SkipWhiteSpace();
            
            while (!eol)
            {
                eol = ReadUntil((char c) => { return char.IsWhiteSpace(c); }, out var symbolStr);

                switch(symbolStr)
                {
                    case "<EOL>":
                        lineBreakSymbols.Add(LineBreakSymbol.EOL);
                        break;

                    case "<none>":
                        lineBreakSymbols.Add(LineBreakSymbol.None);
                        break;

                    case "$":
                        lineBreakSymbols.Add(LineBreakSymbol.DollarSign);
                        break;

                    default:
                        throw new ParseException($"Unsupported Line Break Symbol: {symbolStr}");
                }

                eol = SkipWhiteSpace();
            }
        }

        void ParseInstruction()
        {
            index += 2;
            SkipWhiteSpace();

            ReadUntil((char c) => { return char.IsWhiteSpace(c); }, out var instruction);

            instruction = instruction.ToLower();
            switch (instruction)
            {
                case "linebreak":
                    ParseLinebreakInstruction();
                    break;
                default:
                    throw new ParseException($"Unsupported instruction {instruction} at {lineNum}, {index}");
            }
        }

        void ParseReferenceNumber()
        {
            string referenceNumberStr = currentLine.Substring(index + 2);
            if (uint.TryParse(referenceNumberStr, out uint referenceNumber))
                tune.referenceNumber = referenceNumber;
            else
                throw new ParseException($"Error Parsing Reference number: {referenceNumberStr} at {lineNum},{index + 2}");
        }

        private bool ReadModifierValue(out string result)
        {
            if (currentLine[index] == '"')
            {
                index += 1;
                bool eol = ReadUntil((char c) => { return c == '"';}, out result);
                index += 1;

                return eol;
            }
            
            return ReadUntil((char c) => { return char.IsWhiteSpace(c); }, out result);
        }
        
        void ParseVoiceHeader()
        {
            index += 2;

            //read voice identifier
            SkipWhiteSpace();
            bool eol = ReadUntil((char c) => { return char.IsWhiteSpace(c); }, out string identifier);

            voice = tune.FindVoice(identifier);

            if (voice == null)
            {
                voice = new Voice(identifier);
                voice.initialTimeSignature = timeSignature;
                lineBreaksNeeded[voice.identifier] = false;

                tune.voices.Add(voice);
            }
            
            if(SkipWhiteSpace()) return;

            while (!eol)
            {
                eol = ReadUntil((char c) => { return char.IsWhiteSpace(c) || c == '='; }, out string key);
                if (eol)
                    throw new ParseException($"Unterminated Modifier at {lineNum}, {index}");
                
                SkipWhiteSpace();
                
                if (currentLine[index] != '=')
                    throw new ParseException($"Unexpected Character {currentLine[index]} at {lineNum}, {index}");
                
                index += 1;
                
                SkipWhiteSpace();

                ReadModifierValue(out string value);

                switch (key)
                {
                    case "clef":
                        var clefString = textInfo.ToTitleCase(value);

                        try
                        {
                            voice.clef = (Clef) Enum.Parse(typeof(Clef), clefString);
                        }
                        catch (ArgumentException)
                        {
                            throw new ParseException($"Invalid clef value {value} at {lineNum}, {index - value.Length}");
                        }
                        
                        break;
                    
                    case "name":
                        voice.name = value;
                        break;
                }

                eol = SkipWhiteSpace();
            }
        }

        bool IsStartOfInformationField(int pos)
        {
            if (pos > currentLine.Length - 2)
                return false;

            char ch = Char.ToLower(currentLine[pos]);

            if (ch < 'a' || ch > 'z')
                return false;

            if (currentLine[pos + 1] != ':')
                return false;

            return true;
        }
    }
}
