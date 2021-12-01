using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public class PolygonVertex : Vertex
    {
        public IEnumerable<PolygonVertex> SuccessorVertices { get; set; }
        
        public PolygonVertex(double x, double y, IEnumerable<PolygonVertex> successorVertices) : base(x, y)
        {
            this.SuccessorVertices = successorVertices.ToArray();
        }
    }
}