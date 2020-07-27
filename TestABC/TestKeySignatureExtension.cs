using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestKeySignatureExtension
    {
        [TestMethod]
        public void IsSharp()
        {
            var expectedSharps = new List<KeySignature>()
            {
                KeySignature.Sharps0,
                KeySignature.CMajor,
                KeySignature.AMinor,
        
                KeySignature.Sharps1,
                KeySignature.GMajor,
                KeySignature.EMinor,
        
                KeySignature.Sharps2,
                KeySignature.DMajor,
                KeySignature.BMinor,
        
                KeySignature.Sharps3,
                KeySignature.AMajor,
                KeySignature.FSharpMinor,
        
                KeySignature.Sharps4,
                KeySignature.EMajor,
                KeySignature.CSharpMinor,
        
                KeySignature.Sharps5,
                KeySignature.BMajor,
                KeySignature.GSharpMinor,
        
                KeySignature.Sharps6,
                KeySignature.FSharpMajor,
                KeySignature.DSharpMinor,
        
                KeySignature.Sharps7,
                KeySignature.CSharpMajor,
                KeySignature.ASharpMinor,
            };

            foreach (var expectedSharp in expectedSharps)
            {
                Assert.IsTrue(expectedSharp.IsSharp());
                Assert.IsFalse(expectedSharp.IsFlat());
            }
        }

        [TestMethod]
        public void IsFlat()
        {
            var expectedFlats = new List<KeySignature>()
            {
                KeySignature.Flats0,
        
                KeySignature.Flats1,
                KeySignature.FMajor,
                KeySignature.DMinor,
        
                KeySignature.Flats2,
                KeySignature.BFlatMajor,
                KeySignature.GMinor,
        
                KeySignature.Flats3,
                KeySignature.EFlatMajor,
                KeySignature.CMinor,
        
                KeySignature.Flats4,
                KeySignature.AFlatMajor,
                KeySignature.FMinor,
        
                KeySignature.Flats5,
                KeySignature.DFlatMajor,
                KeySignature.BFlatMinor,
        
                KeySignature.Flats6,
                KeySignature.GFlatMajor,
                KeySignature.EFlatMinor,
        
                KeySignature.Flats7,
                KeySignature.CFlatMajor,
                KeySignature.AFlatMinor
            };

            foreach (var expectedFlat in expectedFlats)
            {
                Assert.IsTrue(expectedFlat.IsFlat());
                Assert.IsFalse(expectedFlat.IsSharp());
            }
        }

        [TestMethod]
        public void None()
        {
            Assert.IsFalse(KeySignature.None.IsFlat());
            Assert.IsFalse(KeySignature.None.IsSharp());
        }
    }
}