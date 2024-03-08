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

            Assert.AreEqual(voice.items[0].id, slur.startId);
            Assert.AreEqual(voice.items[2].id, slur.endId);
        }

        [TestMethod]
        public void ParseNested()
        {
            var abc = "C(CCC| (CC) (CC) | CCCC)";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            List<Slur> expectedSlurs = new List<Slur>(){
                new Slur(Slur.Type.Slur, voice.items[1].id, voice.items[13].id), 
                new Slur(Slur.Type.Slur, voice.items[5].id, voice.items[6].id), 
                new Slur(Slur.Type.Slur, voice.items[7].id, voice.items[8].id)
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
            Assert.AreEqual(new Slur(Slur.Type.Slur, treble.items[1].id, treble.items[9].id), treble.slurs[0]);

            var bass = tune.voices[1];
            Assert.AreEqual(1, bass.slurs.Count);
            Assert.AreEqual(new Slur(Slur.Type.Slur, bass.items[1].id, bass.items[3].id), bass.slurs[0]);
        }

        [TestMethod]
        public void ParseStartAndEndOnSameNote()
        {
            var abc = "(c d (e) f g a)";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            List<Slur> expectedSlurs = new List<Slur>(){
                new Slur(Slur.Type.Slur, voice.items[0].id, voice.items[2].id), 
                new Slur(Slur.Type.Slur, voice.items[2].id, voice.items[5].id)
            };

            Assert.AreEqual(expectedSlurs.Count, voice.slurs.Count);

            for (int i = 0; i < expectedSlurs.Count; i++) {
                Assert.AreEqual(expectedSlurs[i], voice.slurs[i], $"Slur mismatch: {i}");
            }
        }

        [TestMethod]
        public void ParseSlurAfterLineBreak()
        {
            var abc = @"
            X:1
            M:C
            L:1/4
            K:C

            CDEF |

            (DFAc | cAFD) |";

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.slurs.Count);

            var expectedSlur = new Slur(Slur.Type.Slur, voice.items[6].id, voice.items[14].id);
            Assert.AreEqual(expectedSlur, voice.slurs[0]);
        }
    }
}