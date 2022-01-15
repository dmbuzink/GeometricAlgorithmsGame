using System.Collections.Generic;
using JetBrains.Annotations;

namespace DefaultNamespace
{
    /// <summary>
    /// A y-monotone polygon section and connected sections that make up an entire polygon
    /// </summary>
    /// <typeparam name="F">The sources that formed this polygon</typeparam>
    public class MonotonePolygonSection<F> where F: SimplePolygon
    {
        /// <summary>
        /// The points forming the left wall of this section
        /// </summary>
        public List<Vertex> Left = new List<Vertex>();

        /// <summary>
        /// The points forming the right wall of this section
        /// </summary>
        public List<Vertex> Right = new List<Vertex>();

        /// <summary>
        /// The sources that this polygon is derived from
        /// </summary>
        public List<F> Sources;

        /// <summary>
        /// The connected section at the bottom left of this polygon section
        /// </summary>
        [CanBeNull] public MonotonePolygonSection<F> BottomLeft;
        
        /// <summary>
        /// The connected section at the bottom right of this polygon section
        /// </summary>
        [CanBeNull] public MonotonePolygonSection<F> BottomRight;

        /// <summary>
        /// The connected section at the top left of this polygon section
        /// </summary>
        [CanBeNull] public MonotonePolygonSection<F> TopLeft;

        /// <summary>
        /// The connected section at the top right of this polygon section
        /// </summary>
        [CanBeNull] public MonotonePolygonSection<F> TopRight;

        public MonotonePolygonSection(List<F> sources)
        {
            this.Sources = sources;
        }
    }
}