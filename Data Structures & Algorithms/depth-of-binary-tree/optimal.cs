// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    // my solution
    public int MaxDepth(TreeNode root)
    {
        if (root == null)
            return 0;
        return Math.Max(MaxDepth(root.left), MaxDepth(root.right)) + 1;
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, return its maximum depth. Depth is
           the number of nodes on the longest path from the root down to a
           leaf. An empty tree has depth 0. Example: [3,9,20,null,null,15,7]
           -> 3.
 PATTERN : DFS (recursive, post-order) on a tree
================================================================================
IDEA
  MaxDepth(root) asks each child for its own depth, then adds 1 for root.
  The base case is root == null, which returns 0, so a leaf gets
  Math.Max(0, 0) + 1 = 1. It is correct because the longest path through
  root must continue into the deeper of root.left and root.right.
EXAMPLE
  Tree [3,9,20,null,null,15,7]: 9, 15 and 7 are leaves, so each returns 1.
  Node 20 = Max(1, 1) + 1 = 2. Node 9 = 1.
  Root 3 = Max(1, 2) + 1 = 3. Answer: 3.
COMPLEXITY
  Time  O(n)  each node is visited exactly once, with O(1) work per call
  Space O(n)  call stack holds one frame per level, and height is n if skewed
PATH TO OPTIMAL
  List all root-to-leaf paths, take the longest - O(n*h) - wasteful copies.
  Recursive DFS returning depth - O(n) - no path lists, each node once.
  Iterative BFS/stack version - O(n) - no recursion limit
  (optimal-variant.cs).
KEYWORDS
  binary tree, max depth, height, DFS, recursion, post-order, BFS level order
WATCH OUT
  - Returning 1 for null gives depth + 1. Returning 0 for a leaf measures
    edges, not nodes. Check which one the problem wants.
  - A very deep skewed tree (a linked list) can cause a stack overflow here.
    In C#, this crashes the process and cannot be caught.
  - Do not copy this into Min Depth. There, a node with one null child
    must use the other child. Math.Min would wrongly return 1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid recursion?
     -> Use BFS with a queue and count levels, or push (node, depth) pairs on
        a stack. Still O(n) time. Space is O(width) or O(h) on the heap.
  2. Find the minimum depth instead.
     -> Use BFS and stop at the first leaf you reach. This is fast when a leaf
        is shallow. In DFS, ignore a null child when the other is not null.
  3. Check if the tree is height-balanced.
     -> Use the same DFS, but return -1 when the children's heights differ by
        more than 1. Pass the -1 up. One pass, O(n) time.
  4. Find the diameter (longest path between any two nodes).
     -> In the same DFS, update a global best with left + right at each node.
        Return max + 1 as before. Still O(n) time.
TRIGGER
  The answer for a node depends only on the same answer for its children.
================================================================================
*/
