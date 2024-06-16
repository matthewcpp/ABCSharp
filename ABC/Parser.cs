using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

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

        private enum LineBreakSymbol { None, EOL, DollarSign };
        private List<LineBreakSymbol> lineBreakSymbols = new List<LineBreakSymbol>() { LineBreakSymbol.EOL, LineBreakSymbol.DollarSign };
        Dictionary<string, bool> lineBreaksNeeded = new Dictionary<string, bool>();

        class VoiceParseContext 
        {
            public class SlurInfo 
            {
                public int lineNum { get; set; }
                public int linePos { get; set; }
                public int itemIndex { get; set; }
            }

            public class BeamInfo
            {
                public Duration startItem = null;
                public Beam current = null;

                public void Clear()
                {
                    startItem = null;
                    current = null;
                }
            }

            public List<SlurInfo> slurs = new List<SlurInfo>();
            public BeamInfo beam = new BeamInfo();
            public int? tieStartIndex = null;
        }

        Dictionary<Voice, VoiceParseContext> voiceParseContexts = new Dictionary<Voice, VoiceParseContext>();

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
                ClearCurrentBeam();

                if (SkipWhiteSpace()) continue;

                if (IsStartOfInformationField(index))
                    ParseInformationField();
                else
                    ParseTuneBody();
            }

            FinalizaeSlursAndTies();

            return tune;
        }

        private static string defaultVoiceIdentifier = "__default__";

        private void FinalizaeSlursAndTies()
        {
            foreach(var parseContext in voiceParseContexts.Values)
            {
                if (parseContext.slurs.Count > 0) {
                    var slurInfo = parseContext.slurs[parseContext.slurs.Count - 1];
                    throw new ParseException($"Unterminated slur at: {slurInfo.lineNum}, {slurInfo.linePos}");
                }
            }

            foreach (var voice in tune.voices)
            {
                voice.slurs.Sort();
                voice.ties.Sort();
            }
        }
        
        void EnsureVoice()
        {
            if (voice != null) return;
            
            voice = new Voice(defaultVoiceIdentifier);
            voiceParseContexts[voice] = new VoiceParseContext();
            voice.initialTimeSignature = timeSignature;
            lineBreaksNeeded[voice.identifier] = false;

            tune.voices.Add(voice);
        }

        bool SkipWhiteSpace()
        {
            int startIndex = index;

            while (index < currentLine.Length && Char.IsWhiteSpace(currentLine[index])) {
                index += 1;
            }

            if (index != startIndex) {
                ClearCurrentBeam();
            }

            return index == currentLine.Length;
        }

        int ConsumeUntil(Func<char, bool> condition)
        {
            int length = 0;
            while (index + length < currentLine.Length && !condition(currentLine[index + length]))
                length += 1;

            index += length;

            return length;
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
            int length = ConsumeUntil(condition);

            if (length > 0)
                result = currentLine.Substring(start, length);
            else
                result = string.Empty;


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

        bool ReadBarCharacters(out string str)
        {
            string result = string.Empty;
            while (true)
            {
                if (index >= currentLine.Length)
                    break;

                char ch = currentLine[index];

                if (!Elements.barCharacters.Contains(ch))
                    break;

                // handle case where custom bar touches chord eg '|[[CEG]'
                if (ch == '[' && index < currentLine.Length - 1 && Elements.IsStartOfNoteStream(currentLine[index + 1]))
                    break;
                
                result += ch;
                index += 1;
            }

            str = result;
            return index >= currentLine.Length;
        }

        void ParseBar()
        {
            EnsureVoice();
            CheckForLineBreak();
            
            ReadUntil((char c) => c != ':', out var firstRepeatCharStr);
            ReadBarCharacters(out var barCharStr);
            ReadUntil((char c) => c != ':', out var secondCharStr);
            string barString = firstRepeatCharStr + barCharStr + secondCharStr;

            Item barItem;
            try
            {
                barItem = new Bar(Elements.standardBarTypes[barString]);
            }
            catch (Exception)
            {
                barItem = new CustomBar(barString);
            }

            SetDecorationsForItem(barItem);
            voice.items.Add(barItem);
            ClearCurrentBeam();
        }

        private void ParseSlurStart()
        {
            EnsureVoice();
            var parseContext = voiceParseContexts[voice];

            // We check for linebreak here because if a slur starts at the beginning of a line
            // that has a bending break, we want to make sure the item index we store in the
            // Slur info will correspond to the next note that we parese.
            CheckForLineBreak();

            parseContext.slurs.Add(new VoiceParseContext.SlurInfo()
            {
                lineNum = this.lineNum,
                linePos = index,
                itemIndex = voice.items.Count
            });

            index += 1;
        }

        private void ParseSlurEnd() {
            var parseContext = voiceParseContexts[voice];

            if (parseContext.slurs.Count == 0)
            {
                throw new ParseException($"Mismatched ')' at: {lineNum}, {index}");
            }

            var slurStart = parseContext.slurs[parseContext.slurs.Count - 1];
            var slurEndIndex = voice.items.Count - 1;

            // abc spec 4.11 slur can start and end on the same note
            // in this case find the previous slur start and use that
            if (slurEndIndex == slurStart.itemIndex) {
                if (parseContext.slurs.Count < 2) {
                    throw new ParseException($"Mismatched ')' at: {lineNum}, {index}");
                }

                slurStart = parseContext.slurs[parseContext.slurs.Count - 2];
                parseContext.slurs.RemoveAt(parseContext.slurs.Count - 2);
            }
            else {
                parseContext.slurs.RemoveAt(parseContext.slurs.Count - 1);
            }

            var startItem = voice.items[slurStart.itemIndex];
            var endItem = voice.items[slurEndIndex];
            voice.slurs.Add(new Slur(startItem.id, endItem.id));

            index += 1;
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
                    CheckTieStatus();
                }
                else if (Elements.rests.Contains(currentLine[index]))
                {
                    EnsureVoice();
                    ReadRest();
                    ClearCurrentBeam();
                }
                else if (currentLine[index] == '(')
                {
                    ParseSlurStart();
                    continue;
                }
                else if (currentLine[index] == ')') {
                    ParseSlurEnd();
                    continue;
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
            ConsumeUntil((char c) => { return c != '`'; });
            var parseContext = voiceParseContexts[voice];

            if (item.length <= Length.Eighth)
            {
                // Just parsed a chord, potentially start a new beam
                if (parseContext.beam.startItem == null) 
                {
                    parseContext.beam.startItem = item;
                    return;
                }

                if (parseContext.beam.current == null)
                {
                    var beam =  new Beam(voice, parseContext.beam.startItem.id, item.id);
                    parseContext.beam.current = beam;
                    voice.beams.Add(beam);

                    parseContext.beam.startItem.beam = beam;
                    item.beam = beam;
                }
                else
                {
                    parseContext.beam.current.endId = item.id;
                    item.beam = parseContext.beam.current;
                }
            }
            else
            {
                ClearCurrentBeam();
            }
        }

        void ClearCurrentBeam()
        {
            if (voice == null) {
                return;
            }

            var parseContext = voiceParseContexts[voice];
            parseContext.beam.Clear();
        }

        void CheckForLineBreak()
        {
            if (voice == null)
                return;

            lineBreaksNeeded.TryGetValue(voice.identifier, out bool lineBreakNeeded);

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
            CheckTieStatus();
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
                ReadUntil((char c) => !char.IsDigit(c), out string measureCountStr);

                if (measureCountStr.Length > 0)
                {
                    if (int.TryParse(measureCountStr, out int measureCount))
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
        
        /// <summary> This should only be called right after a note has been parsed</summary>
        void CheckTieStatus()
        {
            var parseContext = voiceParseContexts[voice];

            if (parseContext.tieStartIndex.HasValue)
            {
                var startItem = voice.items[parseContext.tieStartIndex.Value];
                var endItem = voice.items[voice.items.Count - 1];
                voice.ties.Add(new Tie(startItem.id, endItem.id));
                parseContext.tieStartIndex = null;
            }

            // tie operator '-' must be attached to the end of a note.
            if (index < currentLine.Length && currentLine[index] == '-')
            {
                parseContext.tieStartIndex = voice.items.Count - 1;
                index += 1;
            }
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
            if (Elements.tuneHeaderInfoCharacters.Contains(currentLine[index]))
            {
                ParseTuneHeaderInfo();
                return;
            }

            switch (currentLine[index])
            {
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

        void ParseTuneHeaderInfo()
        {
            var headerField = currentLine[index];

            if (parsingTuneBody)
                throw new ParseException($"Cannot Set header info field '{headerField}' in tune body at {lineNum}, {index}");

            index += 2;
            var headerValue = currentLine.Substring(index).Trim();
            switch (headerField)
            {
                case 'X':
                    if (uint.TryParse(headerValue, out uint referenceNumber))
                        tune.header.referenceNumber = referenceNumber.ToString();
                    else
                        throw new ParseException($"Error Parsing Reference number: {headerValue} at {lineNum},{index + 2}");
                    break;
                case 'T':
                    tune.header.title = headerValue;
                    break;

                case 'C':
                    tune.header.composer = headerValue;
                    break;
            }
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
                voiceParseContexts[voice] = new VoiceParseContext();
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
