using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseVoiceHeader
    {
        [TestMethod]
        public void Identifier()
        {
            var tune = Tune.Load("V:test");

            Assert.AreEqual(1, tune.voices.Count);
            Assert.AreEqual("test", tune.voices[0].identifier);
        }

        [TestMethod]
        public void MultipleVoices()
        {
            var tune = Tune.Load("V:1\nV:2");

            Assert.AreEqual(2, tune.voices.Count);
            Assert.AreEqual("1", tune.voices[0].identifier);
            Assert.AreEqual("2", tune.voices[1].identifier);
        }

        [TestMethod]
        public void SetsClef()
        {
            var tune = Tune.Load("V:1\nV:2\tclef=treble\nV:3 clef = bass");
            
            Assert.AreEqual(3, tune.voices.Count);
            Assert.AreEqual(Cleff.Treble, tune.voices[0].cleff);
            Assert.AreEqual(Cleff.Treble, tune.voices[1].cleff);
            Assert.AreEqual(Cleff.Bass, tune.voices[2].cleff);
        }

        [TestMethod]
        public void SetsName()
        {
            var tune = Tune.Load("V:1 name=test\nV:2 name=\"two words\"");
            
            Assert.AreEqual(2, tune.voices.Count);
            Assert.AreEqual("test", tune.voices[0].name);
            Assert.AreEqual("two words", tune.voices[1].name);
        }
    }
}
