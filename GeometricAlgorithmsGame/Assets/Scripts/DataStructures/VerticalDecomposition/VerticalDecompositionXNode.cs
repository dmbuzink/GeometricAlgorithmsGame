using System.Collections.Generic;

namespace DefaultNamespace
{
    public class VerticalDecompositionXNode<T>:VerticalDecompositionNode<T> where T : Segment
    {
        private Vertex Point { get; set; }

        public VerticalDecompositionNode<T> Left { get; set; }
        public VerticalDecompositionNode<T> Right { get; set; }

        public VerticalDecompositionXNode(
            Vertex point, 
            VerticalDecompositionNode<T> left,
            VerticalDecompositionNode<T> right
        )
        {
            this.Point = point;
            this.Left = left;
            this.Right = right;
        }

        public override Trapezoid<T> FindTrapezoid(Vertex point)
        {
            if (point.X > this.Point.X || (point.X == this.Point.X && point.Y >= this.Point.Y))
            {
                return this.Right.FindTrapezoid(point);
            }
            else
            {
                return this.Left.FindTrapezoid(point);
            }
        }
        public override Trapezoid<T> FindTrapezoid(Segment segment)
        {
            Vertex point = segment.StartPoint;

            if (point.X > this.Point.X || (point.X == this.Point.X && point.Y >= this.Point.Y))
            {
                return this.Right.FindTrapezoid(segment);
            }
            else
            {
                return this.Left.FindTrapezoid(segment);
            }
        }
        
        public override void Replace(
            VerticalDecompositionNode<T> oldNode,
            VerticalDecompositionNode<T> newNode
        )
        {
            if (this.Left == oldNode) this.Left = newNode;
            if (this.Right == oldNode) this.Right = newNode;
        }

        public override void LinkNodes(VerticalDecompositionNode<T> parent)
        {
            this.Left.LinkNodes(this);
            this.Right.LinkNodes(this);
        }

        public override void GetTrapezoids(List<Trapezoid<T>> trapezoids)
        {
            this.Left.GetTrapezoids(trapezoids);
            this.Right.GetTrapezoids(trapezoids);
        }
    }
}