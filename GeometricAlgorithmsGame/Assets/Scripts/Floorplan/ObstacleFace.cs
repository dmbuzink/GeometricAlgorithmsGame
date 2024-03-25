using System.Collections.Generic;

namespace DefaultNamespace
{
    public class ObstacleFace : SimplePolygon
    {
        public ObstacleFace(IEnumerable<Vertex> points) : base(points)
        {
        }
    }
}