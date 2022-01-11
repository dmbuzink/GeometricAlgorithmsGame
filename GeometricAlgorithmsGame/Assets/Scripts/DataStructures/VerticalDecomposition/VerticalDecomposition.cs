using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefaultNamespace
{
    public class VerticalDecomposition<T> where T: Segment
    {
        private Random rng = new Random(3);
        public VerticalDecompositionNode<T> Root {  get; private set; }
        

        /// <summary>
        /// Returns the segment the given vertex is either on or directly above.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public T GetSegment(Vertex vertex)
        {
            return this.Root.FindTrapezoid(vertex).Bottom;
        }

        /// <summary>
        /// Creates a vertical decomposition of the given segments
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static async Task<VerticalDecomposition<T>> CreateVerticalDecomposition(List<T> segments)
        {
            VerticalDecomposition<T> decomposition = new VerticalDecomposition<T>();
            decomposition.Shuffle(segments);
            decomposition.Root = new Trapezoid<T>(null, null, null, null);

            foreach(T segment in segments)
                decomposition.AddSegment(segment);

            return decomposition;
        }

        /// <summary>
        /// Adds a new segment to the decomposition
        /// </summary>
        /// <param name="segment"></param>
        public void AddSegment(T segment)
        {
            Segment oriented = segment.GetOriented();
            Vertex edgeLeft = oriented.StartPoint;
            Vertex edgeRight = oriented.EndPoint;

            List<Trapezoid<T>> trapezoids = this.FindIntersectingTrapezoids(oriented);

            Trapezoid<T> prev = null;
            Trapezoid<T> prevAbove = null;
            Trapezoid<T> prevBelow = null;

            for (int i = 0; i < trapezoids.Count; i++)
            {
                Trapezoid<T> trapezoid = trapezoids[i];
                bool isFirst = i == 0;
                bool isLast = i + 1 == trapezoids.Count;

                Trapezoid<T> below;
                Trapezoid<T> above;
                Trapezoid<T> startTrapezoid = null;
                Trapezoid<T> endTrapezoid = null;

                bool containsLeft = isFirst && (
                    trapezoid.Left == null
                    || !edgeLeft.Equals(trapezoid.Left)
                );
                bool containsRight = isLast && (
                    trapezoid.Right == null
                    || !edgeRight.Equals(trapezoid.Right)
                );

                // Based on the position of the trapezoid, create the appropriate replacement trapezoids and link them
                if (isFirst && isLast)
                {
                    below = new Trapezoid<T>(
                        edgeLeft,
                        edgeRight,
                        trapezoid.Bottom,
                        segment
                    );
                    above = new Trapezoid<T>(
                        edgeLeft,
                        edgeRight,
                        segment,
                        trapezoid.Top
                    );
                    startTrapezoid = this.CreateStart(
                        trapezoid,
                        edgeLeft,
                        containsLeft,
                        above,
                        below
                    );
                    endTrapezoid = this.CreateEnd(
                        trapezoid,
                        edgeRight,
                        containsRight,
                        above,
                        below
                    );
                } else if (isFirst)
                {
                    below = new Trapezoid<T>(
                        edgeLeft,
                        trapezoid.Right,
                        trapezoid.Bottom,
                        segment
                    );
                    above = new Trapezoid<T>(
                        edgeLeft,
                        trapezoid.Right,
                        segment,
                        trapezoid.Top
                    );
                    startTrapezoid = this.CreateStart(
                        trapezoid,
                        edgeLeft,
                        containsLeft,
                        above,
                        below
                    );
                } else if (isLast)
                {
                    below = this.UpdateBelow(
                        prev,
                        prevBelow,
                        trapezoid,
                        segment,
                        edgeRight
                    );
                    above = this.UpdateAbove(
                        prev,
                        prevAbove,
                        trapezoid,
                        segment,
                        edgeRight
                    );
                    endTrapezoid = this.CreateEnd(
                        trapezoid,
                        edgeRight,
                        containsRight,
                        above,
                        below
                    );
                }
                else
                {
                    below = this.UpdateBelow(
                        prev,
                        prevBelow,
                        trapezoid,
                        segment,
                        trapezoid.Right
                    );
                    above = this.UpdateAbove(
                        prev,
                        prevAbove,
                        trapezoid,
                        segment,
                        trapezoid.Right
                    );
                }

                // Create the search tree node corresponding with the created trapezoids and add to the search tree
                VerticalDecompositionNode<T> candidate = new VerticalDecompositionYNode<T>(
                    oriented,
                    below,
                    above
                );
                if (endTrapezoid != null) 
                    candidate = new VerticalDecompositionXNode<T>(
                        edgeRight, 
                        candidate, 
                        endTrapezoid
                    );
                if (startTrapezoid != null)
                    candidate = new VerticalDecompositionXNode<T>(
                        edgeLeft,
                        startTrapezoid,
                        candidate
                    );
                this.ReplaceTrapezoidInTree(trapezoid, candidate);

                // Update the 'previous' pointers
                prev = trapezoid;
                prevAbove = above;
                prevBelow = below;
            }

        }

        protected Trapezoid<T> CreateStart(
            Trapezoid<T> trapezoid,
            Vertex edgeLeft,
            bool containsLeft,
            Trapezoid<T> above,
            Trapezoid<T> below
        )
        {
            if (containsLeft)
            {
                Trapezoid<T> startTrapezoid = new Trapezoid<T>(
                    trapezoid.Left,
                    edgeLeft,
                    trapezoid.Bottom,
                    trapezoid.Top
                );

                startTrapezoid.LTN = startTrapezoid.LTN;
                startTrapezoid.RTN = above;
                above.LTN = startTrapezoid;

                startTrapezoid.LBN = startTrapezoid.LBN;
                startTrapezoid.RBN = below;
                below.LBN = startTrapezoid;

                if (trapezoid.LTN?.RBN == trapezoid) trapezoid.LTN.RBN = startTrapezoid;
                if (trapezoid.LTN?.RTN == trapezoid) trapezoid.LTN.RTN = startTrapezoid;
                if (trapezoid.LBN?.RBN == trapezoid) trapezoid.LBN.RBN = startTrapezoid;
                if (trapezoid.LBN?.RTN == trapezoid) trapezoid.LBN.RTN = startTrapezoid;
                return startTrapezoid;
            }
            else
            {
                below.LBN = trapezoid.LBN;
                above.LTN = trapezoid.LTN;

                if (trapezoid.LTN?.RBN == trapezoid) trapezoid.LTN.RBN = above;
                if (trapezoid.LTN?.RTN == trapezoid) trapezoid.LTN.RTN = above;
                if (trapezoid.LBN?.RBN == trapezoid) trapezoid.LBN.RBN = below;
                if (trapezoid.LBN?.RTN == trapezoid) trapezoid.LBN.RTN = below;
                return null;
            }
        }

        protected Trapezoid<T> CreateEnd(
            Trapezoid<T> trapezoid,
            Vertex edgeRight,
            bool containsRight,
            Trapezoid<T> above,
            Trapezoid<T> below
        )
        {
            if (containsRight)
            {
                Trapezoid<T> endTrapezoid = new Trapezoid<T>(
                    edgeRight,
                    trapezoid.Right,
                    trapezoid.Bottom,
                    trapezoid.Top
                );

                endTrapezoid.RTN = endTrapezoid.RTN;
                endTrapezoid.LTN = above;
                above.RTN = endTrapezoid;

                endTrapezoid.RBN = endTrapezoid.RBN;
                endTrapezoid.LBN = below;
                below.RBN = endTrapezoid;

                if (trapezoid.RTN?.LBN == trapezoid) trapezoid.RTN.LBN = endTrapezoid;
                if (trapezoid.RTN?.LTN == trapezoid) trapezoid.RTN.LTN = endTrapezoid;
                if (trapezoid.RBN?.LBN == trapezoid) trapezoid.RBN.LBN = endTrapezoid;
                if (trapezoid.RBN?.LTN == trapezoid) trapezoid.RBN.LTN = endTrapezoid;
                return endTrapezoid;
            }
            else
            {
                below.RBN = trapezoid.RBN;
                above.RTN = trapezoid.RTN;

                if (trapezoid.RTN?.LBN == trapezoid) trapezoid.RTN.LBN = above;
                if (trapezoid.RTN?.LTN == trapezoid) trapezoid.RTN.LTN = above;
                if (trapezoid.RBN?.LBN == trapezoid) trapezoid.RBN.LBN = below;
                if (trapezoid.RBN?.LTN == trapezoid) trapezoid.RBN.LTN = below;
                return null;
            }
        }

        protected Trapezoid<T> UpdateAbove(
            Trapezoid<T> prev,
            Trapezoid<T> prevAbove,
            Trapezoid<T> trapezoid,
            T segment,
            Vertex end
        )
        {
            if (prevAbove?.Top == trapezoid.Top)
            {
                prevAbove.Right = end;
                return prevAbove;
            }
            else
            {
                Trapezoid<T> above = new Trapezoid<T>(
                    trapezoid.Left,
                    end,
                    segment,
                    trapezoid.Top
                );

                above.LBN = prevAbove;
                if (trapezoid.LTN != prev) above.LTN = trapezoid.LTN;

                if (prev?.RTN != null)
                {
                    prevAbove.RTN = prev.RTN;
                    prev.RTN.LTN = prevAbove;
                }
                prevAbove.RBN = above;

                if (trapezoid.LTN != prev && trapezoid.LTN?.RTN == trapezoid)
                    trapezoid.LTN.RTN = above;

                return above;
            }
        }

        protected Trapezoid<T> UpdateBelow(
            Trapezoid<T> prev,
            Trapezoid<T> prevBelow,
            Trapezoid<T> trapezoid,
            T segment,
            Vertex end
        )
        {
            if (prevBelow?.Bottom == trapezoid.Bottom)
            {
                prevBelow.Right = end;
                return prevBelow;
            }
            else
            {
                Trapezoid<T> below = new Trapezoid<T>(
                    trapezoid.Left,
                    end,
                    trapezoid.Bottom,
                    segment
                );

                below.LTN = prevBelow;
                if (trapezoid.LBN != prev) below.LBN = trapezoid.LBN;

                if (prev?.RBN != null)
                {
                    prevBelow.RBN = prev.RBN;
                    prev.RBN.LBN = prevBelow;
                }
                prevBelow.RTN = below;

                if (trapezoid.LBN != prev && trapezoid.LBN?.RBN == trapezoid)
                    trapezoid.LBN.RBN = below;

                return below;
            }
        }

        protected List<Trapezoid<T>> FindIntersectingTrapezoids(Segment segment)
        {
            Trapezoid<T> start = this.Root.FindTrapezoid(segment);
            List<Trapezoid<T>> output = new List<Trapezoid<T>>();
            output.Add(start);

            Trapezoid<T> prev = start;
            while (prev != null && prev.Right?.X < segment.EndPoint.X)
            {
                if (segment.GetSideOfPoint(prev.Right) == 1) prev = prev.RTN ?? prev.RBN;
                else prev = prev.RBN ?? prev.RTN;

                if(prev != null) output.Add(prev);
            }

            return output;
        }

        protected void ReplaceTrapezoidInTree(
            Trapezoid<T> trapezoid,
            VerticalDecompositionNode<T> newNode
        )
        {
            bool isRoot = trapezoid.Parents.Count == 0;
            newNode.LinkNodes(null);

            foreach (VerticalDecompositionNode<T> parent in trapezoid.Parents)
                parent.Replace(trapezoid, newNode);

            if (isRoot) this.Root = newNode;
        }

        public void Shuffle<K>(IList<K> list)
        {
            // Src: https://stackoverflow.com/a/1262619/8521718
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = this.rng.Next(n + 1);
                K value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}