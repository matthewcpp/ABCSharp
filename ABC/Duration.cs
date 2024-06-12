using System.Dynamic;

namespace ABC
{
    public abstract class Duration : Item
    {
        public Length length { get; set; }
        public int dotCount { get; set; }
        public Beam beam {get; internal set;} = null;

        public float baseLengthDuration => ParserUtil.lengthDurations[length];

        public float duration
        {
            get
            {
                float totalDuration = baseLengthDuration;
                float dotDuration = totalDuration * 0.5f;
                
                for (int i = 0; i < dotCount; i++)
                {
                    totalDuration += dotDuration;
                    dotDuration *= 0.5f;
                }

                return totalDuration;
            }
        }

        protected Duration(Item.Type type) : base(type)
        {
        }
    }
}