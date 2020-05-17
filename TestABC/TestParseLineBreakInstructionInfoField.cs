using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseLineBreakInstructionInfoField
    {
        [TestMethod]
        public void LinebreakEOL()
        {
            var abc = "I:linebreak <EOL>\nC\nz\nC";

            var expectedTypes = new List<Item.Type>() { Item.Type.Note, Item.Type.LineBreak, Item.Type.Rest, Item.Type.LineBreak, Item.Type.Note };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedTypes.Count, voice.items.Count);
            for (int i = 0; i < expectedTypes.Count; i++)
                Assert.AreEqual(expectedTypes[i], voice.items[i].type);
        }

        [TestMethod]
        public void LinebreakNone()
        {
            var abc = "I:linebreak <none>\nC\nC$\nC";

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(3, voice.items.Count);
            foreach (var item in voice.items)
                Assert.IsNotNull(item as Note);
        }

        [TestMethod]
        public void LinebreakDollarSign()
        {
            var abc = "I:linebreak $\nC$C";

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(3, voice.items.Count);
            var lineBreak = voice.items[1] as LineBreak;
            Assert.IsNotNull(lineBreak);
        }

        [TestMethod]
        public void LinebreakMultiple()
        {
            var abc = "I:linebreak $ <EOL>\nC$C\nC";

            var expectedTypes = new List<Item.Type>
            {
                Item.Type.Note, Item.Type.LineBreak, Item.Type.Note, Item.Type.LineBreak, Item.Type.Note
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedTypes.Count, voice.items.Count);

            for (int i = 0; i < expectedTypes.Count; i++)
                Assert.AreEqual(expectedTypes[i], voice.items[i].type);
        }

        [TestMethod]
        public void MultipleVoices()
        {
            var abc = @"
            V:1
            V:2
            [V:1] C
            [V:2] C
            [V:1] |[DE]F
            [V:2] |[DE]F
            ";

            var expectedTypes = new List<Item.Type>()
            { 
                Item.Type.Note, Item.Type.LineBreak, Item.Type.Bar, Item.Type.Chord, Item.Type.Note
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);
            
            foreach (var voice in tune.voices)
            {
                Assert.AreEqual(expectedTypes.Count, voice.items.Count);

                for (int i = 0; i < expectedTypes.Count; i++)
                    Assert.AreEqual(expectedTypes[i], voice.items[i].type);
            }
        }

        [TestMethod]
        public void InvalidInTuneBody()
        {
            var abc = "[I:linebreak <EOL>]C";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }

        [TestMethod]
        public void InvalidLinebreakValues()
        {
            var abc = "I:linebreak bogus\nC";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }
    }
}
