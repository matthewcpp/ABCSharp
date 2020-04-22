using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseDecorations
    {
        [TestMethod]
        public void AttachesToNotes()
        {
            var abc = "!1!!3!!5!C";

            var expectedDecorationValues = new List<string>()
            {
                "1", "3", "5"   
            };
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(1, tune.decorations.Count);
            
            Assert.AreEqual(1, voice.items.Count);
            var note = voice.items[0] as Note;
            Assert.IsNotNull(note);

            Assert.IsTrue(tune.decorations.ContainsKey(note));
            var decorators = tune.decorations[note];
            Assert.IsTrue(expectedDecorationValues.SequenceEqual(decorators));
        }

        [TestMethod]
        public void AttachesToChords()
        {
            var abc = "!decoration![CDE]";

            var expectedDecorationValues = new List<string>()
            {
                "decoration"
            };
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(1, tune.decorations.Count);
            
            Assert.AreEqual(1, voice.items.Count);
            var chord = voice.items[0] as Chord;
            Assert.IsNotNull(chord);

            Assert.IsTrue(tune.decorations.ContainsKey(chord));
            var decorators = tune.decorations[chord];
            Assert.IsTrue(expectedDecorationValues.SequenceEqual(decorators));
        }

        [TestMethod]
        public void AttachesToRests()
        {
            var abc = "!test!z";

            var expectedDecorationValues = new List<string>()
            {
                "test"
            };
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(1, tune.decorations.Count);
            
            Assert.AreEqual(1, voice.items.Count);
            var rest = voice.items[0] as Rest;
            Assert.IsNotNull(rest);

            Assert.IsTrue(tune.decorations.ContainsKey(rest));
            var decorators = tune.decorations[rest];
            Assert.IsTrue(expectedDecorationValues.SequenceEqual(decorators));
        }

        [TestMethod]
        public void AttachesToBarLines()
        {
            var abc = "!test!|";

            var expectedDecorationValues = new List<string>()
            {
                "test"
            };
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(1, tune.decorations.Count);
            
            Assert.AreEqual(1, voice.items.Count);
            var bar = voice.items[0] as Bar;
            Assert.IsNotNull(bar);

            Assert.IsTrue(tune.decorations.ContainsKey(bar));
            var decorators = tune.decorations[bar];
            Assert.IsTrue(expectedDecorationValues.SequenceEqual(decorators));
        }

        [TestMethod]
        public void OnlyAttachesToNeighbor()
        {
            var abc = "!1!CDE!2!!3!F!5!!6!!7!G";

            var expectedDecorationCounts = new List<int>()
            {
                1, 0, 0, 2, 3
            };

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedDecorationCounts.Count, voice.items.Count);

            for (int i = 0; i < voice.items.Count; i++)
            {
                if (expectedDecorationCounts[i] == 0)
                    Assert.IsFalse(tune.decorations.ContainsKey(voice.items[i]));
                else
                    Assert.AreEqual(expectedDecorationCounts[i], tune.decorations[voice.items[i]].Count);
            }
        }

        [TestMethod]
        public void InvalidDecorations()
        {
            var invalidAbc = new List<string>()
            {
                "AB!decoratio", // unterminated
                "!bogus![V:1]" // attached to illegal item
            };

            foreach (var invalid in invalidAbc)
            {
                Assert.ThrowsException<ParseException>(() => { Tune.Load(invalid); });
            }
        }
    }
}