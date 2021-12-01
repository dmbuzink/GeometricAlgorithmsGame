using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class Floorplan : MonoBehaviour
{
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

    // Start is called before the first frame update
    async void Start()
    {
        this._verticalDecomposition = await VerticalDecomposition.CreateVerticalDecomposition(_polygonVertices);
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
        // TODO: To be implemented by X
    }

    /// <summary>
    /// Calculates the the percentage of the floorplan that is viewed by atleast one camera.
    /// </summary>
    /// <returns></returns>
    public async Task<float> GetPercentageOfFloorplanInView()
    {
        // TODO: To be implemented by X
        throw new ArgumentException();
    }
}
