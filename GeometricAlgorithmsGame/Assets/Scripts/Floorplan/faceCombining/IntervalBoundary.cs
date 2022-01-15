namespace DefaultNamespace
{
    public class IntervalBoundary<F>: Segment where F: SimplePolygon
    {
        /// <summary>
        /// The source polygon that contains this segment
        /// </summary>
        public F Source;

        /// <summary>
        /// A unique ID of this boundary that can be used for sorting
        /// </summary>
        public int ID;

        /// <summary>
        /// Whether the boundary was a left or right wall of the source polygon. If when crossing this boundary from left to right result to being within the polygon, it's a left wall.
        /// </summary>
        public bool IsLeftWall;

        public IntervalBoundary(
            Vertex start,
            Vertex end,
            F source,
            bool isLeftWall
        ): base(start, end)
        {
            this.Source = source;
            this.IsLeftWall = isLeftWall;

            this.ID = maxID++;
        }

        private static int maxID = 0;
    }
}