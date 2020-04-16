using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestDuration
    {
        const float delta = 0.0001f;
        
        [TestMethod]
        public void TotalDuration()
        {
            var notes = new List<Note>()
            {
                new Note(Pitch.C4, Length.Whole, Note.Accidental.Unspecified, 1),
                new Note(Pitch.C4, Length.Whole, Note.Accidental.Unspecified, 2),
                new Note(Pitch.C4, Length.Whole, Note.Accidental.Unspecified, 3),
                
                new Note(Pitch.C4, Length.Half, Note.Accidental.Unspecified, 1),
                new Note(Pitch.C4, Length.Half, Note.Accidental.Unspecified, 2),
                new Note(Pitch.C4, Length.Half, Note.Accidental.Unspecified, 3),
                
                new Note(Pitch.C4, Length.Quarter, Note.Accidental.Unspecified, 1),
                new Note(Pitch.C4, Length.Quarter, Note.Accidental.Unspecified, 2),
                new Note(Pitch.C4, Length.Quarter, Note.Accidental.Unspecified, 3),
                
                new Note(Pitch.C4, Length.Eighth, Note.Accidental.Unspecified, 1),
                new Note(Pitch.C4, Length.Eighth, Note.Accidental.Unspecified, 2),
                new Note(Pitch.C4, Length.Eighth, Note.Accidental.Unspecified, 3),
                
                new Note(Pitch.C4, Length.Sixteenth, Note.Accidental.Unspecified, 1),
                new Note(Pitch.C4, Length.Sixteenth, Note.Accidental.Unspecified, 2),
                new Note(Pitch.C4, Length.Sixteenth, Note.Accidental.Unspecified, 3)
            };

            // https://en.wikipedia.org/wiki/Note_value
            var expectedDurations = new List<float>()
            {
                1.5f, 1.75f, 1 + 7.0f/ 8.0f,
                0.75f, 7.0f/8.0f, 15.0f/16.0f,
                3.0f/8.0f, 7.0f/16.0f, 15.0f/32.0f,
                3.0f/16.0f, 7.0f/32.0f, 15.0f/64.0f,
                3.0f/32.0f, 7.0f/64.0f, 15.0f/128.0f
            };

            for (int i = 0; i < expectedDurations.Count; i++)
                Assert.AreEqual(expectedDurations[i], notes[i].duration, delta);
        }
    }
}