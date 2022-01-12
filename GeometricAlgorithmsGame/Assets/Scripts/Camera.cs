using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Camera : MonoBehaviour
{
    public event Action<Camera> onSelected;
    [SerializeField] private BoxCollider2D _selectionCollider;
    
    public Vertex Position { get; set; }
    public double Angle
    {
        get 
        { 
            var z = this.gameObject.transform.rotation.eulerAngles.z;
            return z < Mathf.Epsilon ? 0 : 360 - z; 
        }
    }

    public Camera(Vertex position)
    {
        this.Position = position;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Calculates the the are that can be viewed by the camera based on the floorplan
    /// </summary>
    /// <param name="floorplan"></param>
    /// <returns></returns>
    public async Task<IEnumerable<PolygonVertex>> CalculateView(Floorplan floorplan)
    {
        // TODO: To be implemented by Damian M. Buzink
        throw new NotImplementedException();
    }

    /// <summary>
    /// Registers the selection of this camera and invokes event for it.
    /// </summary>
    public void OnMouseUpAsButton()
    {
        this.onSelected?.Invoke(this);
    }

    /// <summary>
    /// Set the "selection" collider to active or inactive, depedent on parameter.
    /// This is used to handle overlapping colliders. 
    /// </summary>
    /// <param name="isActive"></param>
    public void SetColliderActive(bool isActive)
    {
        this._selectionCollider.enabled = isActive;
    }
}
