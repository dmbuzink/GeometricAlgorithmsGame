using System.Collections.Generic;

namespace DefaultNamespace
{
    public class OuterFace : SimplePolygon
    {
        public OuterFace(IEnumerable<Vertex> points) : base(points)
        {
        }
    }
}