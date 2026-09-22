// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  BST traversal, recursive descent   [bst-recursive]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursively traverses BST using the same property; call stack depth
// #  equals tree height, O(n) worst case for skewed tree.
// ##########################################################################

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        //My solution
        int min = Math.Min(p.val, q.val);
        int max = Math.Max(p.val, q.val);

        if (max >= root.val && min <= root.val)
            return root;

        if (min > root.val)
        {
            return LowestCommonAncestor(root.right, p, q);
        }
        else
            return LowestCommonAncestor(root.left, p, q);
    }
}

/*
================================================================================
 PATTERN : BST Descent - walk down while both targets stay on one side
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  min      the smaller of p.val and q.val
  max      the larger of p.val and q.val
WHY THIS PATTERN
  The tree is a binary search tree, so every node splits its values into a
  smaller-left / larger-right range. That means you never have to search both
  subtrees: compare root.val against the pair (min, max) and you know which
  single side holds both targets. The first node whose value lands between min
  and max is the split point, and that node is the lowest common ancestor.
BETTER APPROACH
  The better version here is the same comparison written as a loop: while (root
  != null) move root to root.left or root.right, and return root when min <=
  root.val <= max. That uses O(1) extra space, while this file pays for a call
  stack frame at every level, which on a skewed BST is one frame per node. It
  also recomputes Math.Min and Math.Max of p.val and q.val at every single level
  instead of once before the walk.
INVARIANT
  At every call, the current root is an ancestor of both p and q (true for the
  original root by assumption, and preserved by each step). If min > root.val,
  both targets are strictly greater than root.val, so by the BST ordering both
  must live in root.right, and recursing there keeps the invariant. The first
  time neither side holds both - that is min <= root.val <= max - the recursion
  stops, and since it is the deepest node still satisfying the invariant, it is
  the lowest common ancestor.
WATCH OUT
  There is no null check on root. If p or q is not actually in the tree, the
  descent runs off a leaf and the next call throws a NullReferenceException on
  root.val. The same happens if root is null on entry. The code also assumes p
  and q are non-null before the first Math.Min, so a null target crashes on the
  very first line rather than returning anything. It relies on distinct values
  too: with duplicate values in the BST, min <= root.val <= max can match a
  wrong node higher up.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What changes if it is a plain binary tree, not a BST?
     The value comparison is useless, so you recurse into both children and
     return root when both sides come back non-null, one side's result
     otherwise. That becomes O(n) time because every node must be visited.
  2. How do you return null when p or q is not present?
     Do the descent to find the split node, then run two separate searches from
     it to confirm both p.val and q.val exist below; return null if either
     search fails. Cost stays proportional to the height, just with a constant
     factor of about three walks.
  3. What if each node has a parent pointer instead?
     Walk up from p collecting nodes into a HashSet, then walk up from q and
     return the first node already in the set. That is O(h) time and O(h) space,
     and it needs no BST property at all.
  4. How would you extend it to the LCA of k nodes in a BST?
     Replace p.val and q.val with the minimum and maximum over the whole list,
     computed once, then run the identical descent - the split logic does not
     care how many targets sit inside the range.
TRIGGER
  The input is a binary search tree and the question is about a relationship
  between two nodes - use the ordering to pick one child instead of searching
  both.
C# NOTE
  Every branch of this method returns the recursive call directly with nothing
  after it, so it converts to a while (true) loop by just reassigning root - a
  mechanical rewrite that drops the stack frames. Also hoist the Math.Min and
  Math.Max pair out of the recursion by adding a private helper that takes min
  and max as int parameters.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
