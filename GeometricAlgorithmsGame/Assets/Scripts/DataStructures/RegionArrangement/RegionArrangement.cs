
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefaultNamespace
{
    public class RegionArrangement<T> where T: SimplePolygon
    {
        private List<T> Regions { get; set; }
        private VerticalDecomposition<PolygonSegment<T>> Decomposition { get; set; }

        private RegionArrangement(List<T> regions)
        {
            this.Regions = regions;
        }

        protected static async Task<RegionArrangement<T>> CreateRegionArrangement(List<T> regions)
        {
            RegionArrangement<T> decomposition = new RegionArrangement<T>(regions);

            List<PolygonSegment<T>> segments = new List<PolygonSegment<T>>();
            foreach (T region in regions)
            {
                Vertex prev = region.Vertices[region.Vertices.Length - 1];
                foreach (Vertex point in region)
                {
                    if(point.X > prev.X) 
                        segments.Add(new PolygonSegment<T>(prev, point, region));

                    prev = point;
                }
            }

            decomposition.Decomposition = 
                await VerticalDecomposition<PolygonSegment<T>>.CreateVerticalDecomposition(segments);

            return decomposition;
        }

        public T FindRegion(Vertex point)
        {
            return this.Decomposition.GetSegment(point)?.Polygon;
        }

        public Dictionary<T, double> CalculateAreas()
        {

            List<Trapezoid<PolygonSegment<T>>> trapezoids = new List<Trapezoid<PolygonSegment<T>>>();
            this.Decomposition.Root.GetTrapezoids(trapezoids);

            Dictionary<T, double> areas = new Dictionary<T, double>();
            foreach (Trapezoid<PolygonSegment<T>> trapezoid in trapezoids)
            {
                double oldArea = areas[trapezoid.Bottom.Polygon];
                areas.Add(
                    trapezoid.Bottom.Polygon,
                    trapezoid.GetArea() + oldArea
                );
            }

            return areas;
        }
    }
}