using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseKeySignatureInfoField
    {
        [TestMethod]
        public void TestParseMajorKeys()
        {
            var categories = new List<Dictionary<KeySignature, List<string>>>()
            {
                //sharps - major
                new Dictionary<KeySignature, List<string>>()
                {
                    {KeySignature.CMajor, new List<string>() {"C", "C Major", " CmaJ"}},
                    {KeySignature.GMajor, new List<string>() {"G", "G Major", "G\tmAj"}},
                    {KeySignature.DMajor, new List<string>() {"D", "D    Major", "DmAj\t"}},
                    {KeySignature.AMajor, new List<string>() {"A", "A\tMajor", "  Amajor"}},
                    {KeySignature.EMajor, new List<string>() {"E", "E MaJor", " E major"}},
                    {KeySignature.BMajor, new List<string>() {"B", "B Major", "Bmaj"}},
                    {KeySignature.FSharpMajor, new List<string>() {"F#", "F#\tMajor", "F#MAJOR"}},
                    {KeySignature.CSharpMajor, new List<string>() {"C#", "C# Major", "C#MaJOR\t"}},
                },
            
                //flats - major
                new Dictionary<KeySignature, List<string>>()
                {
                    {KeySignature.FMajor, new List<string>() {"F", "F Major", "FMAJOR"}},
                    {KeySignature.BFlatMajor, new List<string>() {"Bb", "Bb\tMajor", "Bb\tmAJOR"}},
                    {KeySignature.EFlatMajor, new List<string>() {"Eb", "Eb\tMajor", "\tEbmAJ"}},
                    {KeySignature.AFlatMajor, new List<string>() {"Ab", "Ab Major", "AbMaj"}},
                    {KeySignature.DFlatMajor, new List<string>() {"Db", "Db MAJOR", "  DbMajor"}},
                    {KeySignature.GFlatMajor, new List<string>() {"Gb", "Gb MAJOR", "Gb Maj"}},
                    {KeySignature.CFlatMajor, new List<string>() {"Cb", "Cb MAJOR", "Cb   "}},
                }
            };

            foreach (var category in categories)
            {
                foreach (var testCase in category)
                {
                    foreach (var testString in testCase.Value)
                    {
                        var abc = $"K: {testString}";
                        var tune = Tune.Load(abc);
                    
                        Assert.AreEqual(1, tune.voices.Count);
                        var voice = tune.voices[0];

                        Assert.AreEqual(testCase.Key, voice.initialKey, $"{testCase.Key}: {abc}");
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestParseMinorKeys()
        {
            var categories = new List<Dictionary<KeySignature, List<string>>>()
            {
                //sharps - major
                new Dictionary<KeySignature, List<string>>()
                {
                    {KeySignature.AMinor, new List<string>() {"Am", "Aminor",}},
                    {KeySignature.EMinor, new List<string>() {"Em", "E minor"}},
                    {KeySignature.BMinor, new List<string>() {"Bm", "B MIN",}},
                    {KeySignature.FSharpMinor, new List<string>() {"F#m", "F#MINOR "}},
                    {KeySignature.CSharpMinor, new List<string>() {"C#m", "C#\tmin",}},
                    {KeySignature.GSharpMinor, new List<string>() {"G#m", "G# M"}},
                    {KeySignature.DSharpMinor, new List<string>() {"D#m", "D#\tM"}},
                    {KeySignature.ASharpMinor, new List<string>() {"A#m", "A#min"}},
                },
            
                //flats - major
                new Dictionary<KeySignature, List<string>>()
                {
                    {KeySignature.DMinor, new List<string>() {"Dm", "D minor"}},
                    {KeySignature.GMinor, new List<string>() {"Gm", "G\tminor "}},
                    {KeySignature.CMinor, new List<string>() {"Cm", "Cminor"}},
                    {KeySignature.FMinor, new List<string>() {"Fm", "F\tmInOr"}},
                    {KeySignature.BFlatMinor, new List<string>() {"Bbm", "BbM"}},
                    {KeySignature.EFlatMinor, new List<string>() {"Ebm", "Eb\tMIN"}},
                    {KeySignature.AFlatMinor, new List<string>() {"Abm", "AbMINOR"}},
                }
            };

            foreach (var category in categories)
            {
                foreach (var testCase in category)
                {
                    foreach (var testString in testCase.Value)
                    {
                        var abc = $"K: {testString}";
                        var tune = Tune.Load(abc);
                    
                        Assert.AreEqual(1, tune.voices.Count);
                        var voice = tune.voices[0];

                        Assert.AreEqual(testCase.Key, voice.initialKey, $"{testCase.Key}: {abc}");
                    }
                }
            }
        }

        [TestMethod]
        public void TestParseNone()
        {
            var inputs = new List<string>()
            {
                "K:",
                "K:   \t",
                "K: none",
                "K:NONE"
            };

            foreach (var input in inputs)
            {
                var tune = Tune.Load(input);
                Assert.AreEqual(1, tune.voices.Count);
                Assert.AreEqual(KeySignature.None, tune.voices[0].initialKey);
            }
        }

        [TestMethod]
        public void InformationFieldSetsForAllVoices()
        {
            var abc = @"
            X:1
            T:Test
            V:1
            V:2
            K:C
            [V:1] C
            [V:2] C,
            K:Eb";

            var tune = Tune.Load(abc);
            Assert.AreEqual(2, tune.voices.Count);
            foreach (var voice in tune.voices)
            {
                Assert.AreEqual(KeySignature.CMajor, voice.initialKey);
                Assert.AreEqual(2, voice.items.Count);
                var key = voice.items[1] as Key;
                Assert.IsNotNull(key);
                Assert.AreEqual(KeySignature.EFlatMajor, key.signature);
            }
        }

        [TestMethod]
        public void InlineInformationFieldSetsActiveVoice()
        {
            var abc = @"
            X:1
            T:Test
            V:1
            V:2
            K:D
            [V:1] C [K:Eb]
            [V:2] C,";
            
            var tune = Tune.Load(abc);
            
            Assert.AreEqual(2, tune.voices.Count);
            var voice = tune.voices[0];
            Assert.AreEqual(KeySignature.DMajor, voice.initialKey);
            Assert.AreEqual(2, voice.items.Count);
            var key = voice.items[1] as Key;
            Assert.IsNotNull(key);
            Assert.AreEqual(KeySignature.EFlatMajor, key.signature);

            voice = tune.voices[1];
            Assert.AreEqual(KeySignature.DMajor, voice.initialKey);
            Assert.AreEqual(1, voice.items.Count);
            Assert.AreEqual(Item.Type.Note, voice.items[0].type);
        }

        [TestMethod]
        public void InvalidKeySignatures()
        {
            var inputs = new List<string>()
            {
                "K: Zm",
                "K: C bogus",
                "K: M"
            };

            foreach (var input in inputs)
                Assert.ThrowsException<ParseException>(() => { Tune.Load(input);});
        }
    }
}