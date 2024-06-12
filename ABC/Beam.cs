using System;
using System.Collections.Generic;

namespace ABC 
{
    public class Beam : IEquatable<Beam>, IComparable<Beam>
    {
        /// <summary>
        /// Item Id of the stat of the beam
        /// </summary>
        public int startId {get;}

        /// <summary>
        /// Item Id of the end of the beam
        /// </summary>
        public int endId {get; internal set;}

        public Beam(int startId, int endId)
        {
            this.startId = startId;
            this.endId = endId;
        }

        public bool Equals(Beam other) {
            if (other == null)
            {
                return false;
            }

            return startId == other.startId && endId == other.endId;
        }

        public int CompareTo(Beam other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            } 

            return startId.CompareTo(other.startId);
        }

        public override int GetHashCode()
        {
            
            int hash =  System.HashCode.Combine(startId, endId);
            return hash;
        }

        public override string ToString()
        {
            return $"[Beam start: {startId} end: {endId}]";
        }
    }
}