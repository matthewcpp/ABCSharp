using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;

namespace TestABC
{
    [TestClass]
    public class TestParseUnitNoteLengthInfoField
    {
        [TestMethod]
        public void SetInHeader()
        {
            var lengthValues = new List<string>()
            {
                "L: 1", "L: 1/1", "L:1/2", "L:1/4", "L:1/8", "L:1/16"
            };

            var expectedUnitLengths = new List<Length>()
            {
                Length.Whole, Length.Whole, Length.Half, Length.Quarter, Length.Eighth, Length.Sixteenth
            };
            
            Assert.AreEqual(expectedUnitLengths.Count, lengthValues.Count);

            for (int i = 0; i < lengthValues.Count; i++)
            {
                var abc = $"{lengthValues[i]}\nC";
                var tune = Tune.Load(abc);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                
                Assert.AreEqual(1, voice.items.Count);
                Assert.AreEqual(Item.Type.Note, voice.items[0].type);
                var noteItem = voice.items[0] as NoteItem;
                
                Assert.AreEqual(expectedUnitLengths[i], noteItem.note.length);
            }
        }

        [TestMethod]
        public void InvalidUnitNoteLength()
        {
            var values = new List<string>()
            {
                "L:1/100", "L:blah"
            };

            foreach (var str in values)
            {
                Assert.ThrowsException<ParseException>(() => { Tune.Load(str); });
            }
        }

        [TestMethod]
        public void SetInTuneBody()
        {
            var abc = @"
            L:1/2
            C [L:1/4] C [L:1/1] C [L:1/8] C
            ";

            var expectedNoteLengths = new List<Length>()
            {
                Length.Half, Length.Quarter, Length.Whole, Length.Eighth
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
    }
}
