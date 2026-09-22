// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// -  Iterative DFS with explicit stack   [iterative-dfs-stack]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Manual stack replaces recursion; visits each node once, stack size
// -  worst case O(n) on skewed tree.
// --------------------------------------------------------------------------

public class Solution
{
    // Iterative DFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null)
            return null;

        Stack<TreeNode> stack = new Stack<TreeNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null)
                stack.Push(node.left);
            if (node.right != null)
                stack.Push(node.right);
        }

        return root;
    }
}

/*
================================================================================
 PATTERN : Iterative DFS with an explicit stack - swap children per node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  stack       nodes seen but not yet swapped; drives the depth-first order
  node        the node currently being processed, popped from stack
  leftChild   saved copy of node.left before the swap overwrites it
WHY THIS PATTERN
  Mirroring a tree is a purely local edit: for every node, exchange its two
  child pointers, and the whole tree comes out mirrored. Nothing a node does
  depends on any other node, so the only real requirement is "visit every node
  exactly once" - any traversal order works, and a stack-driven DFS is the
  cheapest one to write without recursion. The loop pops node, swaps through
  leftChild, and pushes whatever children exist so they get the same treatment.
BRUTE FORCE
  The first version most people write is recursive: swap root.left and
  root.right, then call yourself on both children. It visits the same nodes and
  does the same work, so it ties on time, but the depth of the call stack is the
  height of the tree and a long skewed tree can throw StackOverflowException - a
  limit you cannot catch or raise from inside the method. This file moves that
  same stack onto the heap, where growth is bounded by memory instead of by the
  thread stack size.
INVARIANT
  Every node that is reachable from root is pushed exactly once and popped
  exactly once, and at the moment it is popped its two child pointers are
  exchanged. The push of node.left and node.right happens after the swap, but
  both non-null children are pushed, so the set of scheduled nodes is the same
  set either way - no subtree is lost. When stack.Count hits 0 there is no node
  left unswapped, and "every node's children exchanged" is the definition of the
  mirror tree.
STACK PEAK IS TIED TO HEIGHT
  Each pop removes one node and pushes at most two, so the stack grows by at
  most one entry per level of descent. The peak size is therefore proportional
  to the height of the tree, not to the node count - a balanced tree keeps only
  about log n entries alive. The worst case, a tree that is one long chain, is
  the case where height equals node count.
WATCH OUT
  This mutates the caller's tree in place; the returned reference is the same
  object as root, so any other variable pointing at that tree now sees the
  mirrored version. The null checks read node.left and node.right after the
  swap, so node.left at that point is the original right subtree - harmless here
  because both sides are pushed, but if you ever edit this loop to push only one
  side (a search, a path walk) the post-swap ordering becomes a silent bug.
  There is no visited set, so a malformed structure where two parents share a
  child, or any cycle, makes this loop run forever or swap a node twice. The
  early return for a null root returns null rather than throwing, which is the
  expected contract - do not "simplify" it away.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Could you use a Queue instead of a Stack?
     Yes, and the code is otherwise identical - the swap is order-independent.
     It becomes BFS, and peak memory becomes the widest level of the tree
     instead of the height, which is worse for a balanced tree and better for a
     deep skinny one.
  2. The caller needs the original tree kept intact. What changes?
     Build a new tree instead of editing: create a new node per original node
     with the children crossed over. Time is the same, but you allocate one new
     node per input node and the traversal has to carry parent-plus-side
     information (or go recursive) so each new node can be attached.
  3. Same problem on a tree where a node holds a list of children, not exactly
  two?
     Replace the two-pointer swap with reversing the child list, then push every
     child. The stack loop and the visit-once argument are unchanged.
  4. How would you instead check whether a tree is already a mirror of itself?
     Push pairs of nodes rather than single nodes: start with (root.left,
     root.right), and for each pair compare values and push (a.left, b.right)
     and (a.right, b.left). You stop early and return false on the first
     mismatch, so you may touch far fewer nodes than a full inversion.
TRIGGER
  The task is a local rewrite that must be applied once to every node, with no
  ordering or accumulation between nodes - reach for a plain stack traversal and
  do the edit at pop time.
C# NOTE
  leftChild can be dropped entirely with the tuple assignment (node.left,
  node.right) = (node.right, node.left), which the language evaluates right-hand
  side first and so needs no temp. Stack<TreeNode> here starts with a
  default-size array and doubles as it grows; since you do not know the height
  up front there is no sensible capacity to pass to the constructor.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
