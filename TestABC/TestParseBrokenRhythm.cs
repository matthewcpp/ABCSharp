using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseBrokenRhythm
    {
        [TestMethod]
        public void DotHalf()
        {
            var abc = "L:1/4\nC>DF [CDE]>[DEF]F z>zF";

            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);

            var voice = tune.voices[0];
            Assert.AreEqual(9, voice.items.Count);

            for (int i = 0; i < 9; i += 3)
            {
                var dotItem = voice.items[i] as Duration;
                var halfItem = voice.items[i + 1] as Duration;
                var normalItem = voice.items[i + 2] as Duration;
                
                Assert.IsNotNull(dotItem);
                Assert.IsNotNull(halfItem);
                Assert.IsNotNull(normalItem);
                
                Assert.AreEqual(Length.Quarter, dotItem.length);
                Assert.AreEqual(1, dotItem.dotCount);
                
                Assert.AreEqual(Length.Eighth, halfItem.length);
                Assert.AreEqual(0, halfItem.dotCount);
                
                Assert.AreEqual(Length.Quarter, normalItem.length);
                Assert.AreEqual(0, normalItem.dotCount);
            }
        }
        
        [TestMethod]
        public void HalfDot()
        {
            var abc = "L:1/4\nC<DF [CDE]<[DEF]F z<zF";

            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);

            var voice = tune.voices[0];
            Assert.AreEqual(9, voice.items.Count);

            for (int i = 0; i < 9; i += 3)
            {
                var halfItem = voice.items[i] as Duration;
                var dotItem = voice.items[i + 1] as Duration;
                var normalItem = voice.items[i + 2] as Duration;
                
                Assert.IsNotNull(dotItem);
                Assert.IsNotNull(halfItem);
                Assert.IsNotNull(normalItem);
                
                Assert.AreEqual(Length.Eighth, halfItem.length);
                Assert.AreEqual(0, halfItem.dotCount);
                
                Assert.AreEqual(Length.Quarter, dotItem.length);
                Assert.AreEqual(1, dotItem.dotCount);

                Assert.AreEqual(Length.Quarter, normalItem.length);
                Assert.AreEqual(0, normalItem.dotCount);
            }
        }

        [TestMethod]
        public void RhythmCount()
        {
            var abc = "L:1/2\nC>>D C>>>D";

            var tune = Tune.Load(abc);
            
            Assert.AreEqual(1, tune.voices.Count);

            var voice = tune.voices[0];
            Assert.AreEqual(4, voice.items.Count);

            var note1 = voice.items[0] as Duration;
            Assert.IsNotNull(note1);
            Assert.AreEqual(Length.Half, note1.length);
            Assert.AreEqual(2, note1.dotCount);
            
            var note2 = voice.items[1] as Duration;
            Assert.IsNotNull(note2);
            Assert.AreEqual(Length.Eighth, note2.length);
            Assert.AreEqual(0, note2.dotCount);
            
            var note3 = voice.items[2] as Duration;
            Assert.IsNotNull(note3);
            Assert.AreEqual(Length.Half, note3.length);
            Assert.AreEqual(3, note3.dotCount);
            
            var note4 = voice.items[3] as Duration;
            Assert.IsNotNull(note4);
            Assert.AreEqual(Length.Sixteenth, note4.length);
            Assert.AreEqual(0, note4.dotCount);
        }

        [TestMethod]
        public void Invalid()
        {
            var invalidAbc = new List<string>()
            {
                ">C", "C>|", "C>>[v:2]"
            };

            foreach (var abc in invalidAbc)
                Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }
    }
}