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
            return z - 90;
        }
    }

    public Floorplan floorplan { get; set; }
    public CameraFace cameraView { get; set; }

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
        double minAngle = GeometricHelper.AdjustRadToLimits((AngleCounterClockwise - _angleOfView / 2) * Mathf.Deg2Rad);
        double maxAngle = GeometricHelper.AdjustRadToLimits((AngleCounterClockwise + _angleOfView / 2) * Mathf.Deg2Rad);
        double startAngle = GeometricHelper.AdjustRadToLimits((AngleCounterClockwise + 180) * Mathf.Deg2Rad);

        //We first need a list of all edges
        var edges = floorplan.SimplePolygon
            .GetVerticesPairWise()
            .Select(x => new Edge(new PolygonVertex(x.v1.X, x.v1.Y, new List<PolygonVertex>()), new PolygonVertex(x.v2.X, x.v2.Y, new List<PolygonVertex>())));

        //Get all vertices, group them on their angle to the camera and sort them on their angle
        IEnumerable<(double angle, List<(Vertex vertex, Edge edge)> vertices)> GroupedVertices = edges
            .SelectMany(x => new List<(Vertex vertex, Edge edge)>() { (x.StartPoint, x), (x.EndPoint, x) })
            .GroupBy(x => GeometricHelper.AngleBetweenPointsRad(Position, x.vertex))
            .Select(x => (x.Key, x.ToList()))
            .Union(new List<(double angle, List<(Vertex, Edge)> vertices)>()
            {
                //Add the min and max angles as events since these should be the first and last vertex of the result
                (minAngle, new List<(Vertex, Edge)>()),
                (maxAngle, new List<(Vertex, Edge)>())
            });

        //Sort all edges arround the start order in anti clockwise order
        GroupedVertices = GroupedVertices
            .Where(x => x.angle >= startAngle)
            .OrderBy(x => x.angle)
            .Union(GroupedVertices.Where(x => x.angle < startAngle)
                .OrderBy(x => x.angle)
             );

        List<Edge> listbst = new List<Edge>();
        List<Vertex> listbstresult = new List<Vertex>();

        //Initialization, find edges that intersect the start of the sweepline, we add these edges to the bst at the start.
        listbst.AddRange(edges.Where(x => x.GetAngleIntersection(startAngle, Position) != null));

        //Mark when we passed the minimal camera angle such that we know when to start adding vertices
        bool passedMinAngle = false;

        //Second round we actually add vertices to the result at the start and end of each group / angle
        foreach (var (angle, vertices) in GroupedVertices)
        {
            //Set passedMinAngle to true once we pass it
            passedMinAngle = passedMinAngle || angle == minAngle;

            AddLeaderToResult(angle);
            foreach (var (vertex, edge) in vertices.Where(x => !listbst.Remove(x.edge))){
                listbst.Add(edge);
            }
            AddLeaderToResult(angle);

            //If we just handled group with the maxAngle,
            //we break the loop since further points won't be in the result anyway
            if (angle == maxAngle) break;
        }

        void AddLeaderToResult(double angle)
        {
            if (listbst.Count > 0 && passedMinAngle)
            {
                //Add the intersection with the leader to the result
                var newVertex = listbst
                    .OrderBy(x => x.DistanceAt(Position, angle))
                    .First()
                    .GetAngleIntersection(angle, Position);

                if(newVertex != null && (listbstresult.Count == 0 || !listbstresult.Last().SamePositionAs(newVertex))) { 
                    listbstresult.Add(newVertex);
                }
            }
        }

        listbstresult.Add(new Vertex(Position.X, Position.Y));
        this.cameraView = new CameraFace(new List<Camera>(new []{this}), listbstresult);
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