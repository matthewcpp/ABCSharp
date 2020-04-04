using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseNoteLength
    {

        [TestMethod]
        public void MultiplyNoteLength()
        {
            var abc = "[L:1/4] C C2 C4";

            var expectedNoteLengths = new List<Note.Length>()
            {
                Note.Length.Quarter, Note.Length.Half, Note.Length.Whole,
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i  = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as NoteItem;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.note.length);
            }
        }

        [TestMethod]
        public void DivideNoteLengthWithNumber()
        {
            var abc = @"
                [L:1] C C/2 C/4 C/8 C/16
                [L:1/2] C/2 C/4 C/8";

            var expectedNoteLengths = new List<Note.Length>()
            {
                Note.Length.Whole, Note.Length.Half, Note.Length.Quarter, Note.Length.Eighth, Note.Length.Sixteenth,
                Note.Length.Quarter, Note.Length.Eighth, Note.Length.Sixteenth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as NoteItem;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.note.length);
            }
        }

        [TestMethod]
        public void DivideNoteLengthWithShorthand()
        {
            var abc = @"
                [L:1] C C/ C// C/// C////
                [L:1/2] C/ C// C///";

            var expectedNoteLengths = new List<Note.Length>()
            {
                Note.Length.Whole, Note.Length.Half, Note.Length.Quarter, Note.Length.Eighth, Note.Length.Sixteenth,
                Note.Length.Quarter, Note.Length.Eighth, Note.Length.Sixteenth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as NoteItem;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.note.length);
            }
        }

        [TestMethod]
        public void InvalidNoteLengths()
        {
            var notes = new List<string>()
            {
                "[L:1/8] C//////", "C/512", "C1/3/2"
            };

            foreach (var note in notes)
            {
                Assert.ThrowsException<ParseException>(() => { Tune.Load(note); });
            }
        }
    }
}
