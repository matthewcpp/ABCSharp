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
            var timeSignatureItem = voice.items[0] as TimeSignatureItem;
            
            Assert.IsNotNull(timeSignatureItem);
            Assert.AreEqual(expectedTimeSignature, timeSignatureItem.timeSignature);
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
                var defaultTimeSignature = voice.items[0] as TimeSignatureItem;
                Assert.IsNotNull(defaultTimeSignature);
                Assert.AreEqual("4/4", defaultTimeSignature.timeSignature);

                var cutTimeItem = voice.items[5] as TimeSignatureItem;
                Assert.IsNotNull(cutTimeItem);
                Assert.AreEqual("2/2", cutTimeItem.timeSignature);
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
            var commonTimeItem = voice.items[0] as TimeSignatureItem;
            
            Assert.IsNotNull(commonTimeItem);
            Assert.AreEqual(commonTime, commonTimeItem.timeSignature);

            var cutTimeItem = voice.items[2] as TimeSignatureItem;
            Assert.IsNotNull(cutTimeItem);
            Assert.AreEqual(cutTime, cutTimeItem.timeSignature);
        }
    }
}