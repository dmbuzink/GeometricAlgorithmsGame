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

    /// <summary>
    /// OnMouseDrag event which handles the rotation based on the player's dragging actions.
    /// </summary>
    private void OnMouseDrag()
    {
        var currentAngle = GeometricHelper.AngleBetweenPoints(Camera.transform.position, GetMousePosition());
        this._cameraPlacer.transform.Rotate(new Vector3(0f, 0f, currentAngle - _prevAngle));
        this._prevAngle = currentAngle;
    }

    /// <summary>
    /// Gets the mouse position in world space.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMousePosition()
    {
        var inputMousePosition = Input.mousePosition;
        var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(inputMousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
}
