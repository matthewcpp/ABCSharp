using System;

namespace ABC
{
    public class Tie : Grouping, IEquatable<Tie>, IComparable<Tie> {
        
        public Tie(int startId, int endId) : base(startId, endId)
        {

        }

        public bool Equals(Tie other) {
            if (other == null)
            {
                return false;
            }

            return startId == other.startId && endId == other.endId;
        }

        public int CompareTo(Tie other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            } 

            return startId.CompareTo(other.startId);
        }
    }
}