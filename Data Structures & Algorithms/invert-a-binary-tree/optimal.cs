// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive DFS, swap children   [recursive-dfs-swap]
// #  ranks above optimal-variant.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  visits each node once; call stack depth equals tree height, worst-case
// #  O(n) for a skewed tree
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
 PATTERN : Post-order recursion - mirror subtrees, then swap
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A mirrored tree is defined recursively: mirror(node) is a node whose left
  child is mirror(node.right) and whose right child is mirror(node.left). The
  code is that definition typed out literally. Every node's work is purely local
  - fix my two child pointers - and the shape of the subtrees is somebody else's
  problem, delegated to the two recursive calls. Nothing about a node depends on
  its parent or on any sibling, so there is no state to thread down and no
  accumulator to thread back up.
INVARIANT
  InvertTree(x) returns the root of a fully mirrored version of the subtree at
  x, and it returns the SAME node object it was handed - the tree is rewritten
  in place, not rebuilt. Two consequences worth being able to state out loud:
  1. The return value is redundant for a caller who already holds x; it exists
  so the recursive assignments read cleanly, and so the null base case has
  something to hand back.
  2. Because it mutates, the original tree is destroyed. Calling this on a tree
  someone else still holds a reference to changes what they see.
THE TRAP - WHY LEFT AND RIGHT ARE LOCALS
  The temporaries are the whole correctness story. The tempting compression is:

      root.left = InvertTree(root.right);
      root.right = InvertTree(root.left);

  That is wrong. Line one overwrites root.left, so line two reads the value just
  written and recurses into the already-inverted right subtree - inverting it a
  second time, back to its original shape. The original left subtree is never
  visited and is dropped on the floor, and root.left and root.right end up
  aliasing the same node. The result is a tree with a duplicated branch, not a
  mirror. Holding both results in local left and local right first means both
  recursive calls read the child pointers as they were before any write
  happened; the two assignments then land as an atomic swap.
CORRECTNESS ARGUMENT
  Induction on subtree height. Height 0 (root == null) is vacuously mirrored,
  and the base case returns it untouched. For a node of height h, both recursive
  calls run on subtrees of height at most h-1, so by hypothesis local left holds
  the mirror of the original right subtree and local right holds the mirror of
  the original left subtree. Assigning them to root.left and root.right
  respectively satisfies the mirror definition at root. Every node is reached
  exactly once, through its parent's pair of calls, so the whole tree is
  mirrored.
WATCH OUT
  The recursion depth equals the height of the tree, so a degenerate tree - a
  linked list of a few tens of thousands of nodes - blows the call stack before
  anything else goes wrong. That is the one input class where this solution
  fails on a machine but not on paper. Also note the base case returns root
  rather than the literal null; identical behavior, since root is null there,
  but if asked to explain the line, say that rather than pretending it means
  something more.
INTERVIEWER FOLLOW-UP
  Two likely asks.
  Iterative version: push root onto a Stack or Queue of TreeNode; while
  non-empty, pop a node, swap its two child pointers directly using one temp,
  then enqueue the non-null children. BFS and DFS both work - the swap at a node
  is independent of every other node - and this also answers the stack-depth
  objection above.
  Non-destructive version: instead of writing back into root, build and return a
  new TreeNode whose left is InvertTree(root.right) and whose right is
  InvertTree(root.left). Same recursion, but the input survives.
TRIGGER
  Reach for this shape whenever a tree transformation is defined in terms of the
  same transformation on the children, and each node's result depends only on
  its own value plus its children's results. The tell that you specifically need
  the temporaries: the node has two or more mutable fields whose new values are
  each computed from the OLD value of a different field. Any time reads and
  writes interleave over the same slots, capture all the reads first, then
  write.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
