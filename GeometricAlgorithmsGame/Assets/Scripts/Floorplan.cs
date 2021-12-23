using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class Floorplan : MonoBehaviour
{
    private VerticalDecomposition _verticalDecomposition;
    private IEnumerable<Camera> _cameras = new List<Camera>();
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
        _verticalDecomposition = await VerticalDecomposition.
            CreateVerticalDecomposition(SimplePolygon);
    }

    // Start is called before the first frame update
    void Start()
    {
        // this._lineRenderer = gameObject.AddComponent<LineRenderer>();
        this._lineRenderer = gameObject.GetComponent<LineRenderer>();
        this._lineRenderer.loop = true;
        this._lineRenderer.startWidth = 0.1f;
        this._lineRenderer.endWidth = 0.1f;
        this._lineRenderer.startColor = Color.blue;
        this._lineRenderer.endColor = Color.blue;
        StartCoroutine(DrawFloorplan());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
}
