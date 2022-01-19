using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class DebugFace : MonoBehaviour
{
    public CombinedFace<SimplePolygon> Source;
    private LineRenderer _lineRenderer;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        this.color = Random.ColorHSV(0.1f,0.2f,0.7f,1f, 0.7f,1);
    }

    // Update is called once per frame
    void Update()
    {
        this._lineRenderer = gameObject.GetComponent<LineRenderer>();
        this._lineRenderer.loop = true;
        this._lineRenderer.startWidth = 0.1f;
        this._lineRenderer.endWidth = 0.1f;
        
        this._lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        this._lineRenderer.SetColors(this.color, this.color);
        Source.Draw(this._lineRenderer);
    }
}
