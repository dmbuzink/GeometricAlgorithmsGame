using System;

namespace DefaultNamespace
{
    public class Segment
    {
        public Vertex StartPoint { get; }
        public Vertex EndPoint { get; }
        

        public Segment(Vertex startPoint, Vertex endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }


        /// <summary>
        /// Returns -1 if point is to the left, 0 on the line or 1 if to the right.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int GetSideOfPoint(Vertex point) =>
            // Making use of the determinant with vectors: startPoint -> endPoint, startPoint -> pointInQuestion
            -Math.Sign((EndPoint.X - StartPoint.X) * (point.Y - StartPoint.Y) -
                                           (EndPoint.Y - StartPoint.Y) * (point.X - StartPoint.X));
        /// <summary>
        /// Returns -1 if the given line is to the left of this line, 0 if the lines are collinear or 1 if to the right.
        /// When lines cross the side of the start point is returned
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetSideOfLine(Segment line)
        {
            int startSide = this.GetSideOfPoint(line.StartPoint);
            if (startSide != 0) return startSide;
            return this.GetSideOfPoint(line.EndPoint);
        }
        
        public bool Intersects(Segment line)
        {
            return line.GetSideOfPoint(this.StartPoint) * line.GetSideOfPoint(this.EndPoint) < 0 &&
                   this.GetSideOfPoint(line.StartPoint) * this.GetSideOfPoint(line.EndPoint) < 0;
        }

        /// <summary>
        /// Retrieves this same line but possibly flipped, such that it's oriented form left to right, or bottom to top if it's vertical
        /// </summary>
        /// <returns></returns>
        public Segment GetOriented()
        {
            if (
                this.StartPoint.X < this.EndPoint.X ||
                (this.StartPoint.X == this.EndPoint.X && this.StartPoint.Y < this.EndPoint.Y)
            ) 
                return this;
            else 
                return new Segment(this.EndPoint, this.StartPoint);
        }
    }
}