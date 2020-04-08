using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseTitleInfoField
    {
        [TestMethod]
        public void ParseInfoField()
        {
            var expectedTitle = "Hello World";
            var abc = $"X:1\nT:{expectedTitle}";

            var tune = Tune.Load(abc);
            Assert.AreEqual(expectedTitle, tune.title);
        }

        [TestMethod]
        public void CannotAppearInTuneBody()
        {
            var abc = "CDEF | GABc\nT:error";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }

        [TestMethod]
        public void CannotAppearInline()
        {
            var abc = "CDEF | [T:bogus] GABc";
            Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); });
        }
    }
}