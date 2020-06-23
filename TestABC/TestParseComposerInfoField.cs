using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseComposerInfoField
    {
        [TestMethod]
        public void ParseInfoField()
        {
            var expectedComposer = "Matthew LaRocca";
            var abc = $"X:1\nC:{expectedComposer}";

            var tune = Tune.Load(abc);
            Assert.AreEqual(expectedComposer, tune.header.composer);
        }

        [TestMethod]
        public void CannotAppearInTuneBody()
        {
            var abc = "CDEF | GABc\nC:error";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }

        [TestMethod]
        public void CannotAppearInline()
        {
            var abc = "CDEF | [C:bogus] GABc";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }
    }
}