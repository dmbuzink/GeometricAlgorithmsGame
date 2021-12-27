using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SimplePolygon : IEnumerable<Vertex>
{
    public Vertex[] Vertices { get; private set; }

    private float? _maxX;

    /// <summary>
    /// Creates a new simple polygon given a list of alternating x and y coordinates
    /// </summary>
    /// <param name="points">The coordinates of the points of the polygon, in counter clockwise order</param>
    public SimplePolygon(double[] points)
    {
        this.Vertices = new Vertex[points.Length / 2];
        for (int i = 0; i < points.Length-1; i += 2)
            Vertices[i / 2] = new Vertex(points[i], points[i + 1]);
    }

    /// <summary>
    /// Creates a new simple polygon given a list of vertices
    /// </summary>
    /// <param name="points">The vertices points of the polygon, in counter clockwise order</param>
    public SimplePolygon(IEnumerable<Vertex> points)
    {
        this.Vertices = points.ToArray();
    }

    /// <summary>
    /// Get the vertices pair-wise. So with vertices: v1 -> v2 -> v3 -> v1.
    /// You get: (v1, v2), (v2, v3), (v3, v1)
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(Vertex v1, Vertex v2)> GetVerticesPairWise()
    { 
        var pairs = new List<(Vertex v1, Vertex v2)>();
        for (var i = 0; i < Vertices.Length; i++)
        {
            pairs.Add((Vertices.ElementAt(i), Vertices.ElementAt((i + 1) % Vertices.Length)));
        }
        return pairs;
    }

    public IEnumerator<Vertex> GetEnumerator() => this.Vertices.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.Vertices.GetEnumerator();

    /// <summary>
    /// Asynchronously checks to see if the given point is in the polygon.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public async Task<bool> PointIsWithinPolygonAsync(Vertex point)
    {
        // TODO: Remove this reference and write in the report something about this.
        // Based on solution: https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/

        var startPointAsVector2 = new Vector2(point.Xf, point.Yf);
        var endPointAsVector2 = new Vector2(GetMaxXOfPolygon() + 1, point.Yf);
        
        var lineSegmentOfPoint = new Util.Geometry.LineSegment(startPointAsVector2, endPointAsVector2);

        var intersectionTasks = GetVerticesPairWise()
            .Select(pair => Task.Run(() =>
            {
                var polygonLineSegment = new Util.Geometry.LineSegment(pair.v1.ToVector2(), pair.v2.ToVector2());
                return Util.Geometry.LineSegment.Intersect(lineSegmentOfPoint, polygonLineSegment);
            }));

        var intersectionsPoints = await Task.WhenAll(intersectionTasks);
        var numberOfIntersections = intersectionsPoints.Count(intersection => intersection != null);

        return numberOfIntersections % 2 == 1;
    }

    /// <summary>
    /// Gets the maximum X coordinates of the polygon.
    /// </summary>
    /// <returns></returns>
    public float GetMaxXOfPolygon()
    {
        if (this._maxX is null)
        {
            this._maxX = this.Max(v => v.Xf);
        }

        return _maxX.Value;
    }
}
