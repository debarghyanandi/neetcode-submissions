// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive DFS tree inversion   [recursive-dfs]
// #  ties with optimal-variant-2.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursively visits each node once; call stack depth is O(h), worst
// #  case O(n) on skewed tree.
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
 PATTERN : Tree DFS - post-order swap of children
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left     the already-inverted subtree that came from root.right
  right    the already-inverted subtree that came from root.left
WHY THIS PATTERN
  Inverting a tree means every node swaps its two children, and that rule is the
  same at every level. A recursive definition like that maps straight onto
  depth-first recursion: solve root.right and root.left first, then fix up root.
  The two temporaries left and right hold the finished subtrees so the
  assignment to root.left and root.right cannot destroy data that is still
  needed.
BRUTE FORCE
  There is no cheaper correct shape here - every node must be visited once, so
  O(n) work is the floor. The naive alternative a person might write is to copy
  the tree into a new structure while mirroring it, which is also O(n) time but
  allocates n fresh TreeNode objects instead of rewiring the existing ones. This
  file mutates in place and allocates nothing.
INVARIANT
  When InvertTree(node) returns, the entire subtree rooted at node is fully
  mirrored and node itself is returned unchanged as the subtree root. The two
  recursive calls happen before any write, so at the moment root.left = left
  runs, both children are already complete mirrors. By induction from the null
  base case upward, the whole tree is mirrored when the top call returns.
WATCH OUT
  The variable names are crossed on purpose: left is built from root.right. If
  you ever "fix" that to make the names read naturally, the tree stops
  inverting. The recursion depth equals the height of the tree, so a long chain
  of nodes - a tree that is effectively a linked list - can overflow the call
  stack; there is no tail call to save you because work happens after both
  calls. The null branch returns root, which is just null, so the caller gets
  null and the assignment is still correct.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Yes - push root on a Stack<TreeNode> or Queue<TreeNode>, pop a node, swap
     its two children, push the non-null children. Same O(n) time, and the
     explicit structure moves the memory off the call stack, so a very deep tree
     no longer risks a stack overflow.
  2. What if you are not allowed to modify the input tree?
     Build a new node per visit: return new TreeNode(root.val,
     InvertTree(root.right), InvertTree(root.left)). Same traversal, but now you
     pay n allocations instead of rewiring in place.
  3. How does this change for "check if a tree is symmetric"?
     Do not mutate anything - recurse on a pair (a.left, b.right) and (a.right,
     b.left) and compare values as you go. Same mirrored pairing, but the result
     is a bool and the tree is left untouched.
  4. The tree is huge and wide - does the recursion still hold up?
     Width is not the problem; only height drives the call stack, so a wide
     balanced tree is safe at about log n frames. A deep unbalanced tree is the
     case that forces the iterative version.
TRIGGER
  A tree operation whose rule is stated for one node and applies identically to
  every node - reach for recursion that finishes both children before touching
  the parent.
C# NOTE
  C# tuple assignment would collapse the two temporaries into one line:
  (root.left, root.right) = (InvertTree(root.right), InvertTree(root.left)) -
  the right side is fully evaluated before either write, which is exactly the
  ordering left and right are protecting here.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
