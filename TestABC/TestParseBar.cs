using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;
namespace TestABC
{
    [TestClass]
    public class TestParseBar
    {

        [TestMethod]
        public void BarLine()
        {
            var tune = Tune.Load("|");

            Assert.AreEqual(1, tune.voices.Count);
            var voice = tune.voices[0];

            Assert.AreEqual(1, voice.items.Count);
            Assert.AreEqual(Item.Type.Bar, voice.items[0].type);
            var barItem = voice.items[0] as BarItem;

            Assert.AreEqual(Bar.Type.Line, barItem.bar.type);
        }
    }
}
