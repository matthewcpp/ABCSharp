using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;
using NuGet.Frameworks;

namespace TestABC
{
    [TestClass]
    public class TestParseTie
    {
        [TestMethod]
        public void ParseTie()
        {
            var tests = new string[] {
                "c4-c4", /* tie two notes */
                "abc-|cba", /* tie across bar */
                "[CEG]-[CEG]", /* tie chords */
            };

            var expectedTies = new Slur[] {
                new Slur(Slur.Type.Tie, 0, 1),
                new Slur(Slur.Type.Tie, 2, 4),
                new Slur(Slur.Type.Tie, 0, 1),
            };

            for (int i = 0; i < tests.Length; i++)
            {
                var tune = Tune.Load(tests[i]);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                Assert.AreEqual(1, voice.slurs.Count);
                Assert.AreEqual(expectedTies[i], voice.slurs[0], $"Tie {i} mismatch");
            }
        }
    }
}