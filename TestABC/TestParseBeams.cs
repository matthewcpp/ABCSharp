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
            var items = tune.voices[0].items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[1].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], tune.beams[i]);
            }
        }

        [TestMethod]
        public void BeamsEighthAndSixteenthNotesWithNoWhitespace()
        {
            var abc = "[L:1/8]aac2[L:1/16]bbc4";

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var items = tune.voices[0].items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[1].id), new Beam(items[3].id, items[4].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], tune.beams[i]);
            }
        }

        [TestMethod]
        public void BeamInterruptions()
        {
            var abc = "L:1/8\ncccc c cc[cde]c cczc cc|c";

            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var items = tune.voices[0].items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[0].id, items[3].id), 
                new Beam(items[5].id, items[8].id), 
                new Beam(items[9].id, items[10].id), 
                new Beam(items[13].id, items[14].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], tune.beams[i]);
            }
        }

        [TestMethod]
        public void BackquotedBeams()
        {
            var abc = "L:1/8\nA2``````````B``C";
            var tune = Tune.Load(abc);
            Assert.AreEqual(1, tune.voices.Count);
            var items = tune.voices[0].items;

            var expectedBeams = new List<Beam>()
            {
                new Beam(items[1].id, items[2].id)
            };

            for (int i = 0; i < expectedBeams.Count; i++) {
                Assert.AreEqual(expectedBeams[i], tune.beams[i]);
            }
        }
    }
}