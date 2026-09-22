// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive depth-first search   [recursive-dfs]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursively visits each of n nodes; call stack depth equals tree
// #  height, worst case O(n) in a skewed tree.
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
 PATTERN : DFS on Binary Tree - post-order depth recursion
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  root     the node currently being measured; null means an empty subtree
WHY THIS PATTERN
  The problem asks for the longest path from the root down to a leaf. Depth of a
  node is defined in terms of its children, so the answer for root is one more
  than the larger of the two child answers. That recursive definition maps
  straight onto recursion: MaxDepth(root.left) and MaxDepth(root.right) solve
  the same problem on smaller trees, and Math.Max picks the deeper side before
  adding 1 for root itself.
BRUTE FORCE
  The simplest correct alternative is a breadth-first search: push the root into
  a queue, then pop one whole level at a time and count levels until the queue
  is empty. That is also linear in the number of nodes, so it does not lose on
  speed; it loses on code size and on needing an explicit Queue plus a per-level
  count. This file gets the same result in three lines because the recursion
  already carries the level count in the call stack.
INVARIANT
  Every call returns the exact depth of the subtree rooted at its argument: 0
  for null, and 1 + the deeper child otherwise. Since each call only uses values
  returned by strictly smaller subtrees, and the null case is correct by
  definition, induction over subtree size makes the whole answer correct. Each
  node is visited exactly once because each child pointer is followed only once.
WATCH OUT
  The recursion depth equals the height of the tree, so a long skewed chain
  (every node having only a right child) can throw a StackOverflowException,
  which .NET cannot catch. The comment "my solution" says nothing useful and
  should be dropped. There is no visited set, so a malformed tree with a cycle
  would recurse forever - fine for a real tree, but worth naming out loud. Note
  also that both children are evaluated before Math.Max runs, so there is no
  short-circuit to save work on one side.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the recursion?
     Do an iterative DFS with an explicit Stack of (node, depth) pairs, or a
     level-order BFS with a Queue, tracking the maximum depth seen. Trade-off:
     you move the frames from the call stack to the heap, so a deep tree no
     longer crashes, but you write more code and allocate a collection.
  2. How would you find the minimum depth instead?
     Return 1 for a node with no children, and when one child is null take only
     the non-null side rather than Math.Min of both - otherwise a one-sided node
     wrongly reports depth 1 through the null branch.
  3. What if you also need the diameter (longest path between any two nodes)?
     Keep the same post-order walk, but at each node compare a field like best
     against leftDepth + rightDepth while still returning 1 +
     Math.Max(leftDepth, rightDepth) upward. One pass, same linear cost, because
     the depth you already compute is exactly what the diameter needs.
  4. What if the tree is an n-ary tree?
     Loop over the children list, take the running maximum of the recursive
     calls, and add 1; an empty children list naturally gives 1.
TRIGGER
  When the answer for a node is defined purely in terms of the answers for its
  children, write the post-order recursion and combine the two returns.
C# NOTE
  Math.Max on two ints is the right call here; a hand-rolled ternary would read
  the same but Math.Max states the intent. Because root.left and root.right are
  plain fields, the null check on root at the top is the only guard needed - no
  null-conditional operator is required.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
