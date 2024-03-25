using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Vertex
{
    public double X { get; set; }
    public float Xf => (float) X;
    public double Y { get; set; }
    public float Yf => (float) Y;
    
    public Vertex(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    public (double x, double y) GetCoordinates() => (X, Y);

    public Vector3 ToVector3() => new Vector3(Xf, Yf);
    public Vector2 ToVector2() => new Vector2(Xf, Yf);

    /// <summary>
    /// Return -1 if to the left, 0 on the line or 1 if to the right.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public async Task<int> GetSideOfLine(Vertex startPoint, Vertex endPoint) =>
        // Making use of the determinant with vectors: startPoint -> endPoint, startPoint -> pointInQuestion
        await Task.Run(() => Math.Sign((endPoint.X - startPoint.X) * (Y - startPoint.Y) -
                                            (endPoint.Y - startPoint.Y) * (X - startPoint.X)));

    /// <summary>
    /// Returns true if the difference between the x coordinates and y coordinates is under epsilon
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public bool SamePositionAs(Vertex v) => 
        Math.Abs(this.X - v.X) <= Mathf.Epsilon && Math.Abs(this.Y - v.Y) <= Mathf.Epsilon;

    /// <summary>
    /// Returns true if the vertices are completely identical
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public bool Equals(Vertex v) => v != null && this.X == v.X && this.Y == v.Y;

    /// <summary>
    /// Creates a deep copy of the vertex
    /// </summary>
    /// <returns></returns>
    public Vertex Copy() => new Vertex(X, Y);
}
