using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseTuneBody
    {
        [TestMethod]
        public void Body()
        {
            var expectedTypes = new List<Item.Type>()
            {
                Item.Type.Note, Item.Type.Chord, Item.Type.Note, Item.Type.Chord, Item.Type.Bar, Item.Type.Note
            };

            var tune = Tune.Load("_a' [CEF]B[^gAB]|^F");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(expectedTypes.Count, voice.items.Count);

            for (int i = 0; i < expectedTypes.Count; i++)
                Assert.AreEqual(expectedTypes[i], voice.items[i].type);
        }
    }
}
