// --------------------------------------------------------------------------
// -  optimal.cs            O(k) time / O(n) space
// -  In-order traversal with early termination   [bst-inorder-early-return]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Early termination after visiting k nodes in sorted order; space is
// -  recursion depth, O(n) worst case for unbalanced trees
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
 PATTERN : Inorder DFS on a BST - stop at the kth visited node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  visitedCount  how many nodes the inorder walk has already passed; shared instance field
  left          result bubbled up from the left subtree, or -1 meaning "not found there yet"
WHY THIS PATTERN
  The tree is a binary search tree, so an inorder walk (left, node, right)
  visits values in sorted order. That turns "kth smallest" into "the kth node
  the walk touches", so no sorting is needed. The code counts each node as it is
  visited with visitedCount++ and returns root.val the moment visitedCount
  equals k. Everything to the right of that node is never entered, which is why
  the walk stops early.
BRUTE FORCE
  The first thing most people write is: walk the whole tree, push every value
  into a List, sort it, and return list[k-1]. That is O(n) space and O(n log n)
  time, and it ignores the BST property completely. Even collecting the full
  inorder list without sorting still touches all n nodes when only the first k
  matter.
INVARIANT
  At the moment any node executes visitedCount++, every value smaller than
  root.val has already been visited, and visitedCount is exactly the rank of
  root.val among all values seen so far, counting from 1. Because the walk is
  strictly ordered, the first time visitedCount == k the current node holds the
  kth smallest value. The left != -1 check makes that value travel back up the
  call chain untouched instead of being overwritten by the right-subtree call.
THE SENTINEL DOES TWO JOBS
  The value -1 is used both for "this subtree is empty" and for "the answer is
  not in this subtree". That is why one return statement covers root == null and
  why the parent can test left != -1 as "we are done". It works only if -1 can
  never be a real node value.
WATCH OUT
  If any node in the tree holds the value -1 and it is the answer, the parent
  reads the returned -1 as "not found", keeps walking, and returns a wrong value
  or -1. visitedCount is an instance field, not a parameter: calling KthSmallest
  twice on the same Solution object starts the second call with a non-zero count
  and gives a wrong answer. If k is larger than the number of nodes the method
  returns -1 instead of signalling an error, which the caller cannot distinguish
  from a real -1 value. Recursion depth follows the tree height, so a long
  degenerate chain can overflow the call stack before the count ever reaches k.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The tree is modified often and kth smallest is asked many times. How does
  the design change?
     Store a subtree node count in every node. Then each query descends one
     path: compare k with leftCount + 1 and go left or right, giving O(h) per
     query instead of O(k), at the cost of updating counts on every insert and
     delete.
  2. What if you need the kth largest instead?
     Mirror the traversal - recurse right first, then count the node, then
     recurse left. The counting logic and stopping rule stay exactly the same.
  3. What if the tree is an ordinary binary tree, not a BST?
     Inorder order is no longer sorted, so the early stop is gone. Walk every
     node and keep a max-heap of size k, or a min-heap of all values, giving O(n
     log k) time.
TRIGGER
  A question asks for an element by rank or order position inside a BST - reach
  for inorder traversal with a counter and an early return.
C# NOTE
  An IEnumerable<int> inorder generator using yield return would give the same
  early stop through foreach with Take(k), keep the counter local to each call,
  and remove the -1 sentinel entirely since the caller decides what an empty
  sequence means.
COMPLEXITY
  Time  : O(k)
  Space : O(n)
================================================================================
*/
