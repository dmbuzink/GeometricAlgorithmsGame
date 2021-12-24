using System;
using DefaultNamespace;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public Camera Camera { private get; set; }
    
    private Vector3 _dragOffset;
    [SerializeField] private GameObject _cameraPlacer;
    private float _prevAngle;

    private void OnMouseDown()
    {
        this._prevAngle = GeometricHelper.AngleBetweenPoints(Camera.transform.position, GetMousePosition());
    }

    private void OnMouseDrag()
    {
        // var rotation = GeometricHelper.AngleBetweenPoints(this.transform.position, GetMousePosition());
        // this._cameraPlacer.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
        // this._collider.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
        // this._cameraPlacer.transform.LookAt(GetMousePosition());
        
        var currentAngle = GeometricHelper.AngleBetweenPoints(Camera.transform.position, GetMousePosition());
        this._cameraPlacer.transform.Rotate(new Vector3(0f, 0f, currentAngle - _prevAngle));
        this._prevAngle = currentAngle;
    }

    private Vector3 GetMousePosition()
    {
        var inputMousePosition = Input.mousePosition;
        var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(inputMousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
}
