using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class Note : Duration, IComparable<Note>
    {
        public Pitch pitch { get; set; }
        public Accidental accidental { get; set; }

        public Note(Pitch pitch = Pitch.C4, Length length = Length.Eighth, Accidental accidental = Accidental.Unspecified, int dotCount = 0) 
            : base(Item.Type.Note)
        {
            this.pitch = pitch;
            this.length = length;
            this.accidental = accidental;
            this.dotCount = dotCount;
        }
        
        public override bool Equals(object obj)
        {
            return obj is Note note &&
                   pitch == note.pitch &&
                   accidental == note.accidental &&
                   length == note.length &&
                   dotCount == note.dotCount;
        }
        
        public override int GetHashCode()
        {
            var hashCode = 838805952;
            hashCode = hashCode * -1521134295 + pitch.GetHashCode();
            hashCode = hashCode * -1521134295 + accidental.GetHashCode();
            hashCode = hashCode * -1521134295 + length.GetHashCode();
            hashCode = hashCode * -1521134295 + dotCount.GetHashCode();
            return hashCode;
        }
        
        public override string ToString()
        {
            return $"{pitch.ToString()}, {accidental.ToString()}, {length.ToString()}";
        }

        public int CompareTo(Note other)
        {
            return pitch.CompareTo(other.pitch);
        }
    }
}
