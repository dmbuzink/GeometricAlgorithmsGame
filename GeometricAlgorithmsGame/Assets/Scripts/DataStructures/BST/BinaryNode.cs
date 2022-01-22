using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    // public class BinaryNode<T>
    // {
    //     public T Value { get; set; }

    //     // TODO: To be implemented by Tar van Krieken
    // }
    public class Nullable<T>
    {
        public T val;
        public Nullable(T val)
        {
            this.val = val;
        }
    }

    public class BinaryNode<D>
    {
        public D item;
        public int height;
        public BinaryNode<D> left;
        public BinaryNode<D> right;
        protected Func<D, D, int> compare;

        /// <summary>
        /// Creates a new BSTNode
        /// </summary>
        /// <param name="item">The item to be added</param>
        /// <param name="compare">The comparison function</param>
        public BinaryNode(
            D item,
            Func<D, D, int> compare
        ) : this(item, compare, null, null)
        {
        }

        /// <summary>
        /// Creates a new BSTNode
        /// </summary>
        /// <param name="item">The item to be added</param>
        /// <param name="compare">The comparison function</param>
        /// <param name="left">The left subtree</param>
        /// <param name="right">The right subtree</param>
        public BinaryNode(
            D item,
            Func<D, D, int> compare,
            BinaryNode<D> left,
            BinaryNode<D> right
        )
        {
            this.item = item;
            this.compare = compare;
            this.height = 1;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Inserts the given data element
        /// </summary>
        /// <param name="data">The data element to be inserted</param>
        /// <returns>The new node that represents this subtree after insertion</returns>
        public BinaryNode<D> Insert(D data)
        {
            int side = this.compare(data, this.item);
            if (side < 0)
            {
                if (this.left != null) this.left = this.left.Insert(data);
                else this.left = new BinaryNode<D>(data, this.compare);
            }
            else
            {
                if (this.right != null) this.right = this.right.Insert(data);
                else this.right = new BinaryNode<D>(data, this.compare);
            }

            return this.Rebalance();
        }

        /// <summary>
        /// Deletes the given data element
        /// </summary>
        /// <param name="data">The data element to be deleted</param>
        /// <returns>The new node representing this subtree after deletion</returns>
        public BinaryNode<D> Delete(D data) => DeleteTuple(data).node;

        public (BinaryNode<D> node, bool succeeded) DeleteTuple(D data)
        {
            int side = this.compare(data, this.item);
            bool suc;
            BinaryNode<D> otherChildNode = null;

            BinaryNode<D> newNode;
            if (side < 0)
            {
                if (this.left == null) return (this, false);
                (this.left, suc) = this.left.DeleteTuple(data);
                newNode = this;

                if (!suc)
                {
                    otherChildNode = this.right;
                }
            }
            else if (side > 0)
            {
                if (this.right == null) return (this, false);
                (this.right, suc) = this.right.DeleteTuple(data);
                newNode = this;

                if (!suc)
                {
                    otherChildNode = this.left;
                }
            }
            else
            {
                suc = true;
                // This node itself should be deleted
                if (this.left == null) newNode = this.right;
                else if (this.right == null) newNode = this.left;
                else
                {
                    D next = this.right.GetMin();
                    BinaryNode<D> newRight;
                    newRight = this.right.DeleteTuple(next).node;
                    newNode = new BinaryNode<D>(next, this.compare, this.left, newRight);
                }
            }

            if (!suc && otherChildNode != null)
            {
                otherChildNode.DeleteTuple(data);
            }

            if (newNode == null) return (null, false);
            return (newNode.Rebalance(), suc);
        }


        /// <summary>
        /// Deletes the given data element
        /// </summary>
        /// <param name="comp">The comparision function to find the element to be deleted</param>
        /// <returns>The new node representing this subtree after deletion</returns>
        public BinaryNode<D> Delete(Func<D, int> comp)
        {
            int side = comp(this.item);

            BinaryNode<D> newNode;
            if (side < 0)
            {
                if (this.left == null) return this;
                this.left = this.left.Delete(comp);
                newNode = this;
            }
            else if (side > 0)
            {
                if (this.right == null) return this;
                this.right = this.right.Delete(comp);
                newNode = this;
            }
            else
            {
                // This node itself should be deleted
                if (this.left == null) newNode = this.right;
                else if (this.right == null) newNode = this.left;
                else
                {
                    D next = this.right.GetMin();
                    BinaryNode<D> newRight = this.right.Delete(next);
                    newNode = new BinaryNode<D>(next, this.compare, this.left, newRight);
                }

            }

            if (newNode == null) return null;
            return newNode.Rebalance();
        }

        /// <summary>
        /// Tries to find the specified data
        /// </summary>
        /// <param name="data">The data to be found</param>
        /// <returns>The data if it could be found</returns>
        public D Find(D data)
        {
            int side = this.compare(data, this.item);
            if (side < 0) return this.left != null ? this.left.Find(data) : default(D);
            if (side > 0) return this.right != null ? this.right.Find(data) : default(D);
            return this.item;
        }

        /// <summary>
        /// Tries to find the specified data
        /// </summary>
        /// <param name="comp">The comparison function for the data to be found</param>
        /// <returns>The data if it could be found</returns>
        public D Find(Func<D, int> comp)
        {
            int side = comp(this.item);
            if (side < 0) return this.left != null ? this.left.Find(comp) : default(D);
            if (side > 0) return this.right != null ? this.right.Find(comp) : default(D);
            return this.item;
        }


        /// <summary>
        /// Tries to find the smallest element that's larger than the specified element
        /// </summary>
        /// <param name="data">The data to be retrieved</param>
        /// <returns>The next element</returns>
        public Nullable<D> FindNext(D data)
        {
            int side = this.compare(data, this.item);
            if (side < 0)
            {
                Nullable<D> nextItem = this.left?.FindNext(data);
                if (nextItem != null) return nextItem;
                return new Nullable<D>(this.item);
            }
            return this.right?.FindNext(data);
        }

        /// <summary>
        /// Tries to find the smallest element that's larger than the specified element
        /// </summary>
        /// <param name="comp">The comparison function for the data to be retrieved</param>
        /// <returns>The next element</returns>
        public Nullable<D> FindNext(Func<D, int> comp)
        {
            int side = comp(this.item);
            if (side < 0)
            {
                Nullable<D> nextItem = this.left?.FindNext(comp);
                if (nextItem != null) return nextItem;
                return new Nullable<D>(this.item);
            }
            return this.right?.FindNext(comp);
        }

        /// <summary>
        /// Tries to find the largest element that's smaller than the specified element
        /// </summary>
        /// <param name="data">The data to be retrieved</param>
        /// <returns>The next element</returns>
        public Nullable<D> FindPrevious(D data)
        {
            int side = this.compare(data, this.item);
            if (side > 0)
            {
                Nullable<D> previousItem = this.right?.FindPrevious(data);
                if (previousItem != null) return previousItem;
                return new Nullable<D>(this.item);
            }
            return this.left?.FindPrevious(data);
        }

        /// <summary>
        /// Tries to find the largest element that's smaller than the specified element
        /// </summary>
        /// <param name="comp">The comparison function for the data to be retrieved</param>
        /// <returns>The next element</returns>
        public Nullable<D> FindPrevious(Func<D, int> comp)
        {
            int side = comp(this.item);
            if (side > 0)
            {
                Nullable<D> previousItem = this.right?.FindPrevious(comp);
                if (previousItem != null) return previousItem;
                return new Nullable<D>(this.item);
            }
            return this.left?.FindPrevious(comp);
        }

        /// <summary>
        /// Finds a range of element that's between the start and end comparison functions
        /// </summary>
        /// <param name="start">The element marking the start of the range</param>
        /// <param name="end">The element marking the end of the range</param>
        /// <param name="output">The output to accumulate the results in</param>
        public void FindRange(
            D start,
            D end,
            List<D> output
        )
        {
            int startSide = this.compare(start, this.item);
            int endSide = this.compare(end, this.item);

            if (startSide < 0) this.left?.FindRange(start, end, output);
            if (startSide <= 0 && endSide >= 0) output.Add(this.item);
            if (endSide >= 0) this.right?.FindRange(start, end, output);
        }

        /// <summary>
        /// Finds a range of element that's between the start and end comparison functions
        /// </summary>
        /// <param name="start">The comparison function for the element marking the start of the range</param>
        /// <param name="end">The comparison function for the element marking the end of the range</param>
        /// <param name="output">The output to accumulate the results in</param>
        public void FindRange(
            Func<D, int> start,
            Func<D, int> end,
            List<D> output
        )
        {
            int startSide = start(this.item);
            int endSide = end(this.item);

            if (startSide < 0) this.left?.FindRange(start, end, output);
            if (startSide <= 0 && endSide >= 0) output.Add(this.item);
            if (endSide >= 0) this.right?.FindRange(start, end, output);
        }

        /// <summary>
        /// Retrieves all the items in this subtree
        /// </summary>
        /// <param name="output">The output list to accumulate the results in</param>
        public void GetAll(List<D> output)
        {
            this.left?.GetAll(output);
            output.Add(this.item);
            this.right?.GetAll(output);
        }

        /// <summary>
        /// Retrieves the minimal element of this subtree
        /// </summary>
        /// <returns>The minimal item</returns>
        public D GetMin()
        {
            if (this.left != null) return this.left.GetMin();
            return this.item;
        }

        /// <summary>
        /// Retrieves the maximal element of this subtree
        /// </summary>
        /// <returns>The maximal item</returns>
        public D GetMax()
        {
            if (this.right != null) return this.right.GetMax();
            return this.item;
        }

        /********************
         * Internal utils   *
         ********************/


        /// <summary>
        /// Retrieves the balance of the tree.
        /// E.g: -1 means the left subtree has 1 more than the right, 2 means the right subtree has 2 more than the left
        /// </summary>
        /// <returns>The balance</returns>
        protected int GetBalance()
        {
            return (this.right?.height ?? 0) - (this.left?.height ?? 0);
        }

        /// <summary>
        /// Rotates left
        /// <code>
        ///    this             x 
        ///   /    \          /   \ 
        ///  T1     x   =>  this  T3
        ///        / \      /  \
        ///       T2 T3    T1  T2
        /// ```
        /// </code>
        /// </summary>
        /// <returns>The new Root node of the tree represented by the subtree of this node, after a left rotation</returns>
        protected BinaryNode<D> RotateLeft()
        {
            BinaryNode<D> x = this.right;
            BinaryNode<D> T2 = x.left;

            this.right = T2;
            x.left = this;

            this.height = this.Height(this.left, this.right);
            x.height = this.Height(x.left, x.right);

            return x;
        }

        /// <summary>
        /// Rotates right:
        /// <code>
        ///     this           x
        ///    /    \        /   \
        ///   x     T3  =>  T1  this
        ///  / \                /  \ 
        /// T1 T2              T2  T3 </code>
        /// </summary>
        /// <returns>The new Root node of the tree represented by the subtree of this node, after a left rotation</returns>
        protected BinaryNode<D> rotateRight()
        {
            BinaryNode<D> x = this.left!;
            BinaryNode<D> T2 = x.right;

            this.left = T2;
            x.right = this;

            this.height = this.Height(this.left, this.right);
            x.height = this.Height(x.left, x.right);

            return x;
        }

        /// <summary>
        /// Retrieves the height of a node with the given left and right subtrees
        /// </summary>
        /// <param name="left">The left subtree</param>
        /// <param name="right">The right subtree</param>
        /// <returns>The height of this node</returns>
        protected int Height(BinaryNode<D> left, BinaryNode<D> right)
        {
            return Math.Max(left?.height ?? 0, right?.height ?? 0) + 1;
        }

        /// <summary>
        /// Re-balances this subtree
        /// </summary>
        /// <returns>The node representing this subtree after rebalancing</returns>
        protected BinaryNode<D> Rebalance()
        {
            return this;
            // For more info, see: https://www.geeksforgeeks.org/avl-tree-set-2-deletion/?ref=lbp
            int balance = this.GetBalance();
            if (balance < -1)
            {
                int childBalance = this.left.GetBalance();
                if (childBalance > 0)
                {
                    /*
                     *     this                            this
                     *      / \                            /   \
                     *     y   T4  Bottom Rotate (y)        x    T4
                     *    / \      - - - - - - - - ->    /  \
                     *  T1   x                          y    T3
                     *      / \                        / \
                     *    T2   T3                    T1   T2
                     */
                    this.left = this.left.RotateLeft();
                    // Note: height gets fixed automatically when performing the next rotation
                }

                /*
                 *        this                                     y
                 *         / \                                   /   \
                 *        y   T4      Top Rotate (z)          x     this
                 *       / \          - - - - - - - - ->      /  \    /  \
                 *      x   T3                               T1  T2  T3  T4
                 *     / \
                 *   T1   T2
                 */
                return this.rotateRight();
            }
            if (balance > 1)
            {
                int childBalance = this.right!.GetBalance();
                if (childBalance < 0)
                {
                    /*
                     *   this                         this
                     *    / \                          / \
                     *  T1   y   Top Rotate (y)    T1   x
                     *      / \  - - - - - - - - ->     /  \
                     *     x   T4                      T2   y
                     *    / \                              /  \
                     *  T2   T3                           T3   T4
                     */
                    this.right = this.right!.rotateRight();
                    // Note: height gets fixed automatically when performing the next rotation
                }

                /*
                 *   this                              y
                 *   /  \                            /   \
                 *  T1   y     Bottom Rotate(z)      this    x
                 *      /  \   - - - - - - - ->    / \    / \
                 *     T2   x                     T1  T2 T3  T4
                 *         / \
                 *       T3  T4
                 */
                return this.RotateLeft();
            }

            // Update the height if no rebalance was necessary
            this.height = Height(this.left, this.right);
            return this;
        }
    }
}