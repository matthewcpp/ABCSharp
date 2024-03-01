using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;
using NuGet.Frameworks;

namespace TestABC
{
    [TestClass]
    public class TestParseSlur
    {
        [TestMethod]
        public void ParseStartEnd()
        {
            var abc = "(CCC)";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.slurs.Count);
            var slur = voice.slurs[0];

            Assert.AreEqual(0, slur.start);
            Assert.AreEqual(2, slur.end);
        }

        [TestMethod]
        public void ParseNested()
        {
            var abc = "C(CCC| (CC) (CC) | CCCC)";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            List<Slur> expectedSlurs = new List<Slur>(){
                new Slur(Slur.Type.Slur, 1, 13), new Slur(Slur.Type.Slur, 5, 6), new Slur(Slur.Type.Slur,7, 8)
            };

            Assert.AreEqual(expectedSlurs.Count, voice.slurs.Count);

            for (int i = 0; i < expectedSlurs.Count; i++) {
                Assert.AreEqual(expectedSlurs[i], voice.slurs[i], $"Slur mismatch: {i}");
            }
        }

        [TestMethod]
        public void ParseMismatched() {
            var abc = "C(CC))C";

            Assert.ThrowsException<ParseException>(() => {Tune.Load(abc);});
        }

        [TestMethod]
        public void ParseUnterminated() {
            var abc = "CC(CC";

            Assert.ThrowsException<ParseException>(() => {Tune.Load(abc);});
        }

        [TestMethod]
        public void ParseMultiVoice() {
            var abc = @"
            X:1
            [V:1 clef=treble] C(CCC |
            [V:2 clef=bass] D(DDD) |
            [V:1] CCCC) |
            [V:2] DDDD |";

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);

            var treble = tune.voices[0];
            Assert.AreEqual(1, treble.slurs.Count);
            Assert.AreEqual(new Slur(Slur.Type.Slur, 1, 9), treble.slurs[0]);

            var bass = tune.voices[1];
            Assert.AreEqual(1, bass.slurs.Count);
            Assert.AreEqual(new Slur(Slur.Type.Slur, 1, 3), bass.slurs[0]);
        }

        [TestMethod]
        public void ParseStartAndEndOnSameNote()
        {
            var abc = "(c d (e) f g a)";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            List<Slur> expectedSlurs = new List<Slur>(){
                new Slur(Slur.Type.Slur, 0, 2), new Slur(Slur.Type.Slur, 2, 5)
            };

            Assert.AreEqual(expectedSlurs.Count, voice.slurs.Count);

            for (int i = 0; i < expectedSlurs.Count; i++) {
                Assert.AreEqual(expectedSlurs[i], voice.slurs[i], $"Slur mismatch: {i}");
            }
        }
    }
}