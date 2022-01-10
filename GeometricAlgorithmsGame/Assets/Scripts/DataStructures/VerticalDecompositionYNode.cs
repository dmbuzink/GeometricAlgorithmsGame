using System;

namespace DefaultNamespace
{
    public class VerticalDecompositionYNode<T> : VerticalDecompositionNode<T> where T : Segment
    {
        private Segment Line { get; set; }

        public VerticalDecompositionNode<T> Bottom { get; set; }
        public VerticalDecompositionNode<T> Top { get; set; }

        /// <summary>
        /// Creates a y decision node
        /// </summary>
        /// <param name="line">The bottom to top oriented segment to use</param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        public VerticalDecompositionYNode(
            Segment line,
            VerticalDecompositionNode<T> bottom,
            VerticalDecompositionNode<T> top
        )
        {
            this.Line = line;
            this.Bottom = bottom;
            this.Top = top;
        }

        public override Trapezoid<T> FindTrapezoid(Vertex point)
        {
            int side = this.Line.GetSideOfPoint(point);
            if (side == -1) 
                return this.Top.FindTrapezoid(point);
            else 
                return this.Bottom.FindTrapezoid((point));
        }
        public override Trapezoid<T> FindTrapezoid(Segment segment)
        {
            bool below = this.IsLineBelow(segment);
            if (below)
                return this.Bottom.FindTrapezoid(segment);
            else
                return this.Top.FindTrapezoid(segment);
        }

        private bool IsLineBelow(Segment segment)
        {
            int thisLeftSide = segment.GetSideOfPoint(this.Line.StartPoint);
            int thisRightSide = segment.GetSideOfPoint(this.Line.EndPoint);

            if (thisLeftSide == thisRightSide) return thisLeftSide == -1;
            else if (thisLeftSide == 0) return thisRightSide == -1;

            int segmentLeftSide = this.Line.GetSideOfPoint(segment.StartPoint);
            int segmentRightSide = this.Line.GetSideOfPoint(segment.EndPoint);

            if (segmentLeftSide == segmentRightSide) return segmentLeftSide == 1;
            else if (segmentLeftSide == 0) return segmentRightSide == 1;
            else if (thisRightSide == 0) return thisLeftSide == -1;
            else if (segmentRightSide == 0) return segmentLeftSide == 1;
            else
            {
                throw new ArgumentException("Segments are not allowed to cross in a vertical decomposition");
            }
        }

        public override void Replace(
            VerticalDecompositionNode<T> oldNode,
            VerticalDecompositionNode<T> newNode
        )
        {
            if (this.Top == oldNode) this.Top = newNode;
            if (this.Bottom == oldNode) this.Bottom = newNode;
        }

        public override void LinkNodes(VerticalDecompositionNode<T> parent)
        {
            this.Top.LinkNodes(this);
            this.Bottom.LinkNodes(this);
        }
    }
}