using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public static class FaceCombiner
    {
        /// <summary>
        /// Takes a collection of overlapping polygons and returns a collection of non-overlapping faces that retain the data of what polygons make up what face
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <returns></returns>
        public static List<CombinedFace<F>> CombineFaces<F>(IEnumerable<F> faces) where F : SimplePolygon
        {
            // Define our data structures
            BinarySearchTree<FaceEvent> eventQueue = new BinarySearchTree<FaceEvent>((a, b) =>
            {
                // First order based on y-value followed by x-value
                // (which simulates sorting by y with a tiny count-clockwise rotation applied to all points such that no two points have the same y-value)
                if (a.Point.Y != b.Point.Y) return Math.Sign(a.Point.Y - b.Point.Y);
                if (a.Point.X != b.Point.X) return Math.Sign(a.Point.X - b.Point.X);

                // For events with the exact same coordinates prioritize edge crossing events
                if (a is FaceCrossEvent<F> && !(b is FaceCrossEvent<F>)) return -1;
                if (!(a is FaceCrossEvent<F>) && b is FaceCrossEvent<F>) return 1;

                // Finally for equivalent events use the ID to still uniquely identify the event, but order them arbitrarily
                return a.ID - b.ID;
            });

            Func<IntervalBoundary<F>, IntervalBoundary<F>, int> getBoundarySide = (a, b) =>
            {
                int aStartSide = b.GetSideOfPoint(a.StartPoint);
                int aEndSide = b.GetSideOfPoint(a.EndPoint);

                // aEndSide != -aStartSide prioritizes b's result in case a's points are on opposite sides of b
                if (aStartSide != 0 && aEndSide != -aStartSide) return aStartSide;
                if (aEndSide != 0 && aEndSide != -aStartSide) return aEndSide;

                int bStartSide = a.GetSideOfPoint(b.StartPoint);
                if (bStartSide != 0) return -bStartSide;
                int bEndSide = a.GetSideOfPoint(b.EndPoint);
                if (bEndSide != 0) return -bEndSide;

                return 0;
            };
            BinarySearchTree<FaceInterval<F>> scanLine = new BinarySearchTree<FaceInterval<F>>((ai, bi) =>
            {
                IntervalBoundary<F> a = ai.Left;
                IntervalBoundary<F> b = bi.Left;

                // Only the left most interval has no left boundary, of which only 1 can exist
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                // Check whether a is left of b, and make sure that b doesn't also think it's left of a (if it does, the lines are aligned)
                int sideAOfB = getBoundarySide(a, b);
                int sideBOfA = getBoundarySide(b, a);
                if (sideAOfB != 0 && sideAOfB == -sideBOfA)
                    return sideAOfB;

                // Sort all intervals with the same left boundary arbitrarily by ID
                return a.ID - b.ID;
            });

            HashSet<MonotonePolygonSection<F>> sections = new HashSet<MonotonePolygonSection<F>>();

            // Initialize the datastructures
            foreach (F face in faces)
            {
                List<FaceEvent> events = FaceCombiner.GetFaceEvents(face);
                foreach(FaceEvent faceEvent in events) eventQueue.Insert(faceEvent);
            }

            FaceInterval<F> outerInterval = new FaceInterval<F>(
                null,
                null,
                new List<F>()
            );
            scanLine.Insert(outerInterval);

            // Handle the events
            FaceEvent evt;
            while ((evt = eventQueue.GetMin())!=null)
            {
                eventQueue.Delete(evt);

                if (evt is FaceCrossEvent<F>)
                {
                    FaceCombiner.HandleCrossEvent(evt as FaceCrossEvent<F>, scanLine, eventQueue);
                }
                else
                {
                    // Collect all non-crossing events at this point
                    Vertex point = evt.Point;
                    List<FaceEvent> eventsAtPoint = new List<FaceEvent>();
                    eventsAtPoint.Add(evt);

                    FaceEvent nextEvent;
                    while ((nextEvent = eventQueue.GetMin()) != null)
                    {
                        if (nextEvent is FaceCrossEvent<F>) break;
                        if (!point.Equals(nextEvent.Point)) break;

                        eventQueue.Delete(nextEvent);
                        eventsAtPoint.Add(nextEvent);
                    }
                    
                    FaceCombiner.HandleEvents(eventsAtPoint, scanLine, sections, eventQueue);
                }
            }

            if (scanLine.GetAll().Count != 1)
                throw new InvalidDataException(
                    "Scanline should only contain a single interval when finished");

            // Transform the polygon sections to simple polygons and return them
            return FaceCombiner.GenerateFaces(sections);
        }

        /// <summary>
        /// Retrieves all events for a given face
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <returns></returns>
        private static List<FaceEvent> GetFaceEvents<F>(F face) where F: SimplePolygon
        {
            List<FaceEvent> events = new List<FaceEvent>();

            int l = face.Vertices.Length;
            Vertex prev = face.Vertices[l - 2];
            Vertex point = face.Vertices[l - 1];

            foreach (Vertex next in face.Vertices)
            {
                bool isLeftTurn = new Segment(prev, point).GetSideOfPoint(next) == -1;

                // Identify the type of the point (using a symbolic counter-clockwise rotation for horizontal lines)
                PointType? type = null;
                if (isLeftTurn)
                {
                    if (point.Y < prev.Y && point.Y <= next.Y) type = PointType.start;
                    else if (point.Y > prev.Y && point.Y >= next.Y) type = PointType.stop;
                }
                else
                {
                    if (point.Y <= prev.Y && point.Y < next.Y) type = PointType.split;
                    else if (point.Y >= prev.Y && point.Y > next.Y) type = PointType.merge;
                }

                if (type == null)
                {
                    if (next.Y > point.Y) type = PointType.rightContinue;
                    else if (next.Y < point.Y) type = PointType.leftContinue;
                    else if (next.X > point.X) type = PointType.rightContinue;
                    else type = PointType.leftContinue;
                }

                // Create the events
                if (type == PointType.start || type == PointType.split)
                {
                    events.Add(new FaceEdgeEvent<F>(
                        point,
                        prev,
                        true,
                        face
                    )); 
                    events.Add(new FaceEdgeEvent<F>(
                        point,
                        next,
                        false,
                        face
                    ));
                } else if (type == PointType.merge || type == PointType.stop)
                {
                    events.Add(new FaceEndEvent(point));
                }
                else
                {
                    events.Add(new FaceEdgeEvent<F>(
                        point,
                        type== PointType.leftContinue ? prev : next,
                        type == PointType.leftContinue,
                        face
                    ));
                }

                // Update the points
                prev = point;
                point = next;
            }

            return events;
        }
        
        /// <summary>
        /// Handles crossing events by stopping the crossing segments at their intersection,
        /// and adding new events to continue them after the intersection
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="evt"></param>
        /// <param name="scanLine"></param>
        /// <param name="eventQueue"></param>
        private static void HandleCrossEvent<F>(
            FaceCrossEvent<F> evt,
            BinarySearchTree<FaceInterval<F>> scanLine,
            BinarySearchTree<FaceEvent> eventQueue
        ) where F : SimplePolygon
        {
            FaceInterval<F> interval = evt.Interval;
            Vertex point = evt.Point;
            IntervalBoundary<F> oldLeft = interval.Left;
            IntervalBoundary<F> oldRight = interval.Right;
            if (oldLeft == null || oldRight == null)
                throw new InvalidDataException(
                    "Reached a supposedly unreachable state, no crossing event for an interval missing one of its boundaries can exist");

            // Update the boundaries intervals to no longer cross
            interval.Left = new IntervalBoundary<F>(
                oldLeft.StartPoint, 
                point, 
                oldLeft.Source, 
                oldLeft.IsLeftWall
            );
            interval.Right = new IntervalBoundary<F>(
                oldRight.StartPoint,
                point,
                oldRight.Source,
                oldRight.IsLeftWall
            );

            // Update the boundaries of neighboring intervals
            FaceInterval<F> prevInterval = scanLine.FindPrevious(interval);
            FaceInterval<F> nextInterval = scanLine.FindNext(interval);
            if (prevInterval == null || nextInterval == null)
                throw new InvalidDataException(
                    "Reached a supposedly unreachable state, a boundary should always be shared by two intervals");
            prevInterval.Right = interval.Left;
            nextInterval.Left = interval.Right;

            // Create new line events to continue the intersecting lines
            FaceEdgeEvent<F> newLeftEvent = new FaceEdgeEvent<F>(
                point,
                oldLeft.EndPoint,
                oldLeft.IsLeftWall,
                oldLeft.Source
            );
            FaceEdgeEvent<F> newRightEvent = new FaceEdgeEvent<F>(
                point,
                oldRight.EndPoint,
                oldRight.IsLeftWall,
                oldRight.Source
            );
            eventQueue.Insert(newLeftEvent);
            eventQueue.Insert(newRightEvent);
        }

        /// <summary>
        /// Handles a set of events at the same point by removing old intervals and adding the new intervals
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="events"></param>
        /// <param name="scanLine"></param>
        /// <param name="sections"></param>
        /// <param name="eventQueue"></param>
        public static void HandleEvents<F>(
            List<FaceEvent> events,
            BinarySearchTree<FaceInterval<F>> scanLine,
            HashSet<MonotonePolygonSection<F>> sections,
            BinarySearchTree<FaceEvent> eventQueue
        ) where F : SimplePolygon
        {
            Vertex point = events[0].Point; // The event points are all the same
            List<FaceInterval<F>> intervalsWithPoint = scanLine.FindRange(
                FaceCombiner.GetIntervalFinder<F>(point, -1),
                FaceCombiner.GetIntervalFinder<F>(point, 1)
            );
            if (intervalsWithPoint.Count == 0)
                throw new InvalidDataException(
                    "Reached a supposedly unreachable state, a point should always fall within some interval");

            // Collect all boundaries that start in the given point
            List<IntervalBoundary<F>> newBoundaries = FaceCombiner.GetContinuationsThroughPoint(
                point,
                intervalsWithPoint
            );
            foreach (FaceEvent evt in events)
            {
                if (evt is FaceEdgeEvent<F> edgeEvent)
                {
                    newBoundaries.Add(new IntervalBoundary<F>(
                        evt.Point,
                        edgeEvent.End,
                        edgeEvent.Source,
                        edgeEvent.IsLeftWall
                    ));
                }
            }

            // Finish old intervals that stop at the given point
            for (int i = 1; i < intervalsWithPoint.Count - 1; i++)
            {
                FaceInterval<F> internalInterval = intervalsWithPoint[i];
                internalInterval.Shape.Left.Add(point);
                internalInterval.Shape.Right.Add(point);
                FaceCombiner.RemoveInterval(internalInterval, scanLine, eventQueue);
            }

            // Create the new intervals and or update old intervals
            FaceInterval<F> leftInterval = intervalsWithPoint[0];
            FaceInterval<F> rightInterval = intervalsWithPoint[intervalsWithPoint.Count-1];
            if (newBoundaries.Count > 0)
            {
                (
                    IntervalBoundary<F> leftBoundary,
                    IntervalBoundary<F> rightBoundary,
                    List<FaceInterval<F>> newIntervals
                ) = FaceCombiner.GetNewIntervals(
                    newBoundaries,
                    leftInterval.Shape.Sources
                );

                FaceInterval<F> newLeftInterval;
                FaceInterval<F> newRightInterval;

                if (leftInterval == rightInterval)
                {
                    FaceCombiner.RemoveInterval(leftInterval, scanLine, eventQueue);

                    List<Vertex> lsl = leftInterval.Shape.Left; // Leftinterval Shape Left
                    newLeftInterval = new FaceInterval<F>(
                        leftInterval.Left,
                        leftBoundary,
                        leftInterval.Shape.Sources
                    );
                    MonotonePolygonSection<F> newLeftShape = newLeftInterval.Shape;
                    if (lsl.Count>0) newLeftShape.Left.Add(lsl[lsl.Count-1]);
                    newLeftShape.Right.Add(point);

                    newLeftShape.BottomLeft = leftInterval.Shape;
                    leftInterval.Shape.TopLeft = newLeftShape;

                    List<Vertex> rsr = rightInterval.Shape.Right; // Rightinterval Shape Right
                    newRightInterval = new FaceInterval<F>(
                        rightBoundary,
                        rightInterval.Right,
                        rightInterval.Shape.Sources
                    );
                    MonotonePolygonSection<F> newRightShape = newRightInterval.Shape;

                    if (rsr.Count > 0) newRightShape.Right.Add(rsr[rsr.Count - 1]);
                    newRightShape.Left.Add(point);

                    newRightShape.BottomRight = rightInterval.Shape;
                    rightInterval.Shape.TopRight = newRightShape;

                    FaceCombiner.AddInterval(newLeftInterval, scanLine, sections, eventQueue, point);
                    FaceCombiner.AddInterval(newRightInterval, scanLine, sections, eventQueue, point);
                }
                else
                {
                    leftInterval.Right = leftBoundary;
                    rightInterval.Left = rightBoundary;

                    leftInterval.Shape.Right.Add(point);
                    rightInterval.Shape.Left.Add(point);

                    if (leftInterval.Intersection != null)
                    {
                        eventQueue.Delete(leftInterval.Intersection);
                        leftInterval.Intersection = null;
                    }
                    if (rightInterval.Intersection != null)
                    {
                        eventQueue.Delete(rightInterval.Intersection);
                        rightInterval.Intersection = null;
                    }

                    FaceCombiner.CheckIntersections(leftInterval, eventQueue, point.Y);
                    FaceCombiner.CheckIntersections(rightInterval, eventQueue, point.Y);
                }

                foreach (FaceInterval<F> interval in newIntervals)
                    FaceCombiner.AddInterval(interval, scanLine, sections, eventQueue, point);
            }
            else
            {
                FaceCombiner.RemoveInterval(leftInterval, scanLine, eventQueue);
                FaceCombiner.RemoveInterval(rightInterval, scanLine, eventQueue);

                leftInterval.Shape.Right.Add(point);
                rightInterval.Shape.Left.Add(point);

                FaceInterval<F> newInterval = new FaceInterval<F>(
                    leftInterval.Left, 
                    rightInterval.Right, 
                    leftInterval.Shape.Sources
                );

                List<Vertex> lsl = leftInterval.Shape.Left; // Leftinterval Shape Left
                List<Vertex> rsr = rightInterval.Shape.Right; // Rightinterval Shape Right
                if (lsl.Count > 0) newInterval.Shape.Left.Add(lsl[lsl.Count-1]);
                if (rsr.Count > 0) newInterval.Shape.Right.Add(rsr[rsr.Count - 1]);

                leftInterval.Shape.TopLeft = newInterval.Shape;
                rightInterval.Shape.TopRight = newInterval.Shape;
                newInterval.Shape.BottomLeft = leftInterval.Shape;
                newInterval.Shape.BottomRight = rightInterval.Shape;

                FaceCombiner.AddInterval(newInterval, scanLine, sections, eventQueue, point);
            }
        }



        /// <summary>
        /// Generates the final polygons given a set of monotone polygon sections
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<CombinedFace<F>> GenerateFaces<F>(
            HashSet<MonotonePolygonSection<F>> sections
        ) where F : SimplePolygon
        {
            List<CombinedFace<F>> output = new List<CombinedFace<F>>();

            HashSet<MonotonePolygonSection<F>> sectionsCopy = new HashSet<MonotonePolygonSection<F>>(sections);
            foreach (MonotonePolygonSection<F> section in sectionsCopy)
            {
                if (section.Sources.Count == 0) continue;

                List<Vertex> points = new List<Vertex>();
                FaceCombiner.ExploreSections<F>(section, null, sections, points);
                if (points.Count <= 0) continue;

                List<Vertex> noDuplicatePoints = new List<Vertex>();
                Vertex prevPoint = points[points.Count-1];
                foreach (Vertex point in points)
                {
                    if(!prevPoint.Equals(point))
                        noDuplicatePoints.Add(point);
                    prevPoint = point;
                }

                if (noDuplicatePoints.Count <= 2) continue;

                output.Add(new CombinedFace<F>(
                    section.Sources,
                    noDuplicatePoints
                ));
            }

            return output;
        }

        /// <summary>
        /// Recuresively explores a given section and adds the found points to the list
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="section"></param>
        /// <param name="parent"></param>
        /// <param name="remainingSections"></param>
        /// <param name="points"></param>
        public static void ExploreSections<F>(
            MonotonePolygonSection<F> section,
            MonotonePolygonSection<F> parent,
            HashSet<MonotonePolygonSection<F>> remainingSections,
            List<Vertex> points) 
        where F : SimplePolygon
        {
            if (!remainingSections.Contains(section)) return;
            remainingSections.Remove(section);

            MonotonePolygonSection<F> topLeft = section.TopLeft;
            MonotonePolygonSection<F> topRight = section.TopRight;
            MonotonePolygonSection<F> bottomLeft = section.BottomLeft;
            MonotonePolygonSection<F> bottomRight = section.BottomRight;

            int start = 0;
            if (parent != null)
            {
                if (topLeft == parent) start = 0;
                else if (bottomLeft == parent) start = 1;
                else if (bottomRight == parent) start = 2;
                else if (topRight == parent) start = 3;
            }

            for (int i = 0; i < 4; i++)
            {
                int side = (i + start) % 4;
                if (side == 0)
                {
                    if(topLeft != null && topLeft != parent)
                        ExploreSections<F>(topLeft, section, remainingSections, points);
                } else if (side == 1)
                {
                    if (bottomLeft != null && bottomLeft != parent)
                        ExploreSections<F>(bottomLeft, section, remainingSections, points);
                } else if (side == 2)
                {
                    if (bottomRight != null && bottomRight != parent)
                        ExploreSections<F>(bottomRight, section, remainingSections, points);
                }
                else if (side == 3)
                {
                    if (topRight != null && topRight != parent)
                        ExploreSections<F>(topRight, section, remainingSections, points);
                }

                if (side == 0)
                {
                    List<Vertex> leftPoints = new List<Vertex>(section.Left);
                    leftPoints.Reverse();
                    points.AddRange(leftPoints);
                } 
                else if(side==2) points.AddRange(section.Right);
            }

        }

        /// <summary>
        /// Cuts all boundaries that go through the given point and retrieves new boundaries that represent the continuations of the cut lines
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="point"></param>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static List<IntervalBoundary<F>> GetContinuationsThroughPoint<F>(
            Vertex point,
            List<FaceInterval<F>> intervals
        ) where F : SimplePolygon
        {
            List<IntervalBoundary<F>> cutOff = new List<IntervalBoundary<F>>();

            for (int i = 1; i < intervals.Count; i++)
            {
                FaceInterval<F> interval = intervals[i];
                IntervalBoundary<F> left = interval.Left;

                if (left != null)
                {
                    bool intersects = !point.Equals(left.EndPoint);

                    if (intersects)
                    {
                        IntervalBoundary<F> newBoundary = new IntervalBoundary<F>(
                            point,
                            left.EndPoint,
                            left.Source,
                            left.IsLeftWall
                        );
                        cutOff.Add(newBoundary);
                    }
                }
            }

            return cutOff;
        }

        /// <summary>
        /// Retrieves the newly created intervals defined by the given boundaries
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="boundaries"></param>
        /// <param name="startSources"></param>
        /// <returns>The new left most boundary, right most boundary, and internal intervals</returns>
        public static (
            IntervalBoundary<F>,
            IntervalBoundary<F>,
            List<FaceInterval<F>>
        ) GetNewIntervals<F>(
            List<IntervalBoundary<F>> boundaries,
            List<F> startSources
        ) where F : SimplePolygon
        {
            Vertex point = boundaries[0].StartPoint; // The start points are all the same
            FaceCombiner.SortBoundaries(boundaries);

            List<FaceInterval<F>> newIntervals = new List<FaceInterval<F>>();

            IntervalBoundary<F> prevBoundary = boundaries[0];
            List<F> sources = startSources;
            for (int i = 1; i < boundaries.Count; i++)
            {
                IntervalBoundary<F> boundary = boundaries[i];
                sources = FaceCombiner.AugmentSources(sources, prevBoundary);

                FaceInterval<F> newInterval = new FaceInterval<F>(
                    prevBoundary,
                    boundary,
                    sources
                );
                newInterval.Shape.Left.Add(point);
                newInterval.Shape.Right.Add(point);
                newIntervals.Add(newInterval);

                prevBoundary = boundary;
            }

            return (
                boundaries[0],
                boundaries[boundaries.Count - 1],
                newIntervals
            );
        }

        /// <summary>
        /// Checks whether the walls of a given boundary will intersect, and adds an intersection event if so
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="interval"></param>
        /// <param name="eventQueue"></param>
        /// <param name="minY"></param>
        public static void CheckIntersections<F>(
            FaceInterval<F> interval,
            BinarySearchTree<FaceEvent> eventQueue,
            double minY
        ) where F : SimplePolygon
        {
            if (interval.Left == null || interval.Right == null) return;

            if (interval.Left.Intersects(interval.Right))
            {
                Vertex intersect = interval.Left.GetIntersectionPoint(interval.Right);
                if (intersect.Y < minY)
                {
                    // Deals with a situation that can only occur because of rounding errors
                    intersect.Y = minY;
                }
                

                FaceCrossEvent<F> crossEvent = new FaceCrossEvent<F>(interval, intersect);
                interval.Intersection = crossEvent;
                eventQueue.Insert(crossEvent);
            }
        }

        /// <summary>
        /// Remove a given interval from the scanline as well as any possible associated intersection event
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="interval"></param>
        /// <param name="scanLine"></param>
        /// <param name="eventQueue"></param>
        public static void RemoveInterval<F>(
            FaceInterval<F> interval,
            BinarySearchTree<FaceInterval<F>> scanLine,
            BinarySearchTree<FaceEvent> eventQueue
        ) where F : SimplePolygon
        {
            scanLine.Delete(interval);
            if (interval.Intersection != null) 
                eventQueue.Delete(interval.Intersection);
        }


        /// <summary>
        /// Adds a given interval to the scanline and possible adds its corresponding intersection event
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="interval"></param>
        /// <param name="scanLine"></param>
        /// <param name="sections"></param>
        /// <param name="eventQueue"></param>
        /// <param name="eventPoint"></param>
        public static void AddInterval<F>(
            FaceInterval<F> interval,
            BinarySearchTree<FaceInterval<F>> scanLine,
            HashSet<MonotonePolygonSection<F>> sections,
            BinarySearchTree<FaceEvent> eventQueue,
            Vertex eventPoint
        ) where F : SimplePolygon
        {
            scanLine.Insert(interval);
            sections.Add(interval.Shape);

            FaceCombiner.CheckIntersections(interval, eventQueue, eventPoint.Y);
        }

        /// <summary>
        /// Retrieves a function that can be used on a binary search tree to find the interval that the given point lies in
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="segment"></param>
        /// <param name="steer">
        /// What side of the interval to steer to,
        ///     0 finds an interval it's in,
        ///     -1 points towards the left of the left-most interval it's in,
        ///     1 points towards the right of the right-most interval it's in
        ///
        /// Note that a point may fall in multiple intervals, because intervals may have 0 width and the point can lie on boundaries. 
        /// </param>
        /// <returns></returns>
        public static Func<FaceInterval<F>, int> GetIntervalFinder<F>(Vertex point, int steer) where F: SimplePolygon
        {
            return (i) =>
            {
                if (i.Left != null)
                {
                    int side = i.Left.GetSideOfPoint(point);
                    if (side == -1) return -1;
                }

                if (i.Right != null)
                {
                    int side = i.Right.GetSideOfPoint(point);
                    if (side == 1) return 1;
                }

                return steer;
            };
        }

        /// <summary>
        /// Retrieves the new list of sources given a list of sources on the left of a boundary, and a boundary to be crossed to the right
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="sources"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        public static List<F> AugmentSources<F>(
            List<F> sources,
            IntervalBoundary<F> boundary
        ) where F : SimplePolygon
        {
            if (boundary.IsLeftWall)
            {
                List<F> newSources = new List<F>(sources);
                newSources.Add(boundary.Source);
                return newSources;
            }
            else
            {
                return sources.FindAll(source => source != boundary.Source);
            }
        }

        /// <summary>
        /// Sorts the given bouncaries from left to right
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="boundaries"></param>
        public static void SortBoundaries<F>(
            List<IntervalBoundary<F>> boundaries
        ) where F: SimplePolygon
        {
            boundaries.Sort((a, b) =>
            {
                int startSide = b.GetSideOfPoint(a.StartPoint);
                if (startSide != 0) return startSide;
                int endSide = b.GetSideOfPoint(a.EndPoint);
                if (endSide != 0) return endSide;
                return a.ID - b.ID;
            });
        }

        /// <summary>
        /// A type to label the different event points
        /// </summary>
        private enum PointType
        {
            start,
            split,
            leftContinue,
            rightContinue,
            stop,
            merge
        }
    }

    
}