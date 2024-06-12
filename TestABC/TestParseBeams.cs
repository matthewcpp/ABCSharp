using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseBeams
    {
        [TestMethod]
        public void ChordBeam()
        {
            var abc = "L:1/8\n[CEG][CEG] [CEG]";
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            var items = voice.items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[1].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], voice.beams[i]);
            }
        }

        [TestMethod]
        public void BeamsEighthAndSixteenthNotesWithNoWhitespace()
        {
            var abc = "[L:1/8]aac2[L:1/16]bbc4";

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            var items = voice.items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[1].id), new Beam(items[3].id, items[4].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], voice.beams[i]);
            }
        }

        [TestMethod]
        public void BeamInterruptions()
        {
            var abc = "L:1/8\ncccc c cc[cde]c cczc cc|c";

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            var items = voice.items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[3].id), 
                new Beam(items[5].id, items[8].id), 
                new Beam(items[9].id, items[10].id), 
                new Beam(items[13].id, items[14].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], voice.beams[i]);
            }
        }

        [TestMethod]
        public void BackquotedBeams()
        {
            var abc = "L:1/8\nA2``````````B``C";
            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            var items = voice.items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[1].id, items[2].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], voice.beams[i]);
            }
        }

        [TestMethod]
        public void TestGetItems()
        {
            var abc = @"
            V:1
            V:2
            L:1/4
            [V:1] A/A/ A C/`D/`E/`F/ 
            [V:2] A/`B/`C/`D/ F F";

            var tune = Tune.Load(abc);
            Assert.AreEqual(2, tune.voices.Count);

            var v1 = tune.voices[0];
            var v2 = tune.voices[1];

            var sliceArray = (Voice v, int start, int length) => {
                var slice = new List<Duration>();
                for (int i = 0; i < length; i++) {
                    slice.Add(v.items[start + i] as Duration);
                }
                return slice;
            };

            Assert.AreEqual(2, v1.beams.Count);
            CollectionAssert.AreEqual(sliceArray(v1, 0, 2), v1.GetItems(v1.beams[0]));
            CollectionAssert.AreEqual(sliceArray(v1, 3, 4), v1.GetItems(v1.beams[1]));

            Assert.AreEqual(1, v2.beams.Count);
            CollectionAssert.AreEqual(sliceArray(v2, 0, 4), v2.GetItems(v2.beams[0]));
        }
    }
}