using System;
using UnityEngine;

namespace DefaultNamespace
{
    public static class GeometricHelper
    {
        /// <summary>
        /// Gets the angle between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AngleBetweenPoints(Vector3 a, Vector3 b) => 
            Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        
        /// <summary>
        /// Gets the angle between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double AngleBetweenPoints(Vertex a, Vertex b) =>
            AngleBetweenPointsRad(a, b) * Mathf.Rad2Deg;

        /// <summary>
        /// Get the angle between two points in radians
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The angle in radians</returns>
        public static double AngleBetweenPointsRad(Vertex a, Vertex b) =>
            Mathf.Atan2((float)(a.Y - b.Y), (float)(a.X - b.X));

        /// <summary>
        /// Gets the distance between two points.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetDistanceBetweenTwoPoints(Vertex a, Vertex b) =>
            Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        /// <summary>
        /// Computes the determinant between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The determinant</returns>
        public static double Determinant((double, double) a, (double, double) b) =>
            a.Item1 * b.Item2 - a.Item2 * b.Item1;

        /// <summary>
        /// Computes the determinant between two vertices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The determinant</returns>
        public static double Determinant(Vertex a, Vertex b) =>
            Determinant((a.X, a.Y), (b.X, b.Y));
    }
}