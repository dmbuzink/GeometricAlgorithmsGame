using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DefaultNamespace;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class VerticalDecompositionTest
    {
        [Test]
        public void RetrievesLines()
        {
            // Test case: https://puu.sh/IBMYK/657202211f.png
            Task.Run(async () =>
            {
                List<Segment> lines = new List<Segment>();
                Segment a = new Segment(new Vertex(1, 1), new Vertex(3, 2));
                Segment b = new Segment(new Vertex(5, 3), new Vertex(1, 4));
                Segment c = new Segment(new Vertex(6, 1), new Vertex(2, 3));
                Segment d = new Segment(new Vertex(5, 2), new Vertex(8, 4));
                Segment e = new Segment(new Vertex(9, 2), new Vertex(9, 5));
                Segment f = new Segment(new Vertex(10, 6), new Vertex(1, 6));
                lines.AddRange(new [] { a, b, c, d, e, f });

                Vertex Q1 = new Vertex(3, 1);
                Vertex Q2 = new Vertex(2, 2);
                Vertex Q3 = new Vertex(3, 3);
                Vertex Q4 = new Vertex(5, 4);
                Vertex Q5 = new Vertex(3, 5);
                Vertex Q6 = new Vertex(5, 7);
                Vertex Q7 = new Vertex(6, 4);
                Vertex Q8 = new Vertex(10, 5);


                VerticalDecomposition<Segment> decomposition =
                    await VerticalDecomposition<Segment>.CreateVerticalDecomposition(lines);
                
                Segment R1 = decomposition.GetSegment(Q1);
                Segment R2 = decomposition.GetSegment(Q2);
                Segment R3 = decomposition.GetSegment(Q3);
                Segment R4 = decomposition.GetSegment(Q4);
                Segment R5 = decomposition.GetSegment(Q5);
                Segment R6 = decomposition.GetSegment(Q6);
                Segment R7 = decomposition.GetSegment(Q7);
                Segment R8 = decomposition.GetSegment(Q8);
                

                Assert.AreEqual(null, R1);
                Assert.AreEqual(a, R2);
                Assert.AreEqual(c, R3);
                Assert.AreEqual(d, R4);
                Assert.AreEqual(b, R5);
                Assert.AreEqual(f, R6);
                Assert.AreEqual(d, R7);
                Assert.AreEqual(null, R8);
            }).GetAwaiter().GetResult();
        }


        [Test]
        public void WorksForConnectedEdges()
        {
            // Test case: https://puu.sh/IBNfq/2c8b5c574a.png
            Task.Run(async () =>
            {
                Vertex P1 = new Vertex(2, 1);
                Vertex P2 = new Vertex(3, 1);
                Vertex P3 = new Vertex(4, 2);
                Vertex P4 = new Vertex(4, 3);
                Vertex P5 = new Vertex(3, 4);
                Vertex P6 = new Vertex(2, 4);
                Vertex P7 = new Vertex(1, 3);
                Vertex P8 = new Vertex(1, 2);


                List<Segment> lines = new List<Segment>();
                Segment a = new Segment(P1, P2);
                Segment b = new Segment(P2, P3);
                Segment c = new Segment(P3, P4);
                Segment d = new Segment(P4, P5);
                Segment e = new Segment(P5, P6);
                Segment f = new Segment(P6, P7);
                Segment g = new Segment(P7, P8);
                Segment h = new Segment(P8, P1);
                lines.AddRange(new [] { a, b, c, d, e, f, g, h });

                Vertex Q1 = new Vertex(1.5, 0.5);
                Vertex Q2 = new Vertex(2.5, 0.5);
                Vertex Q3 = new Vertex(3.5, 0.5);
                Vertex Q4 = new Vertex(1.5, 2);
                Vertex Q5 = new Vertex(2.5, 2);
                Vertex Q6 = new Vertex(3.5, 2);
                Vertex Q7 = new Vertex(1.5, 5);
                Vertex Q8 = new Vertex(2.5, 5);
                Vertex Q9 = new Vertex(3.5, 5);


                VerticalDecomposition<Segment> decomposition =
                    await VerticalDecomposition<Segment>.CreateVerticalDecomposition(lines);
;
                Segment R1 = decomposition.GetSegment(Q1);
                Segment R2 = decomposition.GetSegment(Q2);
                Segment R3 = decomposition.GetSegment(Q3);
                Segment R4 = decomposition.GetSegment(Q4);
                Segment R5 = decomposition.GetSegment(Q5);
                Segment R6 = decomposition.GetSegment(Q6);
                Segment R7 = decomposition.GetSegment(Q7);
                Segment R8 = decomposition.GetSegment(Q8);
                Segment R9 = decomposition.GetSegment(Q9);


                Assert.AreEqual(null, R1);
                Assert.AreEqual(null, R2);
                Assert.AreEqual(null, R3);
                Assert.AreEqual(h, R4);
                Assert.AreEqual(a, R5);
                Assert.AreEqual(b, R6);
                Assert.AreEqual(f, R7);
                Assert.AreEqual(e, R8);
                Assert.AreEqual(d, R9);
            }).GetAwaiter().GetResult();
        }
    }
}