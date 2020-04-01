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
        string name { get; set; }
        public Cleff cleff { get; set; } = Cleff.Treble;
        public List<Item> items { get; } = new List<Item>();
    }
}
