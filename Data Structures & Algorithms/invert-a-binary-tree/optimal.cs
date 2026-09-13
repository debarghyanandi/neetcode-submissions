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
 PATTERN : Post-order DFS - reattach swapped children on unwind
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The mirror of a tree is defined recursively: the left subtree of mirror(T) is
  the mirror of T.right, and its right subtree is the mirror of T.left.
  InvertTree is a literal transcription of that definition -
  InvertTree(root.right) becomes the new left, InvertTree(root.left) becomes the
  new right.

  No node value is ever read and no comparison is made, so nothing forces a
  particular visit order. Every node just needs its two child pointers exchanged
  exactly once, which is why a plain traversal suffices and no BST ordering
  logic appears here.
INVARIANT
  InvertTree(x) returns x itself, with everything under x already fully
  mirrored.

  That holds by induction: null is trivially mirrored, and for a non-null root
  both recursive calls have returned before the first assignment executes, so
  left already points at a finished mirrored subtree and right already points at
  a finished mirrored subtree when root.left = left and root.right = right run.
  The node is fixed up on the way back up the stack, never on the way down - the
  subtrees below it are never touched again after its own assignments.
THE TRAP
  The two locals left and right are load-bearing, not style. The tempting
  compression

    root.left = InvertTree(root.right);
    root.right = InvertTree(root.left);

  is wrong. Line 1 overwrites root.left, so line 2 reads the value just stored -
  the already-inverted right subtree - and re-inverts it back into place while
  the original left subtree is dropped entirely. On a tree like 4(2,7) you would
  get 7 under both slots and 2 gone.

  By capturing both results in locals first and assigning afterwards, the read
  of root.right and the read of root.left both happen against the untouched
  node. This is the standard aliasing hazard: two reads and two writes to the
  same object, so the reads must be sequenced before the writes.
BASE CASE AND TERMINATION
  if (root == null) return root; returns null - the same value the caller stores
  into a leaf's child slot, so leaves need no special case. A leaf makes two
  calls that both return null, then assigns null over null: correct and
  harmless.

  Each call recurses on strictly smaller subtrees and a finite tree has finite
  depth, so recursion bottoms out. Nothing is allocated - the return value is
  the same root reference that came in, so the caller's handle to the tree stays
  valid and the inversion is in place.
FOLLOW-UPS TO EXPECT
  1. Do it iteratively. BFS with a Queue<TreeNode>: dequeue a node, swap its two
  children through a temp, enqueue each non-null child. A Stack<TreeNode> works
  identically - order does not matter, per WHY THIS PATTERN.

  2. Why does the recursive version risk a stack overflow? A degenerate tree
  (every node has only a right child) drives the call chain as deep as the tree
  is tall, and .NET will not tail-call-optimize this anyway since the recursive
  calls are not in tail position - work follows them. The iterative version
  moves that frontier to the heap.

  3. Could this return void? Yes - swap root.left and root.right in place, then
  recurse on both. The return value here exists only so the parent frame has
  something to assign.
TRIGGER
  Reach for this shape when the transformation is defined structurally on each
  node in terms of its children and needs no information from outside the
  subtree: mirror a tree, check symmetry, compute a depth or sum, build a
  mirrored copy. The tell is that a node's answer is a pure function of its
  children's answers, which forces the post-order shape - do all the recursion
  first, then commit the writes.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
