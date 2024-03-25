using System;
using DefaultNamespace;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;

namespace Tests
{
    public class ExposedBinarySearchTree<D> : BinarySearchTree<D>
    {
        public BinaryNode<D> root
        {
            get
            {
                return base.root;
            }
        }

        public ExposedBinarySearchTree(Func<D, D, int> compare) : base(compare)
        {
        }
    }

    public class BinarySearchTreeTest
    {

        public BinarySearchTree<D> createTree<D>(Func<D, D, int> compare, D[] vals)
        {
            BinarySearchTree<D> tree = new BinarySearchTree<D>(compare);
            foreach (D val in vals) tree.Insert(val);
            return tree;
        }

        [Test]
        public void Insert_HandlesArbitraryInserts()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {3, 6, 2, 7, 13, 45, 23, 57, 33, 19, 8, 1, 34, 12, 28});

            Assert.AreEqual(57, tree.Find(57));
            Assert.AreEqual(0, tree.Find(14));
            Assert.AreEqual(57, tree.Find(p => 57 - p));
        }

        [Test]
        public void Insert_RemainsBalanced()
        {
            int count = 1000;
            ExposedBinarySearchTree<int> tree = new ExposedBinarySearchTree<int>((a, b) => a - b);
            for (int i = 0; i < count; i++) tree.Insert(i);

            Assert.GreaterOrEqual(Math.Log(count, 2) * 2, tree.root.height);


            tree = new ExposedBinarySearchTree<int>((a, b) => a - b);
            for (int i = count - 1; i >= 0; i--) tree.Insert(i);

            Assert.GreaterOrEqual(Math.Log(count, 2) * 2, tree.root.height);
        }

        [Test]
        public void Insert_HandlesDuplices()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {10, 11, 12, 13, 14, 15, 10, 11, 12, 13});

            Assert.AreEqual(10, tree.Find(10));
            Assert.AreEqual(0, tree.Find(20));
            Assert.AreEqual(12, tree.Find(p => 12 - p));
        }

        [Test]
        public void Delete_HandlesArbitraryDeletes()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {3, 6, 2, 7, 13, 45, 23, 57, 33, 19, 8, 1, 34, 12, 28});

            int[] remove = new int[] {13, 45, 12, 28, 33, 19};
            foreach (int rem in remove) tree.Delete(rem);

            Assert.AreEqual(57, tree.Find(57));
            Assert.AreEqual(0, tree.Find(45));
            Assert.AreEqual(0, tree.Find(p => 28 - p));
        }


        [Test]
        public void Delete_RemainsBalanced()
        {
            int count = 1000;
            ExposedBinarySearchTree<int> tree = new ExposedBinarySearchTree<int>((a, b) => a - b);
            for (int i = 0; i < count; i++) tree.Insert(i);
            for (int i = 0; i < count; i += 2) tree.Delete(i);

            Assert.GreaterOrEqual(Math.Log(count / 2, 2) * 2, tree.root.height);
            Assert.LessOrEqual(Math.Log(count / 2, 2), tree.root.height);

            tree = new ExposedBinarySearchTree<int>((a, b) => a - b);
            for (int i = 0; i < count; i++) tree.Insert(i);
            for (int i = 0; i < count / 2; i++) tree.Delete(i);

            Assert.GreaterOrEqual(Math.Log(count / 2, 2) * 2, tree.root.height);
            Assert.LessOrEqual(Math.Log(count / 2, 2), tree.root.height);
        }

        [Test]
        public void Delete_HandlesDuplicates()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[]
                {
                    3, 6, 2, 7, 13, 45, 23, 57, 33, 19, 8, 1, 34, 12, 28,
                    /** dups */ 7, 23, 23,
                });

            tree.Delete(7);
            Assert.AreEqual(7, tree.Find(7));

            tree.Delete(23);
            tree.Delete(7);
            Assert.AreEqual(0, tree.Find(7));

            Assert.AreEqual(23, tree.Find(23));
            tree.Delete(23);
            Assert.AreEqual(23, tree.Find(23));
            tree.Delete(23);
            Assert.AreEqual(0, tree.Find(23));
        }


        [Test]
        public void Delete_HandlesEmptying()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] { 1, 3, 2 });

            int[] remove = new int[] { 1, 2, 3 };
            foreach (int rem in remove) tree.Delete(rem);

            Assert.AreEqual(0, tree.Find(1));
            Assert.AreEqual(0, tree.Find(2));
            Assert.AreEqual(0, tree.Find(3));
        }

        [Test]
        public void FindNext_ObtainsNextItemIfThere()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39});

            Assert.AreEqual(8, tree.FindNext(4));
            Assert.AreEqual(14, tree.FindNext(12));
            Assert.AreEqual(1, tree.FindNext(-20));
            Assert.AreEqual(1, tree.FindNext(p => -1));
        }

        [Test]
        public void FindNext_ObtainsNothingIfItemAbsent()
        {

            BinarySearchTree<int> tree = new BinarySearchTree<int>((a, b) => a - b);
            Assert.AreEqual(0, tree.FindNext(24));

            int[] vals = new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39};
            foreach (int val in vals) tree.Insert(val);

            Assert.AreEqual(0, tree.FindNext(50));
            Assert.AreEqual(0, tree.FindNext(p => 1));
        }

        [Test]
        public void FindPrevious_ObtainsPreviousItemIfThere()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39});

            Assert.AreEqual(3, tree.FindPrevious(4));
            Assert.AreEqual(10, tree.FindPrevious(12));
            Assert.AreEqual(39, tree.FindPrevious(100));
            Assert.AreEqual(39, tree.FindPrevious(p => 1));
        }

        [Test]
        public void FindPrevious_ObtainsNothingIfItemAbsent()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>((a, b) => a - b);
            Assert.AreEqual(0, tree.FindPrevious(24));

            int[] vals = new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39};
            foreach (int val in vals) tree.Insert(val);

            Assert.AreEqual(0, tree.FindPrevious(0));
            Assert.AreEqual(0, tree.FindPrevious(p => -1));
        }

        [Test]
        public void FindRange_FindsSpecifiedRange()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39});

            CollectionAssert.AreEqual(new List<int>(new int[] {4, 8, 10, 14}), tree.FindRange(4, 14));
            CollectionAssert.AreEqual(new List<int>(new int[] {10, 14, 20, 25}), tree.FindRange(9, 26));
            CollectionAssert.AreEqual(new List<int>(new int[] {20}), tree.FindRange(p => 15 - p, p => 22 - p));

            CollectionAssert.AreEqual(new List<int>(new int[] {1, 2, 3, 4, 8, 10, 14, 20, 25, 39}),
                tree.FindRange(-1, 90));
        }

        [Test]
        public void FindRange_ReportsDuplicatesInRange()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] {1, 2, 3, 4, 4, 8, 10, 10, 10, 14, 14, 20, 25, 39});

            CollectionAssert.AreEqual(new List<int>(new int[] {4, 4}), tree.FindRange(4, 4));
            CollectionAssert.AreEqual(new List<int>(new int[] {8, 10, 10, 10, 14, 14}), tree.FindRange(7, 15));
        }

        [Test]
        public void FindRange_ReportsEmptyListsForEmptyRanges()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>((a, b) => a - b);
            CollectionAssert.AreEqual(new List<int>(new int[] { }), tree.FindRange(21, 24));

            int[] vals = new int[] { 1, 2, 3, 4, 8, 10, 14, 20, 25, 39 };
            foreach (int val in vals) tree.Insert(val);

            CollectionAssert.AreEqual(new List<int>(new int[] { }), tree.FindRange(21, 24));
            CollectionAssert.AreEqual(new List<int>(new int[] { }), tree.FindRange(40, 90));
            CollectionAssert.AreEqual(new List<int>(new int[] { }), tree.FindRange(-20, 0));
        }

        [Test]
        public void GetMin_RetrievesMinimalElement()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] { 1, 2, 3, 4, 4, 8, 10, 10, 10, 14, 14, 20, 25, 39 });

            Assert.AreEqual(1, tree.GetMin());
            tree.Insert(11);
            Assert.AreEqual(1, tree.GetMin());
            tree.Insert(-1);
            Assert.AreEqual(-1, tree.GetMin());
        }

        [Test]
        public void GetMin_RetrievesNothingIfEmpty()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>((a, b) => a - b);
            Assert.AreEqual(0, tree.GetMin());
        }

        [Test]
        public void GetMax_RetrievesMinimalElement()
        {
            BinarySearchTree<int> tree = this.createTree(
                (a, b) => a - b,
                new int[] { 1, 2, 3, 4, 4, 8, 10, 10, 10, 14, 14, 20, 25, 39 });

            Assert.AreEqual(39, tree.GetMax());
            tree.Insert(11);
            Assert.AreEqual(39, tree.GetMax());
            tree.Insert(41);
            Assert.AreEqual(41, tree.GetMax());
        }

        [Test]
        public void GetMax_RetrievesNothingIfEmpty()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>((a, b) => a - b);
            Assert.AreEqual(0, tree.GetMax());
        }
    }
}
