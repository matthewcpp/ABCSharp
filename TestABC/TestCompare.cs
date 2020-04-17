using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestCompare
    {
        [TestMethod]
        public void ChordElements()
        {
            Chord.Element[] pitchElements = new Chord.Element[5]
            {
                new Chord.Element(Pitch.B6, Accidental.Unspecified),
                new Chord.Element(Pitch.C5, Accidental.Unspecified),
                new Chord.Element(Pitch.G3, Accidental.Unspecified),
                new Chord.Element(Pitch.D2, Accidental.Unspecified),
                new Chord.Element(Pitch.C4, Accidental.Unspecified)
            };

            var expectedPitchOrder = new List<Pitch>()
            {
                Pitch.D2, Pitch.G3, Pitch.C4, Pitch.C5, Pitch.B6
            };
            
            Array.Sort(pitchElements);

            for (int i = 0; i < expectedPitchOrder.Count; i++)
                Assert.AreEqual(expectedPitchOrder[i], pitchElements[i].pitch);
        }
    }
}