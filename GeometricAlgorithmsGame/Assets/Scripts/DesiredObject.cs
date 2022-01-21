using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredObject : MonoBehaviour
{
    public Vertex Position { get; set; }

    public DesiredObject(Vertex position)
    {
        this.Position = position;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = this.gameObject.transform.position;
        this.Position = new Vertex(pos.x, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
