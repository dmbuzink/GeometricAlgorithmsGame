namespace DefaultNamespace
{
    public class FaceEvent
    {
        /// <summary>
        /// The unique ID of this event to be used for sorting
        /// </summary>
        public int ID;

        /// <summary>
        /// The point at which this even takes place
        /// </summary>
        public Vertex Point;

        public FaceEvent(Vertex point)
        {
            this.Point = point;
            this.ID = maxID++;
        }

        private static int maxID = 0;
    }
}