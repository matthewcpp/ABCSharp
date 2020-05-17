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

            var expectedNoteLengths = new List<Length>()
            {
                Length.Quarter, Length.Half, Length.Whole,
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i  = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as Note;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.length);
            }
        }

        [TestMethod]
        public void DivideNoteLengthWithNumber()
        {
            var abc = @"
                I:linebreak <none>
                [L:1] C C/2 C/4 C/8 C/16
                [L:1/2] C/2 C/4 C/8";

            var expectedNoteLengths = new List<Length>()
            {
                Length.Whole, Length.Half, Length.Quarter, Length.Eighth, Length.Sixteenth,
                Length.Quarter, Length.Eighth, Length.Sixteenth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as Note;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.length);
            }
        }

        [TestMethod]
        public void DivideNoteLengthWithShorthand()
        {
            var abc = @"
                I:linebreak <none>
                [L:1] C C/ C// C/// C////
                [L:1/2] C/ C// C///";

            var expectedNoteLengths = new List<Length>()
            {
                Length.Whole, Length.Half, Length.Quarter, Length.Eighth, Length.Sixteenth,
                Length.Quarter, Length.Eighth, Length.Sixteenth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedNoteLengths.Count, voice.items.Count);
            for (int i = 0; i < expectedNoteLengths.Count; i++)
            {
                var noteItem = voice.items[i] as Note;
                Assert.IsNotNull(noteItem);

                Assert.AreEqual(expectedNoteLengths[i], noteItem.length);
            }
        }

        [TestMethod]
        public void DottedNotes()
        {
            var lengthValues = new List<string>()
            {
                "1", "1/1", "1/2", "1/4", "1/8", "1/16"
            };

            var expectedUnitLengths = new List<Length>()
            {
                Length.Whole, Length.Whole, Length.Half, Length.Quarter, Length.Eighth, Length.Sixteenth
            };

            for (int i = 0; i < expectedUnitLengths.Count; i++)
            {
                var abc = $"L:{lengthValues[i]}\nC3/2";

                var tune = Tune.Load(abc);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                Assert.AreEqual(1, voice.items.Count);
                var noteItem = voice.items[0] as Note;
                
                Assert.AreEqual(expectedUnitLengths[i], noteItem.length);
                Assert.AreEqual(1, noteItem.dotCount);
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
