using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Edge
    {
        public PolygonVertex StartPoint { get; set; }
        public PolygonVertex EndPoint { get; set; }

        public Edge(PolygonVertex startPoint, PolygonVertex endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        /// <summary>
        /// Get the distance of a line to the camera at an angle
        /// </summary>
        /// <param name="camera">The position of the camera</param>
        /// <param name="angle">The angle from the camera</param>
        /// <returns>The distance of the line segment at the angle</returns>
        public double DistanceAt(Vertex camera, double angle)
        {
            Vertex intersection = GetAngleIntersection(angle, camera);

            double xdistance = intersection.X - camera.X;
            double ydistance = intersection.Y - camera.Y;
            return Math.Sqrt(xdistance * xdistance + ydistance * ydistance);
        }

        /// <summary>
        /// Get the intersection on the line of this edge given an angle and the position of the camera
        /// </summary>
        /// <param name="angle">The angle from the camera</param>
        /// <param name="camera">The position of the camera</param>
        /// <returns>A vertex of the position of the intersection</returns>
        public PolygonVertex GetAngleIntersection(double angle, Vertex camera)
        {
            double cos = Math.Sin(angle * Math.PI);
            double sin = Math.Cos(angle * Math.PI);

            Edge line1 = this;
            Edge line2 = new Edge(new PolygonVertex(camera.X, camera.Y, new List<PolygonVertex>()), new PolygonVertex(camera.X + 1000 * sin, camera.Y + 1000 * cos, new List<PolygonVertex>()));

            var xdiff = (line1.StartPoint.X - line1.EndPoint.X, line2.StartPoint.X - line2.EndPoint.X);
            var ydiff = (line1.StartPoint.Y - line1.EndPoint.Y, line2.StartPoint.Y - line2.EndPoint.Y);


            //Computes the determinant between two vectors
            double Det((double, double) a, (double, double) b)
            {
                return a.Item1 * b.Item2 - a.Item2 * b.Item1;
            }

            var div = Det(xdiff, ydiff);

            var d = (Det((line1.StartPoint.X, line1.StartPoint.Y), (line1.EndPoint.X, line1.EndPoint.Y)), Det((line2.StartPoint.X, line2.StartPoint.Y), (line2.EndPoint.X, line2.EndPoint.Y)));
            var x = Det(d, xdiff) / div;
            var y = Det(d, ydiff) / div;

            //Rather ugly, but seems to solve some of the annoying rounding issues
            if (x > 16331239353195370 / 10000) x = 0;
            if (y > 16331239353195370 / 10000) y = 0;

            return new PolygonVertex(x, y, new List<PolygonVertex>());
        }
    }
}