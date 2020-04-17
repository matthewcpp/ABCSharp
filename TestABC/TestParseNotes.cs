using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseNotes
    {
        [TestMethod]
        public void BasicNotes()
        {
            var notes = "C,,C,Ccc'c''";

            var expectedNotes = new List<Note>()
            {
                new Note(Pitch.C2), new Note(Pitch.C3), 
                new Note(Pitch.C4), new Note(Pitch.C5), 
                new Note(Pitch.C6), new Note(Pitch.C7)
            };
            
            var tune = Tune.Load(notes);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedNotes.Count, voice.items.Count);

            for (int i = 0; i < expectedNotes.Count; i++)
            {
                Assert.AreEqual(Item.Type.Note, voice.items[i].type);
                var noteItem = voice.items[i] as Note;
                Assert.AreEqual(expectedNotes[i], noteItem);
            }
        }

        [TestMethod]
        public void Accidentals()
        {
            var notes = new List<string>()
            {
                "C", "^C", "_C", "=C",
            };

            var expectedNotes = new List<Note>()
            {
                new Note(Pitch.C4, Length.Eighth, Accidental.Unspecified),
                new Note(Pitch.C4, Length.Eighth, Accidental.Sharp),
                new Note(Pitch.C4, Length.Eighth, Accidental.Flat),
                new Note(Pitch.C4, Length.Eighth, Accidental.Natural)
            };

            Assert.AreEqual(notes.Count, expectedNotes.Count);

            for (int i = 0; i < notes.Count; i++)
            {
                var tune = Tune.Load(notes[i]);

                Assert.AreEqual(1, tune.voices.Count);
                var staff = tune.voices[0];

                Assert.AreEqual(Clef.Treble, staff.clef);
                Assert.AreEqual(1, staff.items.Count);

                Assert.AreEqual(Item.Type.Note, staff.items[0].type);
                var noteItem = staff.items[0] as Note;

                Assert.AreEqual(expectedNotes[i], noteItem);
            }
        }

        [TestMethod]
        public void MultipleNotes()
        {

            var notes = new List<string>()
            {
                "_A'^CD", " _A'   ^C  D "
            };

            var expectedNotes = new List<Note>()
            {
                new Note(Pitch.A5, Length.Eighth, Accidental.Flat),
                new Note(Pitch.C4, Length.Eighth, Accidental.Sharp),
                new Note(Pitch.D4, Length.Eighth, Accidental.Unspecified)
            };

            foreach (var noteStr in notes)
            {
                var tune = Tune.Load(noteStr);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                Assert.AreEqual(Clef.Treble, voice.clef);
                Assert.AreEqual(expectedNotes.Count, voice.items.Count);

                for (int i = 0; i < expectedNotes.Count; i++)
                {
                    Assert.AreEqual(Item.Type.Note, voice.items[i].type);
                    var noteItem = voice.items[i] as Note;

                    Assert.AreEqual(expectedNotes[i], noteItem);
                }
            }
        }

        [TestMethod]
        public void InvalidNotes()
        {
            Assert.ThrowsException<ParseException>(() => { Tune.Load("#M"); });
            Assert.ThrowsException<ParseException>(() => { Tune.Load("_M"); });
            Assert.ThrowsException<ParseException>(() => { Tune.Load("_AB'Q"); });
        }

        [TestMethod]
        public void BeamsEighthAndSixteenthNotesWithNoWhitespace()
        {
            var abc = "[L:1/8]aac2[L:1/16]bbc4";

            var expectedBeamVals = new List<int>()
            {
                1, 1, 0, 2, 2, 0
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedBeamVals.Count, voice.items.Count);

            for (int i = 0; i < expectedBeamVals.Count; i++)
            {
                var noteItem = voice.items[i] as Note;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedBeamVals[i], noteItem.beam);
            }
        }

        [TestMethod]
        public void DoesNotBeamNotesWithWhitespace()
        {
            var abc = "[L:1/8]a b[L:1/16]b c";

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(4, voice.items.Count);

            foreach (var item in voice.items)
            {
                var noteItem = item as Note;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(0, noteItem.beam);
            }
        }

        [TestMethod]
        public void BeamInterruptions()
        {
            var abc = "L:1/8\ncccc c cc[cde]c cczc cc|c";

            // items index -> beam value
            var expectedBeamValues = new Dictionary<int, int>()
            {
                { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 0 }, { 5, 3 },{ 6, 3 },{ 8, 0 }, { 9, 5 },{ 10, 5 }, { 12, 0 }, { 13, 7 }, { 14, 7 }, { 16, 0 }
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            foreach(var expectedValue in expectedBeamValues)
            {
                var noteItem = voice.items[expectedValue.Key] as Note;

                Assert.IsNotNull(noteItem);
                Assert.AreEqual(expectedValue.Value, noteItem.beam);
            }
        }
    }
}
