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
 PATTERN : Tree DFS / In-order traversal - count nodes, stop at k
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  visitedCount  how many nodes in-order has reached so far (the rank of the current node)
  left          the result from the left subtree, or -1 if the kth node was not there
WHY THIS PATTERN
  The input is a binary search tree (BST), and we want the kth smallest value.
  An in-order walk (left, node, right) visits BST nodes in sorted order. So the
  node where visitedCount reaches k is the answer. We can stop at that point
  without visiting the rest of the tree.
BRUTE FORCE
  Walk the whole tree into a List<int>, sort it, and return list[k-1]. This is
  O(n log n) time and O(n) space, and it ignores the BST order. A better first
  try is a full in-order walk into a list with no sort, which is O(n). It still
  loses because it always visits every node, even when k is small.
INVARIANT
  When visitedCount is increased at a node, every smaller value in the tree has
  already been counted. So visitedCount is the node's exact rank in sorted
  order. The first node where visitedCount == k is therefore the kth smallest.
  Its value is passed up through the "if (left != -1) return left" checks, and
  no more nodes are counted after that.
EARLY EXIT THROUGH THE RETURN VALUE
  The value returned by each call does two jobs. It is the answer, and it also
  says "found". If the left call returns anything other than -1, the parent
  returns it at once and skips its own node and its right subtree. This is what
  makes the search stop early instead of walking the whole tree.
WATCH OUT
  The sentinel (a special value that means "not found") is -1, but -1 can also
  be a real node value. If the kth smallest value is -1, the parent reads it as
  "not found". It then keeps counting and returns a wrong, larger value. If k is
  bigger than the node count, the method returns -1 without saying the input was
  bad. On a very unbalanced tree, the recursion depth equals the tree height,
  and a deep enough tree can throw a StackOverflowException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Yes. Use an explicit Stack<TreeNode>. Push all the left children, then pop
     a node, count it, and move to its right child. When the count reaches k,
     return. The logic is the same, but a deep tree cannot overflow the call
     stack. The trade-off is more code to write.
  2. The tree changes often and kth-smallest is asked many times. What do you
  change?
     Store a subtree size in each node. At each node, compare k with leftSize +
     1, then go left, return the node, or go right with k reduced by leftSize +
     1. Each query then costs O(h), where h is the tree height. The cost is that
     every insert and delete must also update the sizes.
  3. Can you use O(1) extra space?
     Yes, with Morris traversal. It builds temporary links from each node's
     in-order predecessor back to that node, so no stack is needed. The
     trade-off is that it changes the tree for a short time and is harder to get
     right.
  4. How do you find the kth largest instead?
     Do a reverse in-order walk (right, node, left) and use the same counter.
TRIGGER
  The problem asks for the kth smallest or kth in sorted order, and the input is
  a BST.
C# NOTE
  visitedCount is an instance field, so it is never reset. Calling KthSmallest
  twice on the same Solution object gives a wrong result. A private helper with
  a "ref int count" parameter, started from a local in KthSmallest, keeps the
  count local to one call.
COMPLEXITY
  Time  : O(k)
  Space : O(n)
================================================================================
*/
