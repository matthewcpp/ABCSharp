using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public enum Cleff
    {
        Treble,
        Bass
    }

    public class Voice
    {
        public string identifier { get; }
        public string name { get; set; }
        public Cleff cleff { get; set; } = Cleff.Treble;
        public List<Item> items { get; } = new List<Item>();

        public Voice(string identifier)
        {
            this.identifier = identifier;
        }
    }
}
