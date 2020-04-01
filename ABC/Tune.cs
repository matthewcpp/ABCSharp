using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ABC
{
    public class Tune
    {
        public uint referenceNumber { get; set; } = 1;
        public List<Voice> voices { get; } = new List<Voice>();

        public static Tune Load(string text)
        {
            var Parser = new Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return Parser.Parse(stream);
            }
        }
    }
}
