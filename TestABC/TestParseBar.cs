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
        public void StandardBarTypes()
        {
            var standardBars = new Dictionary<string, Bar.Kind>()
            {
                {"|", Bar.Kind.Line}, {"||", Bar.Kind.DoubleLine}, 
                {"[|", Bar.Kind.Start}, {"|]", Bar.Kind.Final},
                {"|:", Bar.Kind.RepeatStart}, {":|", Bar.Kind.RepeatEnd}, 
                {":|:", Bar.Kind.RepeatEndStart}, {":||:", Bar.Kind.RepeatEndStart}
            };

            foreach (var standardBar in standardBars)
            {
                var abc = $"X:1\nL:1/4\nK:C\nCCCC{standardBar.Key}";
                var tune = Tune.Load(abc);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                
                Assert.AreEqual(5, voice.items.Count);
                var bar = voice.items[4] as Bar;
                
                Assert.IsNotNull(bar);
                Assert.AreEqual(standardBar.Value, bar.kind, standardBar.Key);
            }
        }
        

        [TestMethod]
        public void TestCustomBar()
        {
            var customBars = new List<string>()
            {
                "[|:::", "|[|"
            };
            
            foreach (var customBar in customBars)
            {
                var abc = $"X:1\nL:1/4\nK:C\nCCCC{customBar}";
                var tune = Tune.Load(abc);
                
                Assert.AreEqual(1, tune.voices.Count);
                var voice = tune.voices[0];
                
                Assert.AreEqual(5, voice.items.Count);
                var bar = voice.items[4] as CustomBar;
                
                Assert.IsNotNull(bar);
                Assert.AreEqual(customBar, bar.str, customBar);
            }
        }
    }
}
