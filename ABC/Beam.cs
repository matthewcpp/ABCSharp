using System;
using System.Collections.Generic;
using System.Numerics;

namespace ABC 
{
    public class Beam : IEquatable<Beam>
    {
        /// <summary>
        /// Item Id of the stat of the beam
        /// </summary>
        public int startId {get;}

        /// <summary>
        /// Item Id of the end of the beam
        /// </summary>
        public int endId {get; internal set;}

        private Voice voice;

        public Beam(Voice voice, int startId, int endId)
        {
            this.voice = voice;
            this.startId = startId;
            this.endId = endId;
        }

        public List<Duration> items {get => GetItems();}

        private List<Duration> GetItems()
        {
            var index = voice.GetItemIndex(startId);

            if (index == -1) {
                return null;
            }

            var result = new List<Duration>();

            while(true) {
                var duration = items[index++] as Duration;

                result.Add(duration);

                if (duration.id == endId) {
                    break;
                }
            }

            return result;
        }

        public bool Equals(Beam other) {
            if (other == null)
            {
                return false;
            }

            return voice == other.voice && startId == other.startId && endId == other.endId;
        }

        public override int GetHashCode()
        {
            
            int hash =  System.HashCode.Combine(voice, startId, endId);
            return hash;
        }

        public override string ToString()
        {
            return $"[Beam start: {startId} end: {endId}]";
        }
    }
}