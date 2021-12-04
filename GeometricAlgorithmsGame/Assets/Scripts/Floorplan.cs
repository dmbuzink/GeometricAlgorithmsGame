using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class Floorplan : MonoBehaviour
{
    [SerializeField] private static Floorplan _floorplanPrefab;
    private VerticalDecomposition _verticalDecomposition;
    private IEnumerable<Camera> _cameras = new List<Camera>();
    private IEnumerable<PolygonVertex> _polygonVertices;
    private DesiredObject _desiredObject;
    private Entrance _entrance;

    public Floorplan(IEnumerable<PolygonVertex> polygonVertices, DesiredObject desiredObject, Entrance entrance)
    {
        this._polygonVertices = polygonVertices;
        this._desiredObject = desiredObject;
        this._entrance = entrance;
    }

    /// <summary>
    /// Initiates a floorplan prefab with some arguments.
    /// </summary>
    /// <param name="polygonVertices"></param>
    /// <param name="desiredObject"></param>
    /// <param name="entrance"></param>
    public static async Task Create(IEnumerable<PolygonVertex> polygonVertices, DesiredObject desiredObject,
        Entrance entrance)
    {
        var createdFloorplan = Instantiate(_floorplanPrefab);
        createdFloorplan._polygonVertices = polygonVertices;
        createdFloorplan._desiredObject = desiredObject;
        createdFloorplan._entrance = entrance;
        createdFloorplan._verticalDecomposition = await VerticalDecomposition.
            CreateVerticalDecomposition(createdFloorplan._polygonVertices); 
    }

    // Start is called before the first frame update
    async void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
