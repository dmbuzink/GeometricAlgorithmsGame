
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

            // Below invariants tests help debugging
            if (this.Left!=null)
            {
                double lx = this.Left.X;
                if (this.Top!=null && this.Top.StartPoint.X > lx && this.Top.EndPoint.X > lx)
                    Debug.Log("Trapezoid error 1");
                if (this.Bottom != null && this.Bottom.StartPoint.X > lx && this.Bottom.EndPoint.X > lx)
                    Debug.Log("Trapezoid error 2");
            }
            if (this.Right != null)
            {
                double rx = this.Right.X;
                if (this.Top != null && this.Top.StartPoint.X < rx && this.Top.EndPoint.X < rx)
                    Debug.Log("Trapezoid error 3");
                if (this.Bottom != null && this.Bottom.StartPoint.X < rx && this.Bottom.EndPoint.X < rx)
                    Debug.Log("Trapezoid error 4");
            }
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
            if(!trapezoids.Contains(this))
                trapezoids.Add(this);
        }

        /// <summary>
        /// Computes the area of this trapezoid
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            double fb = 10000000000; // Arbitrary value to essentially make trapezoids without boundaries infinitely large
            double leftX = this.Left?.X ?? -fb;
            double rightX = this.Right?.X ?? fb;
            double topLeftY = this.Top == null ? fb : this.GetYCoordinate(leftX, this.Top);
            double topRightY = this.Top == null ? fb : this.GetYCoordinate(rightX, this.Top);
            double bottomLeftY = this.Bottom == null ? -fb : this.GetYCoordinate(leftX, this.Bottom);
            double bottomRightY = this.Bottom == null ? -fb : this.GetYCoordinate(rightX, this.Bottom);

            double leftHeight = topLeftY - bottomLeftY;
            double rightHeight = topRightY - bottomRightY;
            double width = Right.X - Left.X;

            double area = width * (leftHeight + rightHeight) / 2;


            if (area < 0)
            {
                Debug.Log("neg");
            }
            return area;
        }

        protected double GetYCoordinate(double x, Segment line)
        {
            double dx = line.EndPoint.X - line.StartPoint.X;
            double dy = line.EndPoint.Y - line.StartPoint.Y;

            return (x - line.StartPoint.X) / dx * dy + line.StartPoint.Y;
        }

        /// <summary>
        /// Retrieves the polygon representing this trapezoid
        /// </summary>
        /// <returns></returns>
        public SimplePolygon CalculatePolygon()
        {
            double fb = 100; // Arbitrary value, should be larger than all real coordinates
            double leftX = this.Left?.X ?? -fb;
            double rightX = this.Right?.X ?? fb;
            double topLeftY = this.Top == null ? fb : this.GetYCoordinate(leftX, this.Top);
            double topRightY = this.Top == null ? fb : this.GetYCoordinate(rightX, this.Top);
            double bottomLeftY = this.Bottom == null ? -fb : this.GetYCoordinate(leftX, this.Bottom);
            double bottomRightY = this.Bottom == null ? -fb : this.GetYCoordinate(rightX, this.Bottom);

            return new SimplePolygon(new[]
            {
                leftX, bottomLeftY,
                rightX, bottomRightY,
                rightX, topRightY,
                leftX, topLeftY
            });
        }
    }
}
