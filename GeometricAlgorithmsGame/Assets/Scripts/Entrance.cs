using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Vertex Position { get; set; }

    public Entrance(Vertex position)
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
