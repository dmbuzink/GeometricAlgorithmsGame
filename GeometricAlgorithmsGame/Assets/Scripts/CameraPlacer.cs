using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlacer : MonoBehaviour
{
    private Vector3 _dragOffset;
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCamera(Camera camera)
    {
        this._camera = camera;
        this.transform.position = this._camera.transform.position;
        this.transform.rotation = this._camera.transform.rotation;
    }

    /// <summary>
    /// Confirms the placement of current camera
    /// </summary>
    public Camera Confirm()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes the current camera
    /// </summary>
    public void Delete()
    {
        // Destroy(this._camera.gameObject);
        Destroy(this.gameObject);
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
