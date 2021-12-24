using System;
using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    void Start()
    {
        this._removeButton.OnPressed += this.Delete;
        this._confirmButton.OnPressed += this.Confirm;
    }

    public void SetCamera(Camera cam)
    {
        this._camera = cam;
        this.transform.position = this._camera.transform.position;
        this.transform.rotation = this._camera.transform.rotation;
        this._rotateButton.Camera = cam;
        this.OnActivityChange?.Invoke(true);
        this.gameObject.SetActive(true);
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

    private Vector3 GetMousePosition()
    {
        var inputMousePosition = Input.mousePosition;
        var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(inputMousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
    
    
}
