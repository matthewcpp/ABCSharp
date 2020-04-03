using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseReferenceNumberInfoField
    {
        [TestMethod]
        public void ParseReferenceNumber()
        {
            var tune = Tune.Load("X:100");
            Assert.AreEqual(100U, tune.referenceNumber);

            tune = Tune.Load("X: 100");
            Assert.AreEqual(100U, tune.referenceNumber);
        }

        [TestMethod]
        public void InvalidReferenceNumber()
        {
            Assert.ThrowsException<ParseException>(() => Tune.Load("X:ZZZ"));
        }
    }
}
