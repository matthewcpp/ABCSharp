using System;

namespace ABC
{
    public class Grouping {
        /// <summary>
        /// Item Id of the stat of the group
        /// </summary>
        public int startId {get; private set;}

        /// <summary>
        /// Item Id of the end of the group
        /// </summary>
        public int endId {get; private set;}

        public Grouping( int startId, int endId) {
            this.startId = startId;
            this.endId = endId;
        }
    }
}