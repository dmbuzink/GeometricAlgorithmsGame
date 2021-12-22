using System.Collections.Generic;
using System.Linq;

public class SimplePolygon
{
    public Vertex[] Vertices { get; private set; }

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
    public SimplePolygon(Vertex[] points)
    {
        this.Vertices = points;
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
            pairs.Add((Vertices.ElementAt(i), Vertices.ElementAt(i % Vertices.Length)));
        }
        return pairs;
    }
}
