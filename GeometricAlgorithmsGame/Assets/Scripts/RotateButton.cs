using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    private Vector3 _dragOffset;
    [SerializeField] private GameObject _cameraPlacer;
    [SerializeField] private GameObject _unityCamera;
    [SerializeField] private GameObject _camera;
    // private CircleCollider2D _collider;
    private float _prevAngle;

    void Start()
    {
        // this._collider = GetComponent<CircleCollider2D>();
    }

    private void OnMouseDown()
    {
        this._prevAngle = GeometricHelper.AngleBetweenPoints(_camera.transform.position, GetMousePosition());
    }

    private void OnMouseDrag()
    {
        // var rotation = GeometricHelper.AngleBetweenPoints(this.transform.position, GetMousePosition());
        // this._cameraPlacer.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
        // this._collider.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
        // this._cameraPlacer.transform.LookAt(GetMousePosition());
        
        var currentAngle = GeometricHelper.AngleBetweenPoints(_camera.transform.position, GetMousePosition());
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
