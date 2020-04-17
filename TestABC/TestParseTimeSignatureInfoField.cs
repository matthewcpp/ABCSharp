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
            
            Assert.AreEqual(2, voice.items.Count);
            var timeSignatureItem = voice.items[0] as TimeSignature;
            
            Assert.IsNotNull(timeSignatureItem);
            Assert.AreEqual(expectedTimeSignature, timeSignatureItem.value);
        }

        [TestMethod]
        public void SetsTimeSignatureFromTuneBodyForAllVoices()
        {
            var abc = @"
            M:4/4
            [V:1] CCCC
            [V:2] CCCC
            M:2/2
            ";
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(2, tune.voices.Count);
            foreach (var voice in tune.voices)
            {
                Assert.AreEqual(6, voice.items.Count);
                var defaultTimeSignature = voice.items[0] as TimeSignature;
                Assert.IsNotNull(defaultTimeSignature);
                Assert.AreEqual("4/4", defaultTimeSignature.value);

                var cutTimeItem = voice.items[5] as TimeSignature;
                Assert.IsNotNull(cutTimeItem);
                Assert.AreEqual("2/2", cutTimeItem.value);
            }
        }

        [TestMethod]
        public void SetsTimeSignatureInline()
        {
            var commonTime = "4/4";
            var cutTime = "2/2";
            var abc = $"M:{commonTime}\nC[M:{cutTime}]D";
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(4, voice.items.Count);
            var commonTimeItem = voice.items[0] as TimeSignature;
            
            Assert.IsNotNull(commonTimeItem);
            Assert.AreEqual(commonTime, commonTimeItem.value);

            var cutTimeItem = voice.items[2] as TimeSignature;
            Assert.IsNotNull(cutTimeItem);
            Assert.AreEqual(cutTime, cutTimeItem.value);
        }
    }
}