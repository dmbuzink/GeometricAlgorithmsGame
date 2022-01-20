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
        if (_angleOfView == 0)
        {
            _angleOfView = 90;
        }

        //Method is sometimes called while the postion is not yet set, just return null for now
        if (Position == null)
        {
            throw new NullReferenceException("Position is null");
        }

        //These will cause issues if the the angle of view causes the min/max angle to flip arround 0/360 degrees.
        double minAngle = (AngleCounterClockwise - _angleOfView / 2) * Mathf.Deg2Rad;  
        double maxAngle = (AngleCounterClockwise + _angleOfView / 2) * Mathf.Deg2Rad;
        double startAngle = (AngleCounterClockwise + 180) * Mathf.Deg2Rad;

        //If the angle > Pi, then we subtract 2 pi to make it consistent with the angles of the events which go from -pi to pi instead of pi-2pi
        if (minAngle > Mathf.PI)
        {
            minAngle -= 2 * Mathf.PI;
        }
        if (maxAngle > Mathf.PI)
        {
            maxAngle -= 2 * Mathf.PI;
        }
        if (startAngle > Mathf.PI)
        {
            startAngle -= 2 * Mathf.PI;
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

        GroupedVertices = GroupedVertices
            .Where(x => x.Item1 >= startAngle)
            .OrderBy(x => x.Item1)
            .Union(GroupedVertices.Where(x => x.Item1 < startAngle)
            .OrderBy(x => x.Item1))
            .ToList();
       
        EdgeDistanceComparer comparer = new EdgeDistanceComparer(Position);
        comparer.SetAngle(startAngle);

        SortedSet<Edge> bbst = new SortedSet<Edge>(comparer);
        List<Edge> listbst = new List<Edge>();
        BinarySearchTree<Edge> tarbst = new BinarySearchTree<Edge>(new Func<Edge, Edge, int>((Edge a, Edge b) => comparer.Compare(a, b)));

        List<Tuple<Vertex, Edge>> bstresult = new List<Tuple<Vertex, Edge>>();
        List<Tuple<Vertex, Edge>> listbstresult = new List<Tuple<Vertex, Edge>>();
        List<Tuple<Vertex, Edge>> tarbstResult = new List<Tuple<Vertex, Edge>>();

        //Initialization, find edges that intersect the start of the sweepline, we add these edges to the bst at the start.
        foreach (var edge in edges)
        {
            if (edge.GetAngleIntersection(startAngle, Position) != null)
            {
                listbst.Add(edge);
                bbst.Add(edge);
                tarbst.Insert(edge);
            }
        }

        bool passedMinAngle = false;

        //Second round we actually add vertices to the result at the start and end of each group / angle
        foreach (var group in GroupedVertices)
        {
            double angle = group.Item1;
            comparer.SetAngle(angle);
            List<Tuple<Vertex, Edge>> vertices = group.Item2;

            if(angle == minAngle)
            {
                passedMinAngle = true;
            }

            if (bbst.Count > 0 && passedMinAngle)
            {
                var leader = bbst.Min;
                var newVertex = leader.GetAngleIntersection(angle, Position);
                bstresult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            if (listbst.Count > 0 && passedMinAngle)
            {
                var leader = listbst.OrderBy(x => x.DistanceAt(Position, angle)).First();
                var newVertex = leader.GetAngleIntersection(angle, Position);
                listbstresult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            if (listbst.Count > 0 && passedMinAngle)
            {
                var leader = tarbst.GetMin();
                var newVertex = leader.GetAngleIntersection(angle, Position);
                tarbstResult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            List<Edge> edgesToAdd = new List<Edge>();
            List<Edge> edgesToRemove = new List<Edge>();

            foreach (var vertexEdge in vertices)
            {
                Edge edge = vertexEdge.Item2;
                var testedge = listbst.FirstOrDefault(x => x.ToString() == edge.ToString());

                if(testedge == null)
                {
                    edgesToAdd.Add(edge);
                }
                else
                {
                    listbst.Remove(testedge);
                    if (!bbst.Remove(edge))
                    {
                        bbst.Remove(edge);
                    }
                    tarbst.Delete(edge);
                }
            }

            edgesToAdd.ForEach(x => bbst.Add(x));
            edgesToAdd.ForEach(x => listbst.Add(x));
            edgesToAdd.ForEach(x => tarbst.Insert(x));

            if (bbst.Count > 0 && passedMinAngle)
            {
                var leader = bbst.Min;
                var newVertex = leader.GetAngleIntersection(angle, Position);
                bstresult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            if (listbst.Count > 0 && passedMinAngle)
            {
                var leader = listbst.OrderBy(x => x.DistanceAt(Position, angle)).First();
                var newVertex = leader.GetAngleIntersection(angle, Position);
                listbstresult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            if (listbst.Count > 0 && passedMinAngle)
            {
                var leader = tarbst.GetMin();
                var newVertex = leader.GetAngleIntersection(angle, Position);
                tarbstResult.Add(new Tuple<Vertex, Edge>(newVertex, leader));
            }

            if (listbst.Count() != tarbst.GetAll().Count())
            {
                Console.WriteLine("");
            }

            //If we just handled group with the maxAngle,
            //we break the loop since further points won't be in the result anyway
            if (angle == maxAngle)
            {
                break;
            }
        }

        bstresult.Add(new Tuple<Vertex, Edge>(new Vertex(Position.X, Position.Y), null));
        listbstresult.Add(new Tuple<Vertex, Edge>(new Vertex(Position.X, Position.Y), null));
        tarbstResult.Add(new Tuple<Vertex, Edge>(new Vertex(Position.X, Position.Y), null));
        this.cameraView = new SimplePolygon(listbstresult.Where(x => x.Item1 != null).Select(x => x.Item1));
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

public class EdgeDistanceComparer : IComparer<Edge>
{
    private readonly Vertex camera;
    private double angle;

    public EdgeDistanceComparer(Vertex camera)
    {
        this.camera = camera;
    }

    public void SetAngle(double angle)
    {
        this.angle = angle;
    }

    public int Compare(Edge a, Edge b)
    {
        if (a == null)
        {
            return int.MaxValue;
        }

        if(b == null)
        {
            return int.MaxValue;
        }

        //Return 0 if they are the same
        if (AreEqual(a, b))
        {
            return 0;
        }

        double distanceEdge1 = a.DistanceAt(camera, angle);
        double distanceEdge2 = b.DistanceAt(camera, angle);

        if(distanceEdge1 == distanceEdge2)
        {
            return a.ToString().CompareTo(b.ToString());
        }

        if(distanceEdge1 == int.MaxValue || distanceEdge2 == int.MaxValue)
        {
            return int.MaxValue;
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

    public bool AreEqual(Edge a, Edge b)
    {
        return (a.StartPoint.SamePositionAs(b.StartPoint)  &&
             a.EndPoint.SamePositionAs(b.EndPoint));
    }
}
