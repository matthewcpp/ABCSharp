using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABC;
namespace TestABC
{
    [TestClass]
    public class TestParseBar
    {

        [TestMethod]
        public void BarLine()
        {
            var tune = Tune.Load("|");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.items.Count);
            var barItem = voice.items[0] as Bar;
            Assert.IsNotNull(barItem);

            Assert.AreEqual(Bar.Kind.Line, barItem.kind);
        }

        [TestMethod]
        public void ThinThinLine()
        {
            var tune = Tune.Load("c||[cEG]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(3, voice.items.Count);

            Assert.IsNotNull(voice.items[0] as Note);
            var bar = voice.items[1] as Bar;
            Assert.IsNotNull(bar);
            Assert.AreEqual(Bar.Kind.ThinThinDoubleBar, bar.kind);

            Assert.IsNotNull(voice.items[2] as Chord);
        }

        [TestMethod]
        public void ThinThickDoubleLine()
        {
            var tune = Tune.Load("[|[cEG]c");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(3, voice.items.Count);

            var bar = voice.items[0] as Bar;
            Assert.IsNotNull(bar);
            Assert.AreEqual(Bar.Kind.ThickThinDoubleBar, bar.kind);

            Assert.IsNotNull(voice.items[1] as Chord);
            Assert.IsNotNull(voice.items[2] as Note);
        }
        
        [TestMethod]
        public void ThickThinDoubleLine()
        {
            var tune = Tune.Load("c[cEG]|]");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(3, voice.items.Count);

            var bar = voice.items[2] as Bar;
            Assert.IsNotNull(bar);
            Assert.AreEqual(Bar.Kind.ThinThickDoubleBar, bar.kind);

            Assert.IsNotNull(voice.items[0] as Note);
            Assert.IsNotNull(voice.items[1] as Chord);
        }

        [TestMethod]
        public void StartRepeatSection()
        {
            var tests = new List<Tuple<string, Bar.Kind, int>>()
            {
                Tuple.Create("|:ccc", Bar.Kind.Line, 1),
                Tuple.Create("[|::ccc", Bar.Kind.ThickThinDoubleBar, 2),
                Tuple.Create("||:::ccc", Bar.Kind.ThinThinDoubleBar, 3),
            };

            foreach (var test in tests)
            {
                var tune = Tune.Load(test.Item1);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                
                Assert.AreEqual(4, voice.items.Count);
                var bar = voice.items[0] as Bar;
                Assert.IsNotNull(bar);
                
                Assert.AreEqual(test.Item2, bar.kind);
                Assert.AreEqual(test.Item3, bar.startRepeatCount);
            }
        }
        
        [TestMethod]
        public void EndRepeatSection()
        {
            var tests = new List<Tuple<string, Bar.Kind, int>>()
            {
                Tuple.Create("ccc:|", Bar.Kind.Line, 1),
                Tuple.Create("ccc::|]", Bar.Kind.ThinThickDoubleBar, 2),
                Tuple.Create("ccc:::||", Bar.Kind.ThinThinDoubleBar, 3),
            };

            foreach (var test in tests)
            {
                var tune = Tune.Load(test.Item1);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                
                Assert.AreEqual(4, voice.items.Count);
                var bar = voice.items[3] as Bar;
                Assert.IsNotNull(bar);
                
                Assert.AreEqual(test.Item2, bar.kind);
                Assert.AreEqual(test.Item3, bar.endRepeatCount);
            }
        }
    }
}
