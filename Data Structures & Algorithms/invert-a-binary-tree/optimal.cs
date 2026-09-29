// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public TreeNode InvertTree(TreeNode root)
    {
        //My solution
        if (root == null)
        {
            return root;
        }
        TreeNode left = InvertTree(root.right);
        TreeNode right = InvertTree(root.left);
        root.left = left;
        root.right = right;

        return root;
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, mirror it: at every node, swap the
           left and right child. Return the root of the inverted tree. An
           empty tree returns null. Example: [4,2,7,1,3,6,9] ->
           [4,7,2,9,6,3,1].
 PATTERN : Tree DFS (recursive, post-order swap)
================================================================================
IDEA
  If root is null, return it. Otherwise first invert both subtrees.
  The inverted right subtree goes into the variable left, and the inverted
  left subtree goes into the variable right. Then set root.left = left and
  root.right = right, and return root.
  It is correct by induction: each subtree comes back fully mirrored, so
  putting them on opposite sides mirrors the whole tree.
EXAMPLE
  Input [4,2,7,1,3,6,9]. InvertTree(7): leaves 6 and 9 swap -> 7(9,6).
  InvertTree(2): leaves 1 and 3 swap -> 2(3,1).
  At 4: left = 7(9,6), right = 2(3,1), then assign both.
  Answer: [4,7,2,9,6,3,1].
COMPLEXITY
  Time  O(n)  each node is visited once and does O(1) work
  Space O(n)  recursion stack is as deep as the tree, n for a skewed tree
PATH TO OPTIMAL
  Build a new mirrored copy of the tree - O(n) time, O(n) extra nodes.
  In-place recursive swap (this file) - O(n) / O(h) - no copy is needed.
  Iterative BFS or DFS with a queue/stack - same O(n), no call-stack risk
  (the other O(n) versions are in optimal-variant.cs / -2.cs).
KEYWORDS
  binary tree, mirror tree, invert, DFS, recursion, post-order, BFS
WATCH OUT
  - Bug: root.left = InvertTree(root.right); root.right =
    InvertTree(root.left);
    The second call then sees the new left. Save both results first, as here.
  - A very deep skewed tree (a long chain) can overflow the C# call stack.
  - The tree is changed in place. The caller's original tree is gone.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     -> Use a queue (BFS). Pop a node, swap its children, push the non-null
        children. O(n) time, O(w) space, where w is the max level width.
  2. Keep the original tree unchanged?
     -> Build new nodes: new TreeNode(val, copy(right), copy(left)). O(n)
        time, O(n) extra memory for the copy.
  3. Check whether two trees are mirrors of each other (Symmetric Tree)?
     -> Recurse on pairs: the values match, a.left mirrors b.right, and
        a.right mirrors b.left. O(n) time, O(h) stack.
TRIGGER
  The task changes or compares every node of a tree using only its own
  children, so one DFS that handles each subtree solves it.
================================================================================
*/
