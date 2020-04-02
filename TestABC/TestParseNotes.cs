using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class ParseBasicNotesTrebleCleff
    {
        [TestMethod]
        public void BasicNotes()
        {
            var notes = new List<string>()
            {
                "C", "c", "C''",
                "D", "d", "d'"
            };

            var expectedNotes = new List<Note>
            {
                new Note(Note.Value.C4), new Note(Note.Value.C5), new Note(Note.Value.C6),
                new Note(Note.Value.D4), new Note(Note.Value.D5), new Note(Note.Value.D6)
            };

            Assert.AreEqual(notes.Count, expectedNotes.Count);

            for (int i = 0; i < notes.Count; i++)
            {
                var tune = Tune.Load(notes[i]);
                Assert.AreEqual(1, tune.voices.Count);

                var voice = tune.voices[0];

                Assert.AreEqual(Cleff.Treble, voice.cleff);
                Assert.AreEqual(1, voice.items.Count);

                Assert.AreEqual(Item.Type.Note, voice.items[0].type);
                var noteItem = voice.items[0] as NoteItem;

                Assert.AreEqual(expectedNotes[i], noteItem.note);
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
                new Note(Note.Value.C4, Note.Length.Eighth, Note.Accidental.Unspecified),
                new Note(Note.Value.C4, Note.Length.Eighth, Note.Accidental.Sharp),
                new Note(Note.Value.C4, Note.Length.Eighth, Note.Accidental.Flat),
                new Note(Note.Value.C4, Note.Length.Eighth, Note.Accidental.Natural)
            };

            Assert.AreEqual(notes.Count, expectedNotes.Count);

            for (int i = 0; i < notes.Count; i++)
            {
                var tune = Tune.Load(notes[i]);

                Assert.AreEqual(1, tune.voices.Count);
                var staff = tune.voices[0];

                Assert.AreEqual(Cleff.Treble, staff.cleff);
                Assert.AreEqual(1, staff.items.Count);

                Assert.AreEqual(Item.Type.Note, staff.items[0].type);
                var noteItem = staff.items[0] as NoteItem;

                Assert.AreEqual(expectedNotes[i], noteItem.note);
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
                new Note(Note.Value.A5, Note.Length.Eighth, Note.Accidental.Flat),
                new Note(Note.Value.C4, Note.Length.Eighth, Note.Accidental.Sharp),
                new Note(Note.Value.D4, Note.Length.Eighth, Note.Accidental.Unspecified)
            };

            foreach (var noteStr in notes)
            {
                var tune = Tune.Load(noteStr);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                Assert.AreEqual(Cleff.Treble, voice.cleff);
                Assert.AreEqual(expectedNotes.Count, voice.items.Count);

                for (int i = 0; i < expectedNotes.Count; i++)
                {
                    Assert.AreEqual(Item.Type.Note, voice.items[i].type);
                    var noteItem = voice.items[i] as NoteItem;

                    Assert.AreEqual(expectedNotes[i], noteItem.note);
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
    }
}
