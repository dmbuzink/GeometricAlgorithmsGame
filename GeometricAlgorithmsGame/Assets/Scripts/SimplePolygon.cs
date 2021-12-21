
using System.Collections;

public class SimplePolygon : IEnumerable
{
    public Vertex[] vertices { get; private set; }

    /// <summary>
    /// Creates a new simple polygon given a list of alternating x and y coordinates
    /// </summary>
    /// <param name="points">The coordinates of the points of the polygon, in counter clockwise order</param>
    public SimplePolygon(double[] points)
    {
        this.vertices = new Vertex[points.Length / 2];
        for (int i = 0; i < points.Length-1; i += 2)
            vertices[i / 2] = new Vertex(points[i], points[i + 1]);
    }

    /// <summary>
    /// Creates a new simple polygon given a list of vertices
    /// </summary>
    /// <param name="points">The vertices points of the polygon, in counter clockwise order</param>
    public SimplePolygon(Vertex[] points)
    {
        this.vertices = points;
    }
    

    IEnumerator IEnumerable.GetEnumerator()
    {
        return vertices.GetEnumerator();
    }
}
