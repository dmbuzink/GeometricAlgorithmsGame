using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Camera : MonoBehaviour
{
    public event Action<Camera> onSelected;
    [SerializeField] private BoxCollider2D _selectionCollider;
    
    public Vertex Position { get; set; }
    public double Angle
    {
        get => this._angle;
        set
        {
            if (value < 0 || value >= 360)
            {
                throw new ArgumentException(
                    "The direction of the camera should be between 0 (inclusive) and 360 (exclusive)");
            }

            this._angle = value;
        }
    }
    private double _angle; // <- Should not be used directly, but should only be used by the Angle property.

    //Angle of view of the camera, half of it on both sides of the viewing angle.
    private const double _angleOfView = 90;

    public Camera(Vertex position, double angle = 0)
    {
        this.Position = position;
        this._angle = angle;
    }

    // Start is called before the first frame update
    void Start()
    {
        // this._selectionCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Calculates the the are that can be viewed by the camera based on the floorplan
    /// </summary>
    /// <param name="floorplan"></param>
    /// <returns></returns>
    public IEnumerable<PolygonVertex> CalculateView(Floorplan floorplan)
    {
        //Method is sometimes called while the postion is not yet set, just return null for now
        if(Position == null)
        {
            return null;
        }

        //These will cause issues if the the angle of view causes the min/max angle to flip arround 0/360 degrees.
        //Rotate it 90 degrees since this script assumes 0 degrees to to the dead right instead of up (Might want to change this to make it consistent)
        double minAngle = ((Angle + 90) % 360 - _angleOfView / 2) * Mathf.Deg2Rad;
        double maxAngle = ((Angle + 90) % 360 + _angleOfView / 2) * Mathf.Deg2Rad;

        //We first need a list of all edges
        var edges = floorplan.SimplePolygon
            .GetVerticesPairWise()
            .Select(x => new Edge(new PolygonVertex(x.v1.X, x.v1.Y, new List<PolygonVertex>()), new PolygonVertex(x.v2.X, x.v2.Y, new List<PolygonVertex>())));

        //Get all vertices, group them on their angle to the camera and sort them on their angle
        var GroupedVertices = edges
                .SelectMany(x => new List<Tuple<Vertex, Edge>>() { new Tuple<Vertex, Edge>(x.StartPoint, x), new Tuple<Vertex, Edge>(x.EndPoint, x) })
                .GroupBy(x => GeometricHelper.AngleBetweenPointsRad(Position, x.Item1))
                .Select(x => new Tuple<double, List<Tuple<Vertex, Edge>>>(x.Key, x.ToList()))
                .ToList();

        //Add the min and max angles as events since these should be the first and last vertex of the result
        GroupedVertices.Add(new Tuple<double, List<Tuple<Vertex, Edge>>>(minAngle, new List<Tuple<Vertex, Edge>>()));
        GroupedVertices.Add(new Tuple<double, List<Tuple<Vertex, Edge>>>(maxAngle, new List<Tuple<Vertex, Edge>>()));

        GroupedVertices = GroupedVertices.OrderBy(x => x.Item1).ToList();

        //Initialize the bbst NOTE: this currently still just is a list which gets sorted after each operation, obviously not efficient yet.
        List<Edge> bbst = new List<Edge>();
        List<PolygonVertex> result = new List<PolygonVertex>();

        //Initialization, find edges that intersect the start of the sweepline, we add these edges to the bbst at the start.
        foreach(var edge in edges)
        {
            int startQuadrant = GetQuadrant(edge.StartPoint);
            int endQuadrant = GetQuadrant(edge.EndPoint);

            if (startQuadrant == 1 || endQuadrant == 1)
            {
                if(endQuadrant == 4 || startQuadrant == 4)
                {
                    //Edge intersects the start of the linesweep
                    //Add the vertex that starts in the 4th quadrant in the initialized bbst
                    bbst.Add(edge);
                }

                if(startQuadrant == 3 || endQuadrant == 3)
                {
                    //line goes from quadrant 1 to 3, we have to make sure it goes below the camera,
                    //if that is the case, then the start of the sweepline also intersects it.

                    Vertex start = startQuadrant == 1 ? edge.StartPoint : edge.EndPoint;
                    Vertex end = startQuadrant == 1 ? edge.EndPoint : edge.StartPoint;

                    //Start is q1, end in q3, so if the camera is to the left of this line then the line goes below the camera
                    if(new Vertex(Position.X, Position.Y).GetSideOfLine(start, end).Result == -1)
                    {
                        bbst.Add(edge);
                    }
                }
            }
        }

        //Sort the bbst, obviously gets removed once I actually implement a BBST
        bbst = bbst.OrderBy(x => x.DistanceAt(Position, 0)).ToList();

        //Add the position of the camera to the result, this is the first (and last part of the polygon)
        result.Add(new PolygonVertex(Position.X, Position.Y, new List<PolygonVertex>()));

        //Second round we actually add vertices to the result at the start and end of each group / angle
        foreach(var group in GroupedVertices)
        {
            double angle = group.Item1;
            List<Tuple<Vertex, Edge>> vertices = group.Item2;
            PolygonVertex firstVertex = null;

            //Add the current leader at the start of a group
            //Only if the sweepline is within the viewing angle of the camera
            if (bbst.Count > 0 && angle >= minAngle && angle <= maxAngle)
            {
                firstVertex = bbst.First().GetAngleIntersection(angle, Position);
                result.Add(firstVertex);
            }

            foreach (var vertexEdge in vertices)
            {
                Vertex vertex = vertexEdge.Item1;
                Edge edge = vertexEdge.Item2;

                if (!bbst.Remove(edge))
                {
                    bbst.Add(edge);
                }

                bbst = bbst.OrderBy(x => x.DistanceAt(Position, angle)).ToList();
            }

            //Again add the current edge in the front, could be the same as the one added at the start
            //Only if the sweepline is within the viewing angle of the camera
            if (bbst.Count > 0 && angle >= minAngle && angle <= maxAngle)
            {
                PolygonVertex secondVertex = bbst.First().GetAngleIntersection(angle, Position);
                //Only add this second vertex in the group if it is not the same as the first
                if (firstVertex == null || !firstVertex.SamePositionAs(secondVertex))
                {
                    result.Add(secondVertex);
                }
            }
        }

        //Add the camera again, it also is the last vertex of the polygon
        result.Add(new PolygonVertex(Position.X, Position.Y, new List<PolygonVertex>()));

        //Round all vertices coordinates to 10 decimals, again ugly, but seems to fix some issues, Might want to remove the rounding?
        //Also set the polygon vertex following vertices correctly
        for (int i=0; i<result.Count(); i++)
        {
            result[i] = new PolygonVertex(
                Math.Round(result[i].X, 10),
                Math.Round(result[i].X, 10),
                result.Skip(i + 1)
                );
        }

        return result;
    }

    /// <summary>
    /// Registers the selection of this camera and invokes event for it.
    /// </summary>
    public void OnMouseUpAsButton()
    {
        this.onSelected?.Invoke(this);
    }

    /// <summary>
    /// Set the "selection" collider to active or inactive, depedent on parameter.
    /// This is used to handle overlapping colliders. 
    /// </summary>
    /// <param name="isActive"></param>
    public void SetColliderActive(bool isActive)
    {
        this._selectionCollider.enabled = isActive;
    }

    /// <summary>
    /// Determine the quadrant of a vertex compared to a camera
    /// </summary>
    /// <param name="vertex">Vertex of which to determine the quadrant</param>
    /// <returns>1,2,3 or 4</returns>
    private int GetQuadrant(Vertex vertex)
    {
        if (vertex.Y > Position.Y)
        {
            if (vertex.X > Position.X)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (vertex.X > Position.X)
            {
                return 4;
            }
            else
            {
                return 3;
            }
        }
    }
}
