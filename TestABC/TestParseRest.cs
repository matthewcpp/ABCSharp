using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseRest
    {
        [TestMethod]
        public void ParseRestLength()
        {
            var abc = "L:1/4\nx4x2xx/2x/4";

            var expectedLengths = new List<Length>()
            {
                Length.Whole, Length.Half, Length.Quarter, Length.Eighth, Length.Sixteenth
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedLengths.Count, voice.items.Count);

            for (int i = 0; i < expectedLengths.Count; i++)
            {
                var restItem = voice.items[i] as Rest;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedLengths[i], restItem.length);
            }
        }

        [TestMethod]
        public void ParseRestVisibility()
        {
            var abc = "zx";
            
            var expectedVisibilities = new List<bool>()
            {
                true, false
            };
            
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedVisibilities.Count, voice.items.Count);

            for (int i = 0; i < expectedVisibilities.Count; i++)
            {
                var restItem = voice.items[i] as Rest;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedVisibilities[i], restItem.isVisible);
            }
        }

        [TestMethod]
        public void ParseNotesAndRests()
        {
            var abc = "L:1/4\n c2 z2 A, z4";

            var expectedTypes = new List<Item.Type>()
            {
                Item.Type.Note, Item.Type.Rest, Item.Type.Note, Item.Type.Rest
            };
            
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedTypes.Count, voice.items.Count);

            for (int i = 0; i < expectedTypes.Count; i++)
            {
                if (expectedTypes[i] == Item.Type.Rest)
                    Assert.IsNotNull(voice.items[i] as Rest);
                else
                    Assert.IsNotNull(voice.items[i] as Note);
            }
        }

        [TestMethod]
        public void ParseBrokenRhythm()
        {
            var tests = new List<string>()
            {
                "z2 > z2", "z > z", "z/2 > z/2"
            };

            var expectedValues = new List<ValueTuple<Length, Length>>()
            {
                (Length.Half, Length.Quarter), (Length.Quarter, Length.Eighth), (Length.Eighth, Length.Sixteenth)
            };

            Assert.AreEqual(tests.Count, expectedValues.Count);

            for (int i = 0; i < tests.Count; i++)
            {
                var abc = $"X:1\nL:1/4\nM:C\nK:C\n{tests[i]}";
                var tune = Tune.Load(abc);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                Assert.AreEqual(2, voice.items.Count);

                var rest1 = voice.items[0] as Rest;
                Assert.IsNotNull(rest1);

                var rest2 = voice.items[1] as Rest;
                Assert.IsNotNull(rest2);

                Assert.AreEqual(expectedValues[i].Item1, rest1.length);
                Assert.AreEqual(expectedValues[i].Item2, rest2.length);
            }
        }

        [TestMethod]
        public void CannotAppearInChords()
        {
            var abc = "[CEx]";

            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc);});
        }
    }
}