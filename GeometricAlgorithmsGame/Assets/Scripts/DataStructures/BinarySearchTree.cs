using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class BinarySearchTree<D>
    {
        protected BinaryNode<D> root;
        private readonly Func<D, D, int> compare;

        /// <summary>
        /// Creates a new self balancing binary search using the given comparison function to sort items
        /// </summary>
        /// <param name="compare">The data comparison function, result < 0 iff a < b, result > 0 iff a > b, result = 0 otherwise</param>
        public BinarySearchTree(Func<D, D, int> compare)
        {
            this.compare = compare;
        }

        /// <summary>
        /// Inserts the given data element
        /// </summary>
        /// <param name="data">The data element to be inserted</param>
        public void Insert(D data)
        {
            if (this.root == null) this.root = new BinaryNode<D>(data, this.compare);
            else this.root = this.root.Insert(data);
        }

        /// <summary>
        /// Deletes the given data element
        /// </summary>
        /// <param name="data">The data element to be deleted</param>
        public void Delete(D data)
        {
            if (this.root == null) return;
            this.root = this.root.Delete(data);
        }

        /// <summary>
        /// Deletes the given data element
        /// </summary>
        /// <param name="compare">The data comparison function, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        public void Delete(Func<D, int> compare)
        {
            if (this.root == null) return;
            this.root = this.root.Delete(compare);
        }

        /// <summary>
        /// Checks whether the given data element is in the tree
        /// </summary>
        /// <param name="data">The data element to find</param>
        /// <returns>The data element that was found, if any</returns>
        public D Find(D data)
        {
            return this.root != null ? this.root.Find(data) : default(D);
        }

        /// <summary>
        /// Tries to find a data element according to the given comparison function
        /// </summary>
        /// <param name="compare">The data comparison function, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        /// <returns>The data element that was found, if any</returns>
        public D Find(Func<D, int> compare)
        {
            return this.root != null ? this.root.Find(compare) : default(D);
        }

        /// <summary>
        /// Finds the smallest element that's larger than the given element
        /// </summary>
        /// <param name="data">The data element to find</param>
        /// <returns>The data element that was found, if any</returns>
        public D FindNext(D data)
        {
            if (this.root != null)
            {
                Nullable<D> item = this.root.FindNext(data);
                if (item != null) return item.val;
            }

            return default(D);
        }

        /// <summary>
        /// Finds the smallest element that's bigger than the one specified by the given comparison function
        /// </summary>
        /// <param name="compare">The data comparison function, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        /// <returns>The data element that was found, if any</returns>
        public D FindNext(Func<D, int> compare)
        {
            if (this.root != null)
            {
                Nullable<D> item = this.root.FindNext(compare);
                if (item != null) return item.val;
            }

            return default(D);
        }

        /// <summary>
        /// Finds the largest element that's smaller than the given element
        /// </summary>
        /// <param name="data">The data element to find</param>
        /// <returns>The data element that was found, if any</returns>
        public D FindPrevious(D data)
        {

            if (this.root != null)
            {
                Nullable<D> item = this.root.FindPrevious(data);
                if (item != null) return item.val;
            }

            return default(D);
        }

        /// <summary>
        /// Finds the largest element that's smaller than the one specified by the given comparison function
        /// </summary>
        /// <param name="compare">The data comparison function, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        /// <returns>The data element that was found, if any</returns>
        public D FindPrevious(Func<D, int> compare)
        {
            if (this.root != null)
            {
                Nullable<D> item = this.root.FindPrevious(compare);
                if (item != null) return item.val;
            }

            return default(D);
        }

        /// <summary>
        /// Finds a range of data elements that's between the start and end
        /// </summary>
        /// <param name="start">The element marking the start of the range</param>
        /// <param name="end">The element marking the end of the range</param>
        /// <returns>The data elements that were found</returns>
        public List<D> FindRange(D start, D end)
        {
            List<D> output = new List<D>();
            this.root?.FindRange(start, end, output);
            return output;
        }

        /// <summary>
        /// Finds a range of element that's between the start and end comparison functions
        /// </summary>
        /// <param name="start">The data comparison function for the start of the range, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        /// <param name="end">The data comparison function for the end of the range, result < 0 iff query < item, result > 0 iff query > item, result = 0 otherwise</param>
        /// <returns> The data elements that were found</returns>
        public List<D> FindRange(Func<D, int> start, Func<D, int> end)
        {
            List<D> output = new List<D>();
            this.root?.FindRange(start, end, output);
            return output;
        }

        /// <summary>
        /// Retrieves the smallest item in the tree
        /// </summary>
        /// <returns>The minimal element, if the tree isn't empty</returns>
        public D GetMin()
        {
            return this.root != null ? this.root.GetMin() : default(D);
        }

        /// <summary>
        /// Retrieves the largest item in the tree
        /// </summary>
        /// <returns>The maximal element, if the tree isn't empty</returns>
        public D GetMax()
        {
            return this.root != null ? this.root.GetMax() : default(D);
        }

        /// <summary>
        /// Retrieves all the items in the tree
        /// </summary>
        /// <returns>All the stored items</returns>
        public List<D> GetAll()
        {
            List<D> output = new List<D>();
            this.root?.GetAll(output);
            return output;
        }
    }
}