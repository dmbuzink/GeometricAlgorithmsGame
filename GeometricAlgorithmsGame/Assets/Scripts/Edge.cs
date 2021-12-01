namespace DefaultNamespace
{
    public class Edge
    {
        public PolygonVertex StartPoint { get; set; }
        public PolygonVertex EndPoint { get; set; }

        public Edge(PolygonVertex startPoint, PolygonVertex endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
    }
}