using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    class Parser
    {
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

        void EnsureVoice()
        {
            if (voice == null)
            {
                voice = new Voice();
                tune.voices.Add(voice);
            }
        }

        void ParseTuneBody()
        {
            while (index < currentLine.Length)
            {
                if (Char.IsWhiteSpace(currentLine[index]))
                {
                    index += 1;
                    continue;
                }

                if (currentLine[index] == '[')
                {
                    if (Elements.IsStartOfNoteStream(currentLine[index + 1]))
                        ParseChord();
                }
                else if (Elements.IsStartOfNoteStream(currentLine[index]))
                {
                    EnsureVoice();
                    var note = ReadNote();
                    voice.items.Add(new NoteItem(note));
                }
                else
                {
                    throw new ParseException(string.Format("Unexpected character: {0} at {1}, {2}", currentLine[index], currentLine, index));
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
                    throw new ParseException(string.Format("Invalid character in chord at {0}, {1}", currentLine, index));

                notes.Add(ReadNote());

                if (index == currentLine.Length)
                    throw new ParseException(string.Format("Unterminated chord at {0}, {1}", currentLine, index));

            } while (currentLine[index] != ']');

            index += 1;

            if (notes.Count == 0)
                throw new ParseException(string.Format("Encountered empty chord at {0}, {1}", currentLine, index));

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

            // start with default C value for the clef
            int noteValue = (int)Elements.cleffReferenceNotes[voice.cleff];

            try
            {
                // adjust the value based on the actual note value
                noteValue += Elements.GetNoteOffset(currentLine[index]);
                index += 1;
            }
            catch (ParseException e)
            {
                throw new ParseException(string.Format("{0} at {1}, {2}", e.Message, currentLine, index));
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
            }
        }

        void ParseReferenceNumber()
        {
            string referenceNumberStr = currentLine.Substring(2);
            uint referenceNumber;
            if (uint.TryParse(referenceNumberStr, out referenceNumber))
                tune.referenceNumber = referenceNumber;
            else
                throw new ParseException(string.Format("Error Parsing Reference number {0},{1}", currentLine, 3));
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
