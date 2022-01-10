namespace DefaultNamespace
{
    public abstract class VerticalDecompositionNode<T> where T: Segment
    {
        public abstract Trapezoid<T> FindTrapezoid(Vertex point);
        
        /// <summary>
        /// Finds the trapezoid that a given line segment starts in
        /// </summary>
        /// <param name="line">The line to check, which is expected to be oriented form left to right</param>
        /// <returns></returns>
        public abstract Trapezoid<T> FindTrapezoid(Segment line);

        /// <summary>
        /// Replaces the given old node by the given new node, if it's a child of this node
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        public abstract void Replace(
            VerticalDecompositionNode<T> oldNode, 
            VerticalDecompositionNode<T> newNode
        );

        /// <summary>
        /// Links all the trapezoid nodes with their parents within this subtree
        /// </summary>
        /// <param name="parent"></param>
        public abstract void LinkNodes(VerticalDecompositionNode<T> parent);
    }
}