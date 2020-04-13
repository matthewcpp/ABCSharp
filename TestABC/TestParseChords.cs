using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseChords
    {
        [TestMethod]
        public void BasicChord()
        {
            var expectedNotes = new List<Note>()
            {
                new Note(Note.Pitch.C4),
                new Note(Note.Pitch.E4),
                new Note(Note.Pitch.G4)
            };

            var tune = Tune.Load("[CEG]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(Clef.Treble, voice.clef);
            Assert.AreEqual(1, voice.items.Count);

            var chord = voice.items[0] as ChordItem;
            Assert.IsNotNull(chord);
            Assert.AreEqual(expectedNotes.Count, chord.notes.Length);

            for (int i = 0; i < expectedNotes.Count; i++)
                Assert.AreEqual(expectedNotes[i], chord.notes[i]);
        }

        [TestMethod]
        public void ComplexChord()
        {
            var expectedNotes = new List<Note>()
            {
                new Note(Note.Pitch.F3, Length.Quarter),
                new Note(Note.Pitch.A3, Length.Quarter)
            };

            var tune = Tune.Load("[F,2 A,2]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.items.Count);

            var chord = voice.items[0] as ChordItem;
            Assert.IsNotNull(chord);
            Assert.AreEqual(expectedNotes.Count, chord.notes.Length);

            for (int i = 0; i < expectedNotes.Count; i++)
                Assert.AreEqual(expectedNotes[i], chord.notes[i]);
        }

        [TestMethod]
        public void UnterminatedChord()
        {
            Assert.ThrowsException<ParseException>(()=> { Tune.Load("[CEG"); });
        }

        [TestMethod]
        public void InvalidCharacterInChord()
        {
            Assert.ThrowsException<ParseException>(() => { Tune.Load("[CE[G]"); });
            Assert.ThrowsException<ParseException>(() => { Tune.Load("[#gAB]"); });
        }
    }
}
