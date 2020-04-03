﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ABC;

namespace TestABC
{
    [TestClass]
    public class TestParseVoiceInfoField
    {
        [TestMethod]
        public void Identifier()
        {
            var tune = Tune.Load("V:test");

            Assert.AreEqual(1, tune.voices.Count);
            Assert.AreEqual("test", tune.voices[0].identifier);
        }

        [TestMethod]
        public void MultipleVoices()
        {
            var abc = @"
            V:1
            V:2
            ";

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);
            Assert.AreEqual("1", tune.voices[0].identifier);
            Assert.AreEqual("2", tune.voices[1].identifier);
        }

        [TestMethod]
        public void SetsClef()
        {
            var abc = @"
            V:1
            V:2 clef=treble
            V:3 clef = bass
            ";

            var tune = Tune.Load(abc);
            
            Assert.AreEqual(3, tune.voices.Count);
            Assert.AreEqual(Cleff.Treble, tune.voices[0].cleff);
            Assert.AreEqual(Cleff.Treble, tune.voices[1].cleff);
            Assert.AreEqual(Cleff.Bass, tune.voices[2].cleff);
        }

        [TestMethod]
        public void SetsName()
        {
            var abc = @"
            V:1 name=test
            V:2 name=""two words""";

            var tune = Tune.Load(abc);
            
            Assert.AreEqual(2, tune.voices.Count);
            Assert.AreEqual("test", tune.voices[0].name);
            Assert.AreEqual("two words", tune.voices[1].name);
        }

        [TestMethod]
        public void CreateInTuneBody()
        {
            var abc = @"
            X:1
            [V:1] C
            [V:2 clef=bass] D";

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);

            Assert.AreEqual(Cleff.Treble, tune.voices[0].cleff);
            NoteItem noteItem = tune.voices[0].items[0] as NoteItem;
            Assert.AreEqual(Note.Value.C4, noteItem.note.value);

            Assert.AreEqual(Cleff.Bass, tune.voices[1].cleff);
            noteItem = tune.voices[1].items[0] as NoteItem;
            Assert.AreEqual(Note.Value.D4, noteItem.note.value);
        }

        [TestMethod]
        public void ReferenceInTuneBody()
        {
            var abc = @"
            V:1
            V:2 clef=bass
            [V:1] C
            [V:2] D";

            var tune = Tune.Load(abc);

            Assert.AreEqual(2, tune.voices.Count);

            Assert.AreEqual(Cleff.Treble, tune.voices[0].cleff);
            NoteItem noteItem = tune.voices[0].items[0] as NoteItem;
            Assert.AreEqual(Note.Value.C4, noteItem.note.value);

            Assert.AreEqual(Cleff.Bass, tune.voices[1].cleff);
            noteItem = tune.voices[1].items[0] as NoteItem;
            Assert.AreEqual(Note.Value.D4, noteItem.note.value);
        }
    }
}
