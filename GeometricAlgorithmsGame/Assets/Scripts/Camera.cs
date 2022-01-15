using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Slider = UnityEngine.UI.Slider;

public class Camera : MonoBehaviour
{
    public event Action<Camera> onSelected;
    [SerializeField] private BoxCollider2D _selectionCollider;
    [SerializeField] private LineRenderer _lineRenderer;

    public Vertex Position { get; set; }
    public double Angle
    {
        get 
        { 
            var z = this.gameObject.transform.rotation.eulerAngles.z;
            return z < Mathf.Epsilon ? 0 : 360 - z; 
        }
    }
    public double AngleCounterClockwise
    {
        get
        {
            var z = this.gameObject.transform.rotation.eulerAngles.z;
            return z;
        }
    }

    public Floorplan floorplan { get; set; }
    private SimplePolygon cameraView { get; set; }

    //Angle of view of the camera, half of it on both sides of the viewing angle.
    [SerializeField] private double _angleOfView = 90;

    public Camera(Vertex position, double angle = 0)
    {
        this.Position = position;
        this.floorplan = floorplan;
    }

    // Start is called before the first frame update
    void Start()
    {
        // this._selectionCollider = GetComponent<BoxCollider2D>();
        this._lineRenderer = gameObject.GetComponent<LineRenderer>();
        this._lineRenderer.loop = true;
        this._lineRenderer.startWidth = 0.1f;
        this._lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        this.CalculateView();
        if (this.cameraView!=null) 
            StartCoroutine(DrawCamera());
    }

    private IEnumerator DrawCamera()
    {
        this._lineRenderer.positionCount = cameraView.Count();
        for (var i = 0; i < cameraView.Count(); i++)
        {
            var vertex = cameraView.ElementAt(i);
            this._lineRenderer.SetPosition(i, vertex.ToVector3());
        }

        yield return null;
    }

    /// <summary>
    /// Calculates the the are that can be viewed by the camera based on the floorplan
    /// </summary>
    public void CalculateView()
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
        double minAngle = (AngleCounterClockwise - _angleOfView / 2) * Mathf.Deg2Rad;  
        double maxAngle = (AngleCounterClockwise + _angleOfView / 2) * Mathf.Deg2Rad; 

        //If the angle > Pi, then we subtract 2 pi to make it consistent with the angles of the events which go from -pi to pi instead of pi-2pi
        if (minAngle > Mathf.PI)
        {
            minAngle -= 2 * Mathf.PI;
        }
        if (maxAngle > Mathf.PI)
        {
            maxAngle -= 2 * Mathf.PI;
        }

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
        foreach(var group in testGroupedVertices)
        {
            double angle = group.Item1;
            List<Tuple<Vertex, Edge>> vertices = group.Item2;

            //Add the current leader at the start of a group
            //Only if the sweepline is within the viewing angle of the camera
            if (bst.Count > 0)
            {
                result.Add(bst.First().GetAngleIntersection(angle, Position));

                //var leader = bst.First();

                ////If the leader is of an edge to be removed in the group, then we can just pick the endpoint as vertex to add
                //if(vertices.Any(x => x.Item1 == leader.EndPoint))
                //{
                //    if (result.Count == 0 || !result.Last().SamePositionAs(leader.EndPoint))
                //    {
                //        result.Add(leader.EndPoint);
                //    }
                //}
                //else
                //{
                //    //Else we have to compute the vertex
                //    Vertex firstVertex = leader.GetAngleIntersection(angle, Position);
                //    if (firstVertex == null)
                //    {
                //        //This should not happen
                //    }
                //    else if (result.Count == 0 || !result.Last().SamePositionAs(firstVertex))
                //    {
                //        result.Add(firstVertex);
                //    }
                //}
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
            if (bst.Count > 0)
            {
                result.Add(bst.First().GetAngleIntersection(angle, Position));


                //Vertex secondVertex = bst.First().GetAngleIntersection(angle, Position);
                ////Only add this second vertex in the group if it is not the same as the first
                //if (secondVertex == null)
                //{
                //    //Should not happen
                //}
                //else if (result.Count == 0 || result.Last().SamePositionAs(secondVertex))
                //{
                //    result.Add(secondVertex);
                //}

                //var leader = bst.First();

                ////If the leader is part of the edges just added, simply add this start vertex
                //if(vertices.Any(x => x.Item1 == leader.StartPoint))
                //{
                //    result.Add(leader.StartPoint);
                //}
                //else
                //{
                //    //The new vertex is not the start vertex of the leader, so we have to find it

                //}
            }

            //If we just handled group with the maxAngle,
            //we break the loop since further points won't be in the result anyway
            if(angle == maxAngle)
            {
                break;
            }
        }

        result = result.Where(x => x != null).ToList();

        //Add the camera again, it also is the last vertex of the polygon
        result.Add(new Vertex(Position.X, Position.Y));

        this.cameraView = new SimplePolygon(result);
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
}
