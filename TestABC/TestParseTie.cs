using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

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
                var expectedTie = new Tie(expectedStartItem.id, expectedEndItem.id);

                Assert.AreEqual(1, voice.ties.Count);
                Assert.AreEqual(expectedTie, voice.ties[0], $"Tie {i} mismatch");
            }
        }
    }
}