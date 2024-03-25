namespace DefaultNamespace
{
    public class FaceEdgeEvent<F> : FaceEvent where F : SimplePolygon
    {
        /// <summary>
        /// The end of the segment
        /// </summary>
        public Vertex End;

        /// <summary>
        /// The polygon that the event originates from
        /// </summary>
        public F Source;

        /// <summary>
        /// Whether the boundary was a left or right wall of the source polygon. If when crossing this boundary from left to right result to being within the polygon, it's a left wall.
        /// </summary>
        public bool IsLeftWall;

        public FaceEdgeEvent(Vertex start, Vertex end, bool isLeftWall, F source): base(start)
        {
            this.End = end;
            this.IsLeftWall = isLeftWall;
            this.Source = source;
        }
    }
}