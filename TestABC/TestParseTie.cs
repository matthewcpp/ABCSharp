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

            var expectedTiesIndices = new Tuple<int, int>[] {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(2, 4),
                new Tuple<int, int>(0, 1)
            };

            for (int i = 0; i < tests.Length; i++)
            {
                var tune = Tune.Load(tests[i]);

                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];

                var expectedStartItem = voice.items[expectedTiesIndices[i].Item1];
                var expectedEndItem = voice.items[expectedTiesIndices[i].Item2];
                var expectedSlur = new Slur(Slur.Type.Tie, expectedStartItem.id, expectedEndItem.id);

                Assert.AreEqual(1, voice.slurs.Count);
                Assert.AreEqual(expectedSlur, voice.slurs[0], $"Tie {i} mismatch");
            }
        }
    }
}