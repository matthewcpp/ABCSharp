using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public enum Clef
    {
        Treble,
        Bass
    }

    public class Voice
    {
        public string identifier { get; }
        public string name { get; set; }
        public Clef clef { get; set; } = Clef.Treble;
        public List<Item> items { get; } = new List<Item>();

        public Voice(string identifier)
        {
            this.identifier = identifier;
        }
    }
}
