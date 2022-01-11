using System.Collections.Generic;

namespace DefaultNamespace
{
    public class CameraFace : SimplePolygon
    {
        public List<Camera> Cameras { get; }

        public CameraFace(List<Camera> cameras, IEnumerable<Vertex> points) : base(points)
        {
            this.Cameras = cameras;
        }
    }
}