using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
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
    [SerializeField] private double _angleOfView = 90;

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
    public SimplePolygon CalculateView(Floorplan floorplan)
    {
        //Somethoe unity just sets it to 0 somehow, manually set it to 90 for now
        if(_angleOfView == 0)
        {
            _angleOfView = 90;
        }

        //Method is sometimes called while the postion is not yet set, just return null for now
        if(Position == null)
        {
            throw new NullReferenceException("Position is null");
        }

        //These will cause issues if the the angle of view causes the min/max angle to flip arround 0/360 degrees.
        //Rotate it 90 degrees since this script assumes 0 degrees to to the dead right instead of up (Might want to change this to make it consistent)
        double minAngle = (Angle - _angleOfView / 2) * Mathf.Deg2Rad;
        double maxAngle = (Angle + _angleOfView / 2) * Mathf.Deg2Rad;

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

        //GroupedVertices = GroupedVertices.OrderBy(x => x.Item1).ToList();

        //First sort the vertices with an angle >= the min angle, then add the vertices with a smaller angle in reverse order
        var testGroupedVertices = GroupedVertices.Where(x => x.Item1 >= minAngle).OrderBy(x => x.Item1).ToList();
        testGroupedVertices.AddRange(GroupedVertices.Where(x => x.Item1 < minAngle).OrderBy(x => x.Item1));

        //Initialize the bbst NOTE: this currently still just is a list which gets sorted after each operation, obviously not efficient yet.
        List<Edge> bst = new List<Edge>();
        List<Vertex> result = new List<Vertex>();

        //Initialization, find edges that intersect the start of the sweepline, we add these edges to the bst at the start.
        foreach(var edge in edges)
        {
            if(edge.GetAngleIntersection(minAngle, Position) != null)
            {
                bst.Add(edge);
            }
        }

        //Sort the bbst, obviously gets removed once I actually implement a BST
        bst = bst.OrderBy(x => x.DistanceAt(Position, minAngle)).ToList();

        //Add the position of the camera to the result, this is the first (and last part of the polygon)
        result.Add(new Vertex(Position.X, Position.Y));

        //Second round we actually add vertices to the result at the start and end of each group / angle
        foreach(var group in GroupedVertices)
        {
            double angle = group.Item1;
            List<Tuple<Vertex, Edge>> vertices = group.Item2;

            //Add the current leader at the start of a group
            //Only if the sweepline is within the viewing angle of the camera
            if (bst.Count > 0 && angle >= minAngle && angle <= maxAngle)
            {
                var leader = bst.First();
                                
                //If the leader is of an edge to be removed in the group, then we can just pick the endpoint as vertex to add
                if(vertices.Any(x => x.Item1 == leader.EndPoint))
                {
                    if (result.Count == 0 || !result.Last().SamePositionAs(leader.EndPoint))
                    {
                        result.Add(leader.EndPoint);
                    }
                }
                else
                {
                    //Else we have to compute the vertex
                    Vertex firstVertex = leader.GetAngleIntersection(angle, Position);
                    if (firstVertex == null)
                    {
                        //This should not happen
                    }
                    else if (result.Count == 0 || !result.Last().SamePositionAs(firstVertex))
                    {
                        result.Add(firstVertex);
                    }


                }
            }

            foreach (var vertexEdge in vertices)
            {
                Edge edge = vertexEdge.Item2;

                if (!bst.Remove(edge))
                {
                    bst.Add(edge);
                }
            }

            bst = bst.OrderBy(x => x.DistanceAt(Position, angle)).ToList();

            //Again add the current edge in the front, could be the same as the one added at the start
            //Only if the sweepline is within the viewing angle of the camera
            if (bst.Count > 0 && angle >= minAngle && angle <= maxAngle)
            {
                var leader = bst.First();

                //If the leader is part of the edges just added, simply add this start vertex
                if(vertices.Any(x => x.Item1 == leader.StartPoint))
                {
                    result.Add(leader.StartPoint);
                }
                else
                {
                    //The new vertex is not the start vertex of the leader, so we have to find it
                    Vertex secondVertex = leader.GetAngleIntersection(angle, Position);
                    //Only add this second vertex in the group if it is not the same as the first
                    if (secondVertex == null)
                    {
                        //Should not happen
                    }
                    else if (result.Count == 0 || result.Last().SamePositionAs(secondVertex))
                    {
                        result.Add(secondVertex);
                    }
                }
            }

            //If we just handled group with the maxAngle,
            //we break the loop since further points won't be in the result anyway
            if(angle == maxAngle)
            {
                break;
            }
        }

        //Add the camera again, it also is the last vertex of the polygon
        result.Add(new Vertex(Position.X, Position.Y));

        return new SimplePolygon(result);
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
