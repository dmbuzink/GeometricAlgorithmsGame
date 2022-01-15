using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using NUnit.Framework;

namespace Tests
{
    public class FaceCombinerTest 
    {
        /// <summary>
        /// Verifies whether the given result has the expected structure
        /// </summary>
        /// <param name="expectedShape">The number of points expected in each of the output faces as well as the polygons the face originates from,
        /// where the order of the values is ignored </param>
        /// <param name="result"></param>
        public void VerifyShape((int, SimplePolygon[])[] expectedShape, List<CombinedFace<SimplePolygon>> result)
        {
            Assert.AreEqual(expectedShape.Length, result.Count);

            foreach ((int points, SimplePolygon[] sources) in expectedShape)
            {
                var found = result.Find(data =>
                    data.Sources.Count == sources.Length
                    && sources.AsEnumerable().All(face => data.Sources.Contains(face))
                    && data.Vertices.Length == points
                );

                Assert.NotNull(found);
            }
        }

        [Test]
        public void HandlesSinglePolygons()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                250, 50,
                350, 150,
                300, 250,
                150, 150
            });
            polygons.Add(p1);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (4, new [] {p1}),
            }, result);
        }

        // https://puu.sh/ICmEi/35a9c5eaa9.png
        [Test]
        public void HandlesSharedEndpoint()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                250, 50,
                350, 150,
                300, 250,
                150, 150
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                300, 50,
                400, 150,
                300, 250,
                200, 150
            });
            polygons.Add(p2);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new []
            {
                (5, new [] {p1}),
                (4, new [] {p1, p2}),
                (5, new [] {p2}),
            }, result);
        }

        // https://puu.sh/ICodJ/2dc0d346f4.png
        [Test]
        public void HandlesArbitraryInput()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                50, 50,
                200, 40,
                320, 200,
                250, 300,
                200, 250,
                100, 250,
                40, 350,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                280, 70,
                400, 120,
                450, 200,
                400, 350,
                200, 450,
                230, 400,
                300, 350,
                260, 300,
                200, 270,
            });
            polygons.Add(p2);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (7, new [] {p1}),
                (11, new [] {p2}),
                (5, new [] {p1, p2}),
                (3, new [] {p2}),
                (3, new [] {p1}),
            }, result);
        }

        // https://puu.sh/ICoqE/f528081514.png
        [Test]
        public void HandlesHoles()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                150, 50,
                250, 100,
                250, 200,
                150, 250,
                50, 200,
                50, 100,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                150, 100,
                200, 150,
                150, 200,
                100, 150,
            });
            polygons.Add(p2);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (12, new [] {p1}),
                (4, new [] {p1, p2}),
            }, result);
        }

        // https://puu.sh/ICorz/0cfbcdc937.png
        [Test]
        public void HandlesCommonPointsAndSegmentsWithoutOverlap()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                50, 50,
                150, 50,
                150, 150,
                50, 150,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                150, 50,
                250, 50,
                250, 250,
                50, 250,
                50, 150,
                150, 150,
            });
            polygons.Add(p2);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (4, new [] {p1}),
                (6, new [] {p2}),
            }, result);
        }

        // https://puu.sh/ICosB/a19511f580.png
        [Test]
        public void HandlesCommonPointsAndSegments()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                50, 50,
                150, 50,
                150, 150,
                50, 150,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                150, 50,
                250, 50,
                250, 250,
                50, 250,
                50, 150,
                150, 130,
            });
            polygons.Add(p2);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (4, new [] {p1}),
                (7, new [] {p2}),
                (3, new [] {p1, p2}),
            }, result);
        }

        // https://puu.sh/ICotJ/382a193fc6.png
        [Test]
        public void HandlesMultipleIntersectionsInOnePoint()
        {
            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                100, 100,
                150, 150,
                100, 200,
                50, 150,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                100, 50,
                250, 50,
                250, 250,
                100, 250,
            });
            polygons.Add(p2);
            SimplePolygon p3 = new SimplePolygon(new double[]
            {
                75, 50,
                200, 200,
                125, 150,
            });
            polygons.Add(p3);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (3, new [] {p1}),
                (8, new [] {p2}),
                (3, new [] {p3}),
                (4, new [] {p1, p2}),
                (5, new [] {p2, p3}),
                (4, new [] {p1, p2, p3}),
            }, result);
        }

        // https://puu.sh/ICovP/7cff066e3e.png
        [Test]
        public void HandlesPreventedIntersections()
        {
            // Occurs when a segment turns out to be cut by another line in an earlier point than initially expected by another line

            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                50, 50,
                150, 50,
                150, 200,
                50, 200,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                80, 80,
                220, 180,
                80, 180,
            });
            polygons.Add(p2);
            SimplePolygon p3 = new SimplePolygon(new double[]
            {
                180, 80,
                180, 160,
                100, 160,
            });
            polygons.Add(p3);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (9, new [] {p1}),
                (5, new [] {p2}),
                (4, new [] {p3}),
                (6, new [] {p1, p2}),
                (3, new [] {p1, p3}),
                (4, new [] {p1, p2, p3}),
                (4, new [] {p2, p3}),
            }, result);
        }

        // https://puu.sh/ICoyp/7766411297.png
        [Test]
        public void HandlesComplexSituationOfIntersectionsInOuterPlane()
        {
            // Occurs when a segment turns out to be cut by another line in an earlier point than initially expected by another line

            List<SimplePolygon> polygons = new List<SimplePolygon>();
            SimplePolygon p1 = new SimplePolygon(new double[]
            {
                50, 50,
                200, 50,
                200, 200,
                50, 200,
            });
            polygons.Add(p1);
            SimplePolygon p2 = new SimplePolygon(new double[]
            {
                75, 75,
                175, 75,
                175, 175,
            });
            polygons.Add(p2);
            SimplePolygon p3 = new SimplePolygon(new double[]
            {
                100, 100,
                150, 100,
                100, 150,
            });
            polygons.Add(p3);
            SimplePolygon p4 = new SimplePolygon(new double[]
            {
                150, 100,
                150, 150,
                100, 150,
            });
            polygons.Add(p4);

            var result = FaceCombiner.CombineFaces(polygons);

            VerifyShape(new[]
            {
                (12, new [] {p1}),
                (6, new [] {p1, p2}),
                (3, new [] {p1, p3}),
                (3, new [] {p1, p4}),
                (3, new [] {p1, p2, p3}),
                (3, new [] {p1, p2, p4}),
            }, result);
        }
    }
}