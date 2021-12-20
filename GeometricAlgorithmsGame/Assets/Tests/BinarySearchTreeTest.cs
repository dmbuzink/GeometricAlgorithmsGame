using System;
using DefaultNamespace;
using NUnit.Framework;

namespace Tests
{
    public class BinarySearchTreeTest
    {
        public BinarySearchTree<D> createTree<D>(Func<D, D, int> compare, D[] vals)
        {
            BinarySearchTree<D> tree = new BinarySearchTree<D>(compare);
            foreach (D val in vals)
            {
                tree.Insert(val);
            }

            return tree;
        }

        [Test]
        public void handlesInserts()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] { 3, 6, 2, 7, 13, 45, 23, 57, 33, 19, 8, 1, 34, 12, 28 });

            Assert.AreEqual(expected: 57, actual: tree.Find(57));
            Assert.AreEqual(expected: 0, actual: tree.Find(14));
            Assert.AreEqual(expected: 57, actual: tree.Find(p => 57 - p));
        }
    }
}
