using System;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float CameraSizeMargin;
    [SerializeField] private Floorplan FloorplanPrefab;
    [SerializeField] private string _levelConfigJson;
    [SerializeField] private Camera _cameraPrefab;
    [SerializeField] private Floorplan _floorplan;
    [SerializeField] private UnityEngine.Camera _unityCamera;
    [SerializeField] private CameraPlacer _cameraPlacer;
    [SerializeField] private GameObject _addCameraButton;
    [SerializeField] private GameObject _confirmAllCamerasButton;
    private Camera _currentCamera;
    private LevelConfig _levelConfig;
    private Vector3 _centerPointOfWorld;


    // Start is called before the first frame update
    async void Start()
    {
        _levelConfig = string.IsNullOrEmpty(_levelConfigJson)
            ? GetDefaultLevelConfig()
            : LevelConfigManager.GetSelectedLevelConfig(this._levelConfigJson);
        
        this._cameraPlacer.OnCameraRemoved += this.HandleCameraDeletion;
        this._cameraPlacer.OnCameraConfirmed += async cam =>
        {
            var camIsValid = await this._floorplan.SimplePolygon.PointIsWithinPolygonAsync(cam.Position);
        };
        this._cameraPlacer.OnActivityChange += cameraPlacerIsActive => this._addCameraButton.SetActive(!cameraPlacerIsActive);
        await SetUnityCamera();
        await InstantiateFloorplan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Instantiate the floorplan with the level configuration
    /// </summary>
    private async Task InstantiateFloorplan()
    {
        this._floorplan = Instantiate(FloorplanPrefab);
        await this._floorplan.SetUp(_levelConfig.GetSimplePolygon(), _levelConfig.DesiredObject,
            _levelConfig.Entrance);
        _cameraPlacer.SetFloorplan(_floorplan);
        this._floorplan.OnAmountOfCamerasChanged += HandleAmountOfCamerasChanged;
    }

    /// <summary>
    /// The unity scene camera starts by default at (0,0) with scale (1,1).
    /// This results in the floorplan to be not correctly centered in view
    /// and also to be only seen partly.
    /// This will center the unity camera on the floorplan dynamically.
    /// </summary>
    private async Task SetUnityCamera()
    {
        var maxX = this._levelConfig.Vertices.Max(v => v.X);
        var minX = this._levelConfig.Vertices.Min(v => v.X);
        var maxY = this._levelConfig.Vertices.Max(v => v.Y);
        var minY = this._levelConfig.Vertices.Min(v => v.Y);
        
        // size = camera height from center to top
        // calculate required size based on height and width (with relative convertion)
        // Choose biggest + margin

        var halfYRange = (maxY - minY) / 2;
        const double widthToHeighRatio = 1080f / 1920f;
        var halfXRange = (maxX - minX) / 2;

        var cameraSize = (float) Math.Max(halfYRange, halfXRange * widthToHeighRatio);
        this._unityCamera.gameObject.transform.localPosition = new Vector3( (float)(minX + halfXRange), 
            (float) (minY + halfYRange), -10);
        this._unityCamera.orthographicSize = cameraSize + CameraSizeMargin;

        this._centerPointOfWorld =
            UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
        this._centerPointOfWorld.z = 0;
    }

    /// <summary>
    /// Instantiates a new camera and makes it available for the player to modify.
    /// </summary>
    public void AddCamera()
    {
        this._cameraPlacer.gameObject.transform.position = _centerPointOfWorld;
        this._cameraPlacer.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        var newCamera = Instantiate(this._cameraPrefab, _cameraPlacer.transform);
        this._floorplan.AddCamera(newCamera);
        this._cameraPlacer.SetCamera(newCamera);
        newCamera.onSelected += HandleSelectionOfCamera;

        this._floorplan.ActivateSelectionColliderOfAllCameras();
        newCamera.SetColliderActive(false);
    }

    /// <summary>
    /// Handles the deletion of the currently selected camera.
    /// </summary>
    /// <param name="cam"></param>
    private void HandleCameraDeletion(Camera cam)
    {
        cam.onSelected -= HandleSelectionOfCamera;
        this._floorplan.RemoveCamera(cam);
    }

    /// <summary>
    /// Handles the selection of a camera by the player.
    /// </summary>
    /// <param name="cam"></param>
    private void HandleSelectionOfCamera(Camera cam)
    {
        this._currentCamera = cam;
        this._cameraPlacer.SetCamera(cam);
        this._floorplan.ActivateSelectionColliderOfAllCameras();
        cam.SetColliderActive(false);
        cam.gameObject.transform.SetParent(this._cameraPlacer.gameObject.transform);
    }
    
    /// <summary>
    /// Confirms the camera placements of all cameras, currently in the floorplan.
    /// Make such are proceeding processes are started (like checking for a path, etc.).
    /// This method is the 'non-async' version to be call-able from Unity's editor.
    /// </summary>
    public void ConfirmAllCameras() => this.ConfirmAllCamerasAsync();

    /// <summary>
    /// Confirms the camera placements of all cameras, currently in the floorplan.
    /// Make such are proceeding processes are started (like checking for a path, etc.).
    /// </summary>
    private async Task ConfirmAllCamerasAsync()
    {
        // TODO: To be implemented by Damian M. Buzink
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handles the change in amount of cameras.
    /// More specifically turns activates or deactivates
    /// 'Confirm placement of all cameras' button.
    /// </summary>
    private void HandleAmountOfCamerasChanged(int amountOfCameras)
    {
        var isActive = amountOfCameras > 0;
        this._confirmAllCamerasButton.SetActive(isActive);
    }

    /// <summary>
    /// Show the screen for when the player succeeds
    /// </summary>
    private void ShowSuccessScreen()
    {
        // TODO: To be implemented by Teun van Zon
        throw new NotImplementedException();
    }

    /// <summary>
    /// Show the screen for when the player fails
    /// </summary>
    private void ShowFailureScreen()
    {
        // TODO: To be implemented by Teun van Zon
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the configuration for the default level.
    /// </summary>
    /// <returns></returns>
    private LevelConfig GetDefaultLevelConfig()
    {
        var entrance = new Vertex(11.7, 7);
        var desiredObject = new Vertex(5.1, 5);
        
        var v1 = new Vertex(1, 1);
        var v2 = new Vertex(2, 2);
        var v3 = new Vertex(1.5, 3);
        var v4 = new Vertex(3, 4);
        var v5 = new Vertex(5, 7);
        var v6 = new Vertex(12, 7);
        var v7 = new Vertex(16, 5);
        var v8 = new Vertex(15, 2.5);
        var v9 = new Vertex(14, 1.7);
        var v10 = new Vertex(6, 1.2);

        return new LevelConfig()
        {
            LevelId = -1,
            Entrance = entrance,
            DesiredObject = desiredObject,
            Vertices = new[] { v10, v9, v8, v7, v6, v5, v4, v3, v2, v1 }
        };
    }
}
