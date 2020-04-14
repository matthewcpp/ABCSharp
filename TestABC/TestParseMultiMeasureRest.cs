using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseMultiMeasureRest
    {
        [TestMethod]
        public void ParseMeasureCount()
        {
            var abc = "ZZ1Z2XX2X10";
            
            var expectedCounts = new List<int>()
            {
                1,1,2,1,2,10
            };

            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedCounts.Count, voice.items.Count);

            for (int i = 0; i < expectedCounts.Count; i++)
            {
                var restItem = voice.items[i] as MultiMeasureRestItem;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedCounts[i], restItem.rest.count);
            }
        }

        [TestMethod]
        public void ParseRestVisibility()
        {
            var abc = "ZX";
            
            var expectedVisibilities = new List<bool>()
            {
                true, false
            };
            
            var tune = Tune.Load(abc);

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];
            
            Assert.AreEqual(expectedVisibilities.Count, voice.items.Count);

            for (int i = 0; i < expectedVisibilities.Count; i++)
            {
                var restItem = voice.items[i] as MultiMeasureRestItem;
                Assert.IsNotNull(restItem);
                
                Assert.AreEqual(expectedVisibilities[i], restItem.rest.isVisible);
            }
        }
        
        [TestMethod]
        public void CannotAppearInChords()
        {
            var abc = "[CEZ]";

            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc);});
        }
    }
}