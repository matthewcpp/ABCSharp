using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ABC
{
    class Parser
    {
        TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
        private Tune tune;

        private Voice voice;

        private Note.Length unitNoteLength = Note.Length.Eighth;
        private string timeSignature;

        StreamReader reader;

        int lineNum = 0;
        int index = 0;
        string currentLine;
        private bool parsingTuneBody = false;

        public Tune Parse(Stream stream)
        {
            reader = new StreamReader(stream);
            tune = new Tune();

            while (reader.Peek() >= 0)
            {
                currentLine = reader.ReadLine();
                lineNum += 1;
                index = 0;

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
            
            if (timeSignature != null)
                voice.items.Add(new TimeSignatureItem(timeSignature));
            
            tune.voices.Add(voice);
        }

        bool SkipWhiteSpace()
        {
            while (index < currentLine.Length && Char.IsWhiteSpace(currentLine[index]))
                index += 1;

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

            ParseInformationField();

            currentLine = savedCurrentLine;
            index = savedIndex;
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
                        throw new ParseException($"Unexpected character: {currentLine[index]} at {lineNum}, {index}");
                }
                else if (currentLine[index] == '|')
                {
                    EnsureVoice();
                    voice.items.Add(new BarItem(new Bar(Bar.Type.Line)));
                    index += 1;
                }
                else if (Elements.IsStartOfNoteStream(currentLine[index]))
                {
                    EnsureVoice();
                    var note = ReadNote();
                    voice.items.Add(new NoteItem(note));
                }
                else
                {
                    throw new ParseException($"Unexpected character: {currentLine[index]} at {lineNum}, {index}");
                }
            }
        }

        void ParseChord()
        {
            EnsureVoice();
            
            index += 1;
            var notes = new List<Note>();

            do
            {
                if (!Elements.IsStartOfNoteStream(currentLine[index]))
                    throw new ParseException($"Invalid character in chord at {lineNum}, {index}");

                notes.Add(ReadNote());

                if (index == currentLine.Length)
                    throw new ParseException($"Unterminated chord at {lineNum}, {index}");

            } while (currentLine[index] != ']');

            index += 1;

            if (notes.Count == 0)
                throw new ParseException($"Encountered empty chord at {lineNum}, {index}");

            voice.items.Add(new ChordItem(notes));
        }

        Note ReadNote()
        {
            var note = new Note();

            note.accidental = Elements.GetAccidental(currentLine[index]);
            
            if (note.accidental != Note.Accidental.Unspecified)
                index += 1;

            if (index == currentLine.Length)
                throw new ParseException("Invalid note specification");

            // start with Middle C aka 'C'
            int noteValue = (int) Note.Value.C4;

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
            if (noteValue < (int)Note.Value.A0 || noteValue > (int)Note.Value.C8)
                throw new ParseException("Invalid note value");

            note.value = (Note.Value)noteValue;
            note.length = ParseNoteLengthModifier();

            return note;
        }

        Note.Length ParseNoteLengthModifier()
        {
            // length modifier will be either '/*' or '/N'
            ReadUntil((char c) => { return !char.IsDigit(c) && c != '/'; }, out string lengthMod);

            if (lengthMod.Length == 0) // no modifier specified
                return unitNoteLength;

            int lengthValue = (int)unitNoteLength;

            if (char.IsDigit(lengthMod[0]))  // ex: A2
            {
                if (!int.TryParse(lengthMod, out int value))
                    throw new ParseException($"Invalid note length modifier {lengthMod} at {lineNum},{index}");

                lengthValue = (int)unitNoteLength / value;
            }
            else if (lengthMod.Length > 1 && char.IsDigit(lengthMod[1])) //ex: A/2  
            {
                if (!int.TryParse(lengthMod.Substring(1), out int value))
                    throw new ParseException($"Invalid note length modifier {lengthMod} at {lineNum},{index}");

                lengthValue = (int)unitNoteLength * value;
            }
            else // ex: A//
            {
                for (int i = 0; i < lengthMod.Length; i++) // all characters should be '/'
                {
                    if (lengthMod[i] != '/')
                        throw new ParseException($"Invalid note length modifier {lengthMod} at {lineNum},{index}");
                }

                lengthValue = (int)unitNoteLength * (1 << lengthMod.Length);
            }

            // ensure that final calculated value is a supported enumeration value
            if (Enum.IsDefined(typeof(Note.Length), lengthValue))
                return (Note.Length)Enum.ToObject(typeof(Note.Length), lengthValue);
            else
                throw new ParseException($"Note length modifier resulted in invalid note length  at {lineNum},{index}");
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
            }
        }

        void ParseTimeSignature()
        {
            index += 2;
            timeSignature = currentLine.Substring(index).Trim();

            if (parsingTuneBody)
            {
                foreach (var v in tune.voices)
                    v.items.Add(new TimeSignatureItem(timeSignature));
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
            if (!Elements.noteLengths.TryGetValue(lengthInfo, out Note.Length length))
                throw new ParseException($"Unrecognized note length: {lengthInfo} at {lineNum}, {index - lengthInfo.Length}");

            unitNoteLength = length;
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

                if (timeSignature != null)
                    voice.items.Add(new TimeSignatureItem(timeSignature));

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
