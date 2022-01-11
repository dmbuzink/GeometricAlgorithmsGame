namespace DefaultNamespace
{
    public class PolygonSegment<T> : Segment where T : SimplePolygon
    {
        public T Polygon { set; get; }

        public PolygonSegment(Vertex start, Vertex end, T polygon): base(start, end)
        {
            this.Polygon = polygon;
        }
    }
}