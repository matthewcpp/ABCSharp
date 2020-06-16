using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseTimeSignatureInfoField
    {
        [TestMethod]
        public void SetsDefaultTimeSignatureFromHeader()
        {
            var expectedTimeSignature = "4/4";
            var abc = $"M:{expectedTimeSignature}\nC";
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedTimeSignature, voice.initialTimeSignature);
        }

        [TestMethod]
        public void SetsTimeSignatureFromTuneBodyForAllVoices()
        {
            var expectedInitialTimeSignature = "4/4";
            var expectedTimeSignatureItem = "2/2";
            
            var abc = $@"
            M:{expectedInitialTimeSignature}
            [V:1] C
            [V:2] C
            M:{expectedTimeSignatureItem}
            ";

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);
            foreach (var voice in tune.voices)
            {
                Assert.AreEqual(expectedInitialTimeSignature, voice.initialTimeSignature);

                Assert.AreEqual(2, voice.items.Count);
                var defaultTimeSignature = voice.items[1] as TimeSignature;
                Assert.IsNotNull(defaultTimeSignature);
                Assert.AreEqual(expectedTimeSignatureItem, defaultTimeSignature.value);
            }
        }

        [TestMethod]
        public void SetsTimeSignatureInline()
        {
            var commonTime = "4/4";
            var cutTime = "2/2";
            var abc = $"M:{commonTime}\nC[M:{cutTime}]";
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(commonTime, voice.initialTimeSignature);
            Assert.AreEqual(2, voice.items.Count);

            var timeSignature = voice.items[1] as TimeSignature;
            Assert.IsNotNull(timeSignature);
            Assert.AreEqual(cutTime, timeSignature.value);
        }

        [TestMethod]
        public void ParseSymbols()
        {
            var symbols = new List<string>()
            {
                "C", "C|"
            };

            foreach (var symbol in symbols)
            {
                var abc = $"M:{symbol}\nC";
                var tune = Tune.Load(abc);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                Assert.AreEqual(symbol, voice.initialTimeSignature, symbol);
            }
        }

        [TestMethod]
        public void FreeMeter()
        {
            var abc = "M: none\nC";

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            Assert.AreEqual(string.Empty, voice.initialTimeSignature);
        }
    }
}