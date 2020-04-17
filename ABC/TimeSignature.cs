using System;

namespace ABC
{
    public class TimeSignature : Item
    {
        public string value { get; }

        public TimeSignature(string value) : base(Item.Type.TimeSignature)
        {
            this.value = value;
        }
    }
}