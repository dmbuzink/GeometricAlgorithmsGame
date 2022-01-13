using System;
using System.Collections.Generic;
using System.Numerics;
using Util.Geometry;

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
            try
            {
                Vertex intersection = GetAngleIntersection(angle, camera);

                double xdistance = intersection.X - camera.X;
                double ydistance = intersection.Y - camera.Y;
                return Math.Sqrt(xdistance * xdistance + ydistance * ydistance);
            }
            catch(Exception ex)
            {
                //Should not happen
                return double.MaxValue;
            }
        }

        /// <summary>
        /// Get the intersection on the line of this edge given an angle and the position of the camera
        /// </summary>
        /// <param name="angle">The angle from the camera</param>
        /// <param name="camera">The position of the camera</param>
        /// <returns>A vertex of the position of the intersection</returns>
        public Vertex GetAngleIntersection(double angle, Vertex camera)
        {
            //Rotate the angle 90 degrees
            var rotated = angle - Math.PI / 2;

            double sin = Math.Sin(rotated);
            double cos = Math.Cos(rotated);

            //Create a line segment for the camera with an somewhat arbitrary length
            //The line segment should be larger than any floorplan will be, so just take a large value
            var cameraLineSegment = new LineSegment(camera.ToVector2(), new Vertex(camera.X + 10000 * sin, camera.Y - 10000 * cos).ToVector2());
            var edgeLineSegment = new LineSegment(StartPoint.ToVector2(), EndPoint.ToVector2());

            var result = LineSegment.Intersect(cameraLineSegment, edgeLineSegment);
            if (!result.HasValue)
            {
                //Intersection not found (null since this is used to determine the start intersections of the sweepline)
                return null;
            }

            return new Vertex(result.Value.x, result.Value.y);
       }
    }
}