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
            Mathf.Atan2((float) (a.Y - b.Y),  (float) (a.X - b.X)) * Mathf.Rad2Deg;
    }
}