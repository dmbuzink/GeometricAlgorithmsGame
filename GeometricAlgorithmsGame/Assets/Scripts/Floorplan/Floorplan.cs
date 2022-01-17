using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class Floorplan : MonoBehaviour
{
    public event Action<int> OnAmountOfCamerasChanged;
    
    private RegionArrangement<CombinedFace<SimplePolygon>> _regions;
    public List<Camera> Cameras;
    public FloorFace SimplePolygon;
    [SerializeField] private DesiredObject _desiredObject;
    [SerializeField] private Entrance _entrance;
    private LineRenderer _lineRenderer; 
    
    public DebugFace _debugFacePrefab;
    public List<DebugFace> _debugFaces = new List<DebugFace>();

    public Floorplan(FloorFace simplePolygon, DesiredObject desiredObject, Entrance entrance)
    {
        this.SimplePolygon = simplePolygon;
        this._desiredObject = desiredObject;
        this._entrance = entrance;
    }

    /// <summary>
    /// Initiates a floorplan prefab with some arguments.
    /// </summary>
    /// <param name="simplePolygon"></param>
    /// <param name="desiredObject"></param>
    /// <param name="entrance"></param>
    public async Task SetUp(FloorFace simplePolygon, Vertex desiredObjectVertex,
        Vertex entranceVertex)
    {
        SimplePolygon = simplePolygon;
        _entrance.transform.position = new Vector3((float)entranceVertex.X, (float)entranceVertex.Y);
        _desiredObject.transform.position = new Vector3((float) desiredObjectVertex.X, (float) desiredObjectVertex.Y);
        // _verticalDecomposition = await VerticalDecomposition.
            // CreateVerticalDecomposition(SimplePolygon);
        // Temp. commented out due to issues with errors in GameManagers.InstantiateFloorplan() as result
    }

    // Start is called before the first frame update
    void Start()
    {
        this.Cameras = new List<Camera>();
        // this._lineRenderer = gameObject.AddComponent<LineRenderer>();
        this._lineRenderer = gameObject.GetComponent<LineRenderer>();
        this._lineRenderer.loop = true;
        this._lineRenderer.startWidth = 0.1f;
        this._lineRenderer.endWidth = 0.1f;
        this._lineRenderer.startColor = Color.magenta;
        this._lineRenderer.endColor = Color.magenta;
        //SimplePolygon.Draw(this._lineRenderer);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Activates the collider with which the player can select cameras.
    /// This is turned of for a specific camera when modifying it's position,
    /// due to overlapping colliders.
    /// </summary>
    public void ActivateSelectionColliderOfAllCameras()
    {
        this.Cameras.ForEach(c => c.SetColliderActive(true));
    }

    /// <summary>
    /// Calculates the view of all cameras combined of the floorplan
    /// </summary>
    public async Task CalculateView()
    {
        float outerPadding = 1;
        (float minX, float maxX, float minY, float maxY) = this.SimplePolygon.GetBoundingBox();
        List<Vertex> outerPoints = new List<Vertex>(new[]
        {
            new Vertex(minX - outerPadding, minY - outerPadding),
            new Vertex(maxX + outerPadding, minY - outerPadding),
            new Vertex(maxX + outerPadding, maxY + outerPadding),
            new Vertex(minX - outerPadding, maxY + outerPadding),
        });
        OuterFace outerFace = new OuterFace(outerPoints);

        List<SimplePolygon> polygons = new List<SimplePolygon>(new SimplePolygon[]
        {
            outerFace,
            this.SimplePolygon
        });
        foreach(Camera camera in this.Cameras)
        {
            CameraFace view = camera.cameraView;
            if (view != null) polygons.Add(view);
        }

        List<CombinedFace<SimplePolygon>> combinedFaces = FaceCombiner.CombineFaces(polygons);
        this.DrawDebugFaces(combinedFaces);
        List<string> debugPolygs = combinedFaces.Select<CombinedFace<SimplePolygon>, string>(face =>
                face.Vertices
                    .Select<Vertex, string>(v => "{x:" + v.X + ", y:" + v.Y + "}")
                    .Aggregate((s, v) => s + "," + v)
            ).ToList();

        _regions = await RegionArrangement<CombinedFace<SimplePolygon>>.CreateRegionArrangement(combinedFaces);
        //this.DrawDebugTrapezoids();
        
       
    }

    public void DrawDebugFaces(List<CombinedFace<SimplePolygon>> newFaces)
    {
        foreach (DebugFace dbgFace in _debugFaces)
        {
            Destroy(dbgFace.gameObject);
        }

        _debugFaces.Clear();

        foreach (CombinedFace<SimplePolygon> face in newFaces)
        {
            var newDebugFace = Instantiate(this._debugFacePrefab);
            _debugFaces.Add(newDebugFace);
            newDebugFace.Source = face;
        }
    }

    public void DrawDebugTrapezoids()
    {
        List<Trapezoid<PolygonSegment<CombinedFace<SimplePolygon>>>> trapezoids = new List<Trapezoid<PolygonSegment<CombinedFace<SimplePolygon>>>>();
        _regions.Decomposition.Root.GetTrapezoids(trapezoids);
        List<CombinedFace<SimplePolygon>> trapezoidPolys = new List<CombinedFace<SimplePolygon>>();
        foreach (var t in trapezoids)
        {
            SimplePolygon sp = t.CalculatePolygon();
            if (sp == null) continue;
            trapezoidPolys.Add(new CombinedFace<SimplePolygon>(new List<SimplePolygon>(new SimplePolygon[]
            {
                sp
            }), sp.Vertices));
        }
        DrawDebugFaces(trapezoidPolys);
    }

    /// <summary>
    /// Calculates the the percentage of the floorplan that is viewed by atleast one camera.
    /// </summary>
    /// <returns></returns>
    public async Task<float> GetPercentageOfFloorplanInView()
    {
        if (this._regions == null)
        {
            throw new Exception("The regions should be calculated first by calling CalculateView");
        }
        
        Dictionary<CombinedFace<SimplePolygon>, double> areas = this._regions.CalculateAreas();
        double totalArea = 0;
        double cameraArea = 0;
        double total = 0;

        foreach(CombinedFace<SimplePolygon> region in areas.Keys)
        {
            double area = areas[region];
            total += area;
            bool containsCamera = region.Sources.Find(source => source is CameraFace) != null;
            bool containsFloorPlan = region.Sources.Find(source => source is FloorFace) != null;
            if(containsFloorPlan)
            {
                totalArea += area;
                if (containsCamera) cameraArea += area;
            }
            Debug.Log(area+" "+containsCamera+" "+containsFloorPlan);
        }
        Debug.Log(total+" "+totalArea+"  "+ cameraArea+"  ");


        return (float)(cameraArea / totalArea);
    }

    /// <summary>
    /// Checks to see if a path exists from the entrance to the desired object,
    /// such that you would not be spotted by a camera
    /// </summary>
    /// <returns></returns>
    public async Task<bool> PathExistsFromEntranceToDesiredObject()
    {
        // TODO: To be implemented by Teun van Zon
        throw new ArgumentException();
    }

    /// <summary>
    /// Adds the given camera to the set of stored cameras.
    /// </summary>
    /// <param name="cam"></param>
    public void AddCamera(Camera cam)
    {
        this.Cameras.Add(cam);
        this.OnAmountOfCamerasChanged?.Invoke(this.Cameras.Count);
    }

    /// <summary>
    /// Removes the given camera from the set of stored cameras.
    /// </summary>
    /// <param name="cam"></param>
    public void RemoveCamera(Camera cam)
    {
        this.Cameras.Remove(cam);
        this.OnAmountOfCamerasChanged?.Invoke(this.Cameras.Count);
    }
}
