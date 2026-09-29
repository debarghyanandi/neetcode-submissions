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
 PATTERN : Tree DFS (post-order) - swap children after recursing
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left     the old right subtree, already inverted; it becomes the new root.left
  right    the old left subtree, already inverted; it becomes the new root.right
WHY THIS PATTERN
  Inverting a tree means every node swaps its two children. The same rule
  applies to each subtree, so the problem breaks into smaller copies of itself.
  Recursion fits this directly. InvertTree(root.right) gives back the finished
  mirror of the right side, and that result becomes root.left. The left side is
  handled the same way.
BRUTE FORCE
  A first attempt might copy the tree into a new mirrored tree, or collect the
  nodes level by level and rebuild the links. Both are correct, but they
  allocate extra nodes or lists and take more code. There is no simpler correct
  method that is also slower. Every node must be visited once, so this in-place
  swap is already the direct answer.
INVARIANT
  When InvertTree(x) returns, the whole subtree under x is mirrored, and x is
  still its root. The base case (root == null) is trivially mirrored. For any
  other node, both recursive calls return mirrored subtrees. Placing them on the
  opposite sides gives a mirror of the whole subtree. By induction, the call on
  the real root returns the fully inverted tree.
WATCH OUT
  The names are easy to misread: left is built from root.right, and right is
  built from root.left. That is correct, but a quick edit that "fixes" the names
  to match would break the swap. The code changes the input tree in place, so
  the caller's original tree is gone after the call. Recursion depth equals the
  tree height, so a very skewed tree (like a linked list) can overflow the call
  stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     Yes. Put root in a Queue<TreeNode> (BFS, level by level) or a
     Stack<TreeNode> (DFS). Pop a node, swap its left and right, then push the
     non-null children. This removes the stack overflow risk, but you have to
     manage the queue or stack yourself.
  2. What if the original tree must not be changed?
     Build new nodes: new TreeNode(root.val, Mirror(root.right),
     Mirror(root.left)). The input stays the same, but you allocate n new nodes.
  3. How would you check if a tree is symmetric, using the same idea?
     Compare two pointers together: a.left against b.right and a.right against
     b.left, recursively. You do not need to build an inverted copy.
TRIGGER
  The answer for a node depends only on the same answer for its two children, so
  solve both children first and then combine at the node.
C# NOTE
  Returning root in the null branch works, but "return null;" says the intent
  more clearly. If nullable reference types are enabled, the return type should
  be TreeNode? so the compiler does not warn.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
