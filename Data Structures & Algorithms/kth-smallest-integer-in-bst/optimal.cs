// --------------------------------------------------------------------------
// -  optimal.cs            O(k) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    private int visitedCount = 0;

    public int KthSmallest(TreeNode root, int k)
    {
        if (root == null)
            return -1;

        int left = KthSmallest(root.left, k);
        if (left != -1)
            return left;

        visitedCount++;

        if (visitedCount == k)
        {
            return root.val;
        }

        return KthSmallest(root.right, k);
    }
}

/*
================================================================================
 PROBLEM : You get the root of a binary search tree and an integer k
           (1-based). Return the k-th smallest value among all node values in
           the tree. Example: tree 3 (left 1, right 4), node 1 has right child
           2; k = 2 -> 2.
 PATTERN : DFS in-order traversal with early stop
================================================================================
IDEA
  An in-order walk (left, node, right) of a BST visits values in sorted order.
  The code recurses left first, then increments visitedCount at each node.
  When visitedCount == k, it returns root.val. A non -1 result from the left
  subtree is passed straight up, so the search stops early.
  It is correct because the k-th node seen in sorted order is the k-th
  smallest.
EXAMPLE
  Tree 3 (left 1, right 4), 1 has right child 2; k = 2. Sorted order: 1,2,3,4.
  Go left from 3 to 1; 1.left null -> -1; visit 1: visitedCount=1, not k.
  Go to 1.right = 2; 2.left null -> -1; visit 2: visitedCount=2 == k, return
  2.
  2 goes up through 1 and 3 as left != -1; nodes 3 and 4 are never counted.
COMPLEXITY
  Time  O(k)  walk down the left spine, then count only k nodes before
              stopping
  Space O(n)  recursion stack as deep as the tree height, n if the tree is
              skewed
PATH TO OPTIMAL
  Collect all values, sort, take index k-1 - O(n log n) - simple, ignores BST.
  Full in-order into a list, take k-1 - O(n) - BST order removes the sort.
  In-order with counter, stop at k (this file) - O(h + k) - no list, stops
  early.
  No sibling file in this folder holds the earlier steps.
KEYWORDS
  BST, in-order traversal, kth smallest, DFS, recursion, early termination
WATCH OUT
  - -1 is used as "not found". If a node value is -1, the code thinks nothing
    was found and keeps searching, so it returns a wrong answer.
  - visitedCount is a class field that is never reset. Calling KthSmallest
    twice on the same Solution object gives wrong results.
  - If you forget "if (left != -1) return left", the answer found on the left
    is lost.
  - If k is larger than the node count, the code silently returns -1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The tree changes often and kth is asked many times. What would you do?
     -> Store the subtree size in each node, and update it on insert and
        delete. Compare k with left.size to go left, answer, or go right: O(h)
        per query.
  2. Can you do it without recursion?
     -> Use an explicit stack: push all left children, pop, count, move right.
        The time and space are the same, and there is no risk of stack overflow.
  3. Can you use O(1) extra space?
     -> Use Morris traversal. It threads right pointers back to the in-order
        successor. It takes O(n) time and changes the tree for a while.
  4. How do you find the k-th largest?
     -> Do a reverse in-order walk (right, node, left) with the same counter.
TRIGGER
  When a problem asks for a rank or sorted position inside a BST, reach for an
  in-order walk with a counter.
================================================================================
*/
