
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Trapezoid<T>: VerticalDecompositionNode<T> where T: Segment
    {
        public Vertex Left { get; set; }
        public Vertex Right { get; set; }
        public T Top { get; set; }
        public T Bottom { get; set; }

        public Trapezoid<T> RTN { get; set; }
        public Trapezoid<T> LTN { get; set; }
        public Trapezoid<T> RBN { get; set; }
        public Trapezoid<T> LBN { get; set; }

        public readonly List<VerticalDecompositionNode<T>> Parents = new List<VerticalDecompositionNode<T>>();

        public Trapezoid(Vertex left, Vertex right, T bottom, T top)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }
        public override Trapezoid<T> FindTrapezoid(Vertex point)
        {
            return this;
        }

        public override Trapezoid<T> FindTrapezoid(Segment segment)
        {
            return this;
        }


        public override void Replace(
            VerticalDecompositionNode<T> oldNode,
            VerticalDecompositionNode<T> newNode
        ) {}

        public override void LinkNodes(VerticalDecompositionNode<T> parent)
        {
            if(!this.Parents.Contains(parent)) this.Parents.Add(parent);
        }
    }
}
