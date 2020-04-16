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
                var restItem = voice.items[i] as RestItem;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedLengths[i], restItem.rest.length);
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
                var restItem = voice.items[i] as RestItem;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedVisibilities[i], restItem.rest.isVisible);
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
                    Assert.IsNotNull(voice.items[i] as RestItem);
                else
                    Assert.IsNotNull(voice.items[i] as Note);
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