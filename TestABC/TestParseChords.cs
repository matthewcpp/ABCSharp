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
            var expectedNotes = new List<Chord.Element>()
            {
                new Chord.Element(Pitch.C4, Accidental.Unspecified),
                new Chord.Element(Pitch.E4, Accidental.Unspecified),
                new Chord.Element(Pitch.G4, Accidental.Unspecified)
            };

            var tune = Tune.Load("[CEG]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(1, voice.items.Count);
            var chord = voice.items[0] as Chord;
            Assert.IsNotNull(chord);
            Assert.AreEqual(expectedNotes.Count, chord.notes.Length);

            for (int i = 0; i < expectedNotes.Count; i++)
                Assert.AreEqual(expectedNotes[i], chord.notes[i]);
        }

        [TestMethod]
        public void ComplexChord()
        {
            var expectedNotes = new List<Chord.Element>()
            {
                new Chord.Element(Pitch.F3, Accidental.Unspecified),
                new Chord.Element(Pitch.A3, Accidental.Unspecified)
            };

            var tune = Tune.Load("[F,2 A,2]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.items.Count);

            var chord = voice.items[0] as Chord;
            Assert.IsNotNull(chord);
            Assert.AreEqual(expectedNotes.Count, chord.notes.Length);

            for (int i = 0; i < expectedNotes.Count; i++)
                Assert.AreEqual(expectedNotes[i], chord.notes[i]);
        }

        [TestMethod]
        public void ChordLength()
        {
            var abc = "L:1/4\n[C2E2G2] [C2E2G2]2 [C/2C2C2]";

            var expectedLengths = new List<Length>()
            {
                Length.Half, Length.Whole, Length.Eighth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedLengths.Count, voice.items.Count);

            for (int i = 0; i < expectedLengths.Count; i++)
            {
                var chord = voice.items[i] as Chord;
                Assert.IsNotNull(chord);
                Assert.AreEqual(expectedLengths[i], chord.length);
            }
        }

        [TestMethod]
        public void ChordBeam()
        {
            var abc = "L:1/8\n[CEG][CEG] [CEG]";
            var expectedBeams = new List<int>()
            {
                1,1,0
            };

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);

            var voice = tune.voices[0];
            Assert.AreEqual(expectedBeams.Count, voice.items.Count);

            for (int i = 0; i < expectedBeams.Count; i++)
            {
                var chord = voice.items[i] as Chord;
                Assert.IsNotNull(chord);
                
                Assert.AreEqual(expectedBeams[i], chord.beam);
            }
        }

        [TestMethod]
        public void SortsElements()
        {
            var notes = new List<Note>()
            {
                new Note(Pitch.C4),
                new Note(Pitch.D2),
                new Note(Pitch.E3),
                new Note(Pitch.A6)
            };

            var expectedElementPitches = new List<Pitch>()
            {
                Pitch.D2, Pitch.E3, Pitch.C4, Pitch.A6
            };
            
            var chord = Chord.FromNotes(notes);
            Assert.AreEqual(expectedElementPitches.Count, chord.notes.Length);
            
            for (int i = 0; i < expectedElementPitches.Count; i++)
                Assert.AreEqual(expectedElementPitches[i], chord.notes[i].pitch);
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
