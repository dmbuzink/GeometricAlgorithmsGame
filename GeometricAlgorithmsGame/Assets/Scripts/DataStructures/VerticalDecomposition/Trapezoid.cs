
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        
        public override void GetTrapezoids(List<Trapezoid<T>> trapezoids)
        {
            trapezoids.Add(this);
        }

        /// <summary>
        /// Computes the area of this trapezoid
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            if (this.Left == null || this.Right == null || this.Bottom == null || this.Top == null)
            {
                return 0;
            }

            double topLeftY = this.GetYCoordinate(this.Left.X, this.Top);
            double topRightY = this.GetYCoordinate(this.Right.X, this.Top);
            double bottomLeftY = this.GetYCoordinate(this.Left.X, this.Bottom);
            double bottomRightY = this.GetYCoordinate(this.Right.X, this.Bottom);

            double leftHeight = topLeftY - bottomLeftY;
            double rightHeight = topRightY - bottomRightY;
            double width = Right.X - Left.X;

            double area = width * (leftHeight + rightHeight) / 2;
            //if (area < 0)
            //{
            //    Debugger.Break();
            //}
            return area;
        }

        protected double GetYCoordinate(double x, Segment line)
        {
            double dx = line.EndPoint.X - line.StartPoint.X;
            double dy = line.EndPoint.Y - line.StartPoint.Y;

            return (x - line.StartPoint.X) / dx * dy + line.StartPoint.Y;
        }
    }
}
