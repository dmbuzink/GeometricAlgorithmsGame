using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraPlacer : MonoBehaviour
{
    public event Action<Camera> OnCameraRemoved;
    public event Action<Camera> OnCameraConfirmed;
    /// <summary>
    /// Event that triggers if the gameobject is either set to active or not.
    /// The given parameter is true if active, false if inactive.
    /// </summary>
    public event Action<bool> OnActivityChange; 

    [SerializeField] private RemoveButton _removeButton;
    [SerializeField] private ConfirmButton _confirmButton;
    [SerializeField] private RotateButton _rotateButton;
    private Vector3 _dragOffset;
    private Camera _camera;
    private Floorplan _floorplan;
    
    // Start is called before the first frame update
    void Start()
    {
        this._removeButton.OnPressed += this.Delete;
        this._confirmButton.OnPressed += this.Confirm;
    }

    async void Update()
    {
        if (this.gameObject.activeSelf && _camera != null && _floorplan != null)
        {
            await ValidatePosition();
        }
    }

    /// <summary>
    /// Sets the camera for the camera placer, so the camera's placement
    /// can be modified by the player.
    /// </summary>
    /// <param name="cam"></param>
    public void SetCamera(Camera cam)
    {
        if (_camera != null)
        {
            this.Confirm();
        }
        this._camera = cam;
        this.transform.position = this._camera.transform.position;
        this.transform.rotation = this._camera.transform.rotation;
        this._rotateButton.Camera = cam;
        this.OnActivityChange?.Invoke(true);
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the floorplan
    /// </summary>
    /// <param name="fp"></param>
    public void SetFloorplan(Floorplan fp)
    {
        this._floorplan = fp;
    }

    /// <summary>
    /// Confirms the placement of current camera
    /// </summary>
    public void Confirm()
    {
        var camPosition = this._camera.gameObject.transform.position;
        this._camera.Position = new Vertex(camPosition.x, camPosition.y);
        this._camera.gameObject.transform.parent = null;
        this.OnCameraConfirmed?.Invoke(this._camera);
        this.gameObject.SetActive(false);
        this.OnActivityChange?.Invoke(false);
        this._camera.SetColliderActive(true);
    }

    /// <summary>
    /// Deletes the current camera
    /// </summary>
    public void Delete()
    {
        this.OnCameraRemoved?.Invoke(this._camera);
        Destroy(this._camera.gameObject);
        this.OnActivityChange?.Invoke(false);
        this.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        this._dragOffset = transform.position - GetMousePosition();
    }

    private void OnMouseDrag()
    {
        this.transform.position = GetMousePosition() + this._dragOffset;
    }

    /// <summary>
    /// Gets the position of the cursor in world space.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMousePosition()
    {
        var inputMousePosition = Input.mousePosition;
        var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(inputMousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }

    private Vertex _lastValidatedPosition;
    private bool _lastPositionValidity;
    /// <summary>
    /// Validates the position of the camera currently. Uses caching when 'onlyIfNewPosition' is true.
    /// </summary>
    /// <param name="onlyIfNewPosition"></param>
    /// <returns></returns>
    public async Task<bool> ValidatePosition(bool onlyIfNewPosition = true)
    {
        var camPositionVector = this._camera.gameObject.transform.position;
        this._camera.Position = new Vertex(camPositionVector.x, camPositionVector.y);
        if (_lastValidatedPosition != null && _lastValidatedPosition.SamePositionAs(this._camera.Position) && onlyIfNewPosition)
        {
            return _lastPositionValidity;
        }

        this._camera.Position = new Vertex(camPositionVector.x, camPositionVector.y);
        _lastPositionValidity = await this._floorplan.SimplePolygon.PointIsWithinPolygonAsync(this._camera.Position);
        _lastValidatedPosition = this._camera.Position.Copy();
        this.SetConfirmButtonActive(_lastPositionValidity);
        return _lastPositionValidity;
    }

    /// <summary>
    /// Sets the confirm button to active or inactive depending on the parameter.
    /// </summary>
    /// <param name="isActive"></param>
    private void SetConfirmButtonActive(bool isActive)
    {
        this._confirmButton.gameObject.SetActive(isActive);
    }
}
