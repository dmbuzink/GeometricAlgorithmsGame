using System.Collections.Generic;
using JetBrains.Annotations;

namespace DefaultNamespace
{
    public class FaceInterval<F> where F: SimplePolygon
    {
        /// <summary>
        /// The left wall of this boundary, can be null if it's the left most interval
        /// </summary>
        [CanBeNull] public IntervalBoundary<F> Left;

        /// <summary>
        /// The right wall of this boundary, can be null if it's the right most interval
        /// </summary>
        [CanBeNull] public IntervalBoundary<F> Right;

        /// <summary>
        /// The shape that has been accumulated throughout the existence of this interval
        /// </summary>
        public MonotonePolygonSection<F> Shape;

        /// <summary>
        /// The event representing when this interval's current boundaries cross, if they will cross
        /// </summary>
        [CanBeNull] public FaceCrossEvent<F> Intersection;

        public FaceInterval(IntervalBoundary<F> left, IntervalBoundary<F> right, List<F> sources)
        {
            this.Left = left;
            this.Right = right;

            this.Shape = new MonotonePolygonSection<F>(sources);
        }
    }
}