using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ABC
{
    public class Tune
    {
        public TuneHeader header { get; } = new TuneHeader();
        public string title { get { return header.title; } }
        public List<Voice> voices { get; } = new List<Voice>();
        
        /// <summary>
        /// Maps an item to a list of decorations which have been attached to it.
        /// </summary>
        public Dictionary<int, List<string>> decorations { get; } = new Dictionary<int, List<string>>();

        /// <summary>
        /// Loads a Tune from a string
        /// </summary>
        public static Tune Load(string text)
        {
            var Parser = new Parser();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return Parser.Parse(stream);
            }
        }

        /// <summary>
        /// Loads a tune from a stream.
        /// </summary>
        public static Tune Load(Stream stream)
        {
            if (!stream.CanRead)
                throw new ParseException("Unable to read from supplied stream.");

            var Parser = new Parser();
            return Parser.Parse(stream);
        }

        /// <summary>
        /// Gets a voice with the given identifier.
        /// </summary>
        /// <returns>Voice object which has the supplied identifier, or null if no voice was found.</returns>
        public Voice FindVoice(string identifier)
        {
            return voices.Find((Voice v) => { return v.identifier == identifier; });
        }
    }
}
