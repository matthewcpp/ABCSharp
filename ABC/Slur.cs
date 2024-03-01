using System;

namespace ABC
{
    public class Slur : IEquatable<Slur>, IComparable<Slur> {
        public enum Type {Slur, Tie};

        public int start {get; private set;}
        public int end {get; private set;}

        public Slur.Type type {get; private set;}

        public Slur(Slur.Type type, int start, int end) {
            this.type = type;
            this.start = start;
            this.end = end;
        }

        public bool Equals(Slur other) {
            if (other == null)
            {
                return false;
            }

            return type == other.type && start == other.start && end == other.end;
        }

        public int CompareTo(Slur other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            } 

            return start.CompareTo(other.start);
        }
    }
}