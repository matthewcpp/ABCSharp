using System;

namespace ABC
{
    public class Slur : IEquatable<Slur>, IComparable<Slur> {
        public enum Type {Slur, Tie};

        /// <summary>
        /// Item Id of the stat of the slur
        /// </summary>
        public int startId {get; private set;}

        /// <summary>
        /// Item Id of the end of the slur
        /// </summary>
        public int endId {get; private set;}

        public Slur.Type type {get; private set;}

        public Slur(Slur.Type type, int startId, int endId) {
            this.type = type;
            this.startId = startId;
            this.endId = endId;
        }

        public bool Equals(Slur other) {
            if (other == null)
            {
                return false;
            }

            return type == other.type && startId == other.startId && endId == other.endId;
        }

        public int CompareTo(Slur other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            } 

            return startId.CompareTo(other.startId);
        }
    }
}