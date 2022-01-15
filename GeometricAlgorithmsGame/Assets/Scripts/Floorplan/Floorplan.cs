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
    
    private VerticalDecomposition<Segment> _verticalDecomposition;
    public List<Camera> Cameras;
    public SimplePolygon SimplePolygon;
    [SerializeField] private DesiredObject _desiredObject;
    [SerializeField] private Entrance _entrance;
    private LineRenderer _lineRenderer;

    public Floorplan(SimplePolygon simplePolygon, DesiredObject desiredObject, Entrance entrance)
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
    public async Task SetUp(SimplePolygon simplePolygon, Vertex desiredObjectVertex,
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
        StartCoroutine(DrawFloorplan());
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    /// <summary>
    /// Draws the simple polygon making up the floorplan
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrawFloorplan()
    {
        this._lineRenderer.positionCount = SimplePolygon.Count();
        for (var i = 0; i < SimplePolygon.Count(); i++)
        {
            var vertex = SimplePolygon.ElementAt(i);
            this._lineRenderer.SetPosition(i, vertex.ToVector3());
        }

        yield return null;
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
        // TODO: To be implemented by Damian M. Buzink
    }

    /// <summary>
    /// Calculates the the percentage of the floorplan that is viewed by atleast one camera.
    /// </summary>
    /// <returns></returns>
    public async Task<float> GetPercentageOfFloorplanInView()
    {
        // TODO: To be implemented by Damian M. Buzink
        throw new ArgumentException();
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
