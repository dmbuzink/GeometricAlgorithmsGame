using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Vertex
{
    public double X { get; set; }
    public double Y { get; set; }
    
    public Vertex(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    public (double x, double y) GetCoordinates() => (X, Y);
}
