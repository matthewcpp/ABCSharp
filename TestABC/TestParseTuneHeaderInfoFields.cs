using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseTuneHeaderInfoFields
    {
        [TestMethod]
        public void TitleInfoField()
        {
            var expectedTitle = "Hello World";
            var abc = $"X:1\nT:{expectedTitle}";

            var tune = Tune.Load(abc);
            Assert.AreEqual(expectedTitle, tune.title);
        }

        [TestMethod]
        public void ComposerInfoField()
        {
            var expectedComposer = "Matthew LaRocca";
            var abc = $"X:1\nC:{expectedComposer}";

            var tune = Tune.Load(abc);
            Assert.AreEqual(expectedComposer, tune.header.composer);
        }

        [TestMethod]
        public void ParseReferenceNumber()
        {
            var tune = Tune.Load("X:100");
            Assert.AreEqual("100", tune.header.referenceNumber);

            tune = Tune.Load("X: 100");
            Assert.AreEqual("100", tune.header.referenceNumber);
        }

        [TestMethod]
        public void InvalidReferenceNumber()
        {
            Assert.ThrowsException<ParseException>(() => Tune.Load("X:ZZZ"));
        }

        [TestMethod]
        public void CannotAppearInTuneBody()
        {
            var infoFields = new List<string>()
            {
                "T:Hello World",
                "C:Matthew LaRocca",
                "X:1"
            };

            foreach (var infoField in infoFields)
            {
                var abc = $"CDEF | GABc\n{infoField}";
                Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); }, infoField);
            }
        }

        [TestMethod]
        public void CannotAppearInline()
        {
            var infoFields = new List<string>()
            {
                "T:Hello World",
                "C:Matthew LaRocca",
                "X:1"
            };

            foreach (var infoField in infoFields)
            {
                var abc = $"CDEF | [{infoField}] GABc";
                Assert.ThrowsException<ParseException>(() => { Tune.Load(abc); }, infoField);
            }
        }

        [TestMethod]
        public void HeaderFieldsNullIfNotSet()
        {
            var abc = "X:1\nK:C\nCCCC|";
            var tune = Tune.Load(abc);
            
            Assert.IsNotNull(tune.header.referenceNumber);
            Assert.IsNull(tune.header.title);
            Assert.IsNull(tune.header.composer);
        }
    }
}
