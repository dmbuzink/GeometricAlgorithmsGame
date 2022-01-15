using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class CombinedFace<F> : SimplePolygon where F: SimplePolygon
    {
        public List<F> Sources;

        public CombinedFace(List<F> sources, IEnumerable<Vertex> points) : base(points)
        {
            this.Sources = sources;
        }
    }
}