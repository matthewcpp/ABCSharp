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
        private Stream stream;
        private Tune tune;

        private Voice voice;

        StreamReader reader;

        int lineNum = 0;
        int index = 0;
        string currentLine;

        public Tune Parse(Stream stream)
        {
            reader = new StreamReader(stream);
            tune = new Tune();

            while (reader.Peek() >= 0)
            {
                currentLine = reader.ReadLine();
                lineNum += 1;
                index = 0;

                if (LineIsInformationField())
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
            tune.voices.Add(voice);
        }

        bool SkipWhiteSpace()
        {
            while (index < currentLine.Length && Char.IsWhiteSpace(currentLine[index]))
                index += 1;

            return index == currentLine.Length;
        }

        bool ReadUntil(Func<char, bool> condition, out string result)
        {
            int start = index;
            int length = 0;
            while (index + length < currentLine.Length && !condition(currentLine[index + length]))
            {
                length += 1;
            }

            result = currentLine.Substring(start, length);
            index += length;

            return index == currentLine.Length;
        }

        void ParseTuneBody()
        {
            while (index < currentLine.Length)
            {
                if (SkipWhiteSpace()) return;

                if (currentLine[index] == '[')
                {
                    if (Elements.IsStartOfNoteStream(currentLine[index + 1]))
                        ParseChord();
                    else
                        throw new ParseException(string.Format("Unexpected character: {0} at {1}, {2}", currentLine[index], lineNum, index));
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
                    throw new ParseException(string.Format("Unexpected character: {0} at {1}, {2}", currentLine[index], lineNum, index));
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
                    throw new ParseException(string.Format("Invalid character in chord at {0}, {1}", lineNum, index));

                notes.Add(ReadNote());

                if (index == currentLine.Length)
                    throw new ParseException(string.Format("Unterminated chord at {0}, {1}", lineNum, index));

            } while (currentLine[index] != ']');

            index += 1;

            if (notes.Count == 0)
                throw new ParseException(string.Format("Encountered empty chord at {0}, {1}", lineNum, index));

            voice.items.Add(new ChordItem(notes));
        }

        Note ReadNote()
        {
            var note = new Note();

            note.accidental = Elements.GetAccidental(currentLine[index]);
            note.length = Note.Length.Eighth; // temp until note length parsing
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
                throw new ParseException(string.Format("{0} at {1}, {2}", e.Message, lineNum, index));
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

            return note;
        }

        void ParseInformationField()
        {
            switch (currentLine[0])
            {
                case 'X':
                    ParseReferenceNumber();
                    break;

                case 'V':
                    ParseVoiceHeader();
                    break;
            }
        }

        void ParseReferenceNumber()
        {
            string referenceNumberStr = currentLine.Substring(2);
            uint referenceNumber;
            if (uint.TryParse(referenceNumberStr, out referenceNumber))
                tune.referenceNumber = referenceNumber;
            else
                throw new ParseException(string.Format("Error Parsing Reference number {0},{1}", lineNum, 3));
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

            voice = new Voice(identifier);
            tune.voices.Add(voice);
            
            if(SkipWhiteSpace()) return;
            

            while (!eol)
            {
                eol = ReadUntil((char c) => { return char.IsWhiteSpace(c) || c == '='; }, out string key);
                if (eol)
                    throw new ParseException(string.Format("Unterminated Modifier at {0}, {1}", lineNum, index));
                
                SkipWhiteSpace();
                
                if (currentLine[index] != '=')
                    throw new ParseException(string.Format("Unexpected Character {0} at {1}, {2}", currentLine[index], lineNum, index));
                
                index += 1;
                
                SkipWhiteSpace();

                ReadModifierValue(out string value);

                switch (key)
                {
                    case "clef":
                        var clefString = textInfo.ToTitleCase(value);

                        try
                        {
                            voice.cleff = (Cleff) Enum.Parse(typeof(Cleff), clefString);
                        }
                        catch (ArgumentException e)
                        {
                            throw new ParseException(string.Format("Invalid clef value {0} at {1}, {2}", value, lineNum, index - value.Length));
                        }
                        
                        break;
                    
                    case "name":
                        voice.name = value;
                        break;
                }

                eol = SkipWhiteSpace();
            }
        }

        bool LineIsInformationField()
        {
            if (currentLine.Length < 2)
                return false;

            char ch = Char.ToLower(currentLine[0]);

            if (ch < 'a' || ch > 'z')
                return false;

            if (currentLine[1] != ':')
                return false;

            return true;
        }
    }
}
