using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EdgeDistanceComparer : IComparer<Edge>
{
    private readonly Vertex camera;
    private double angle;

    public EdgeDistanceComparer(Vertex camera, double startAngle)
    {
        this.camera = camera;
        this.angle = startAngle;
    }

    public void SetAngle(double angle)
    {
        this.angle = angle;
    }

    public int Compare(Edge a, Edge b)
    {
        if (a == null)
        {
            return int.MinValue;
        }

        if (b == null)
        {
            return int.MaxValue;
        }

        //Return 0 if they are the same
        if (EdgesAreEqual(a, b))
        {
            return 0;
        }

        double distanceEdge1 = a.DistanceAt(camera, angle);
        double distanceEdge2 = b.DistanceAt(camera, angle);

        if (distanceEdge1 == distanceEdge2)
        {
            return a.ToString().CompareTo(b.ToString());
        }

        if (distanceEdge1 < distanceEdge2)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    public bool EdgesAreEqual(Edge a, Edge b)
    {
        return (a.StartPoint.SamePositionAs(b.StartPoint) &&
             a.EndPoint.SamePositionAs(b.EndPoint));
    }
}