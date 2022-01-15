namespace DefaultNamespace
{
    public class FaceCrossEvent<F> : FaceEvent where F : SimplePolygon
    {
        /// <summary>
        /// The interval whose boundaries are crossing at this event
        /// </summary>
        public FaceInterval<F> Interval;

        public FaceCrossEvent(FaceInterval<F> interval, Vertex point) : base(point)
        {
            this.Interval = interval;
        }
    }
}
