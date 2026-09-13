// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive post-order DFS, swap children   [recursive-dfs-swap]
// #  ties with optimal-variant-2.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  visits each node once via recursion, holding old children in locals
// #  before reassigning; call stack depth equals tree height, worst-case
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
 PATTERN : DFS recursion - swap child pointers, mutate in place
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Inverting a tree is defined recursively: the mirror of a node is that node
  with its two subtrees swapped and each subtree itself mirrored. That
  definition maps one-to-one onto a recursive call per node, so there is nothing
  to design - you write the definition down. No traversal order needs to be
  chosen and no auxiliary structure is needed, because the work at a node
  (swapping two pointers) does not depend on anything outside that node.
WHAT THE RETURN VALUE ACTUALLY IS
  InvertTree always returns the same node object it was handed - the null branch
  returns root (which is null there), and the tail returns root. It never
  allocates and never picks a different node to hand back. So left is exactly
  root.right and right is exactly root.left, already inverted underneath. The
  last three lines are therefore a plain swap of root.left and root.right; the
  locals named left and right are standing in for the temp variable of a swap.
  Reading it weeks later: this is not a rebuild, it is in-place pointer surgery
  on the tree the caller passed in. The caller's root reference stays valid,
  which is why returning root at all is a convenience, not a necessity.
THE TRAP
  The bug this shape avoids is clobbering. If you write root.left =
  InvertTree(root.right) and then root.right = InvertTree(root.left), the second
  line reads root.left AFTER it was overwritten, so you recurse into the
  already-inverted right subtree and lose the original left subtree entirely.
  Capturing both results into locals before doing any assignment to root makes
  the read-then-write ordering explicit and immune to that. Same reason a swap
  needs a temp. This is the single thing an interviewer will poke at, and it is
  also the thing that is easy to get wrong when you retype this from memory in a
  hurry.
CORRECTNESS
  Induction on height. Base case: root == null, nothing to mirror, returning
  null is correct. Inductive step: assume the two recursive calls correctly
  mirror the subtrees rooted at root.right and root.left. After they return,
  root.left points at the mirrored old right subtree and root.right points at
  the mirrored old left subtree - which is precisely the mirror of root. Every
  node is passed to exactly one call (each child pointer is followed once), so
  every node gets swapped exactly once; swapping twice would restore the
  original, so the once-only property matters.
FOLLOW-UP
  Two likely asks. (1) Do it iteratively: push root on a Stack<TreeNode> (or
  Queue for BFS), pop a node, swap its two children with a temp, push the
  non-null children. Order of visitation is irrelevant, which is why BFS and DFS
  both work here - unusual, and worth saying out loud. (2) What breaks on a
  degenerate input: a tree that is one long chain of left children forces one
  nested frame per node, so a sufficiently deep skewed tree overflows the call
  stack where the explicit-stack version would not. That is the honest argument
  for the iterative rewrite - not speed.
TRIGGER
  Reach for this shape whenever the transformation at a node is local (touches
  only that node and its immediate child pointers) and the subtree results are
  independent. Mirror, clone, sum, height, count-nodes all fit. The tell that it
  does NOT fit: the work at a node needs information from a sibling or from an
  ancestor's state, which forces a passed-down parameter or a global.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
