// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive bounds validation with return tuple
// #  [recursive-bounds-check]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each node visited once; recursion depth O(n) in worst case.
// ##########################################################################

// Sentinel values are safe only because Node.val is restricted to [-1000000000, 1000000000];
// if the value range can reach int.MinValue/MaxValue, use long or nullable bounds.
public class Solution
{
    //my solution
    public bool IsValidBST(TreeNode root)
    {
        var result = IsValidBSTWithMinMax(root);
        return result.found;
    }

    public (int min, int max, bool found) IsValidBSTWithMinMax(TreeNode root)
    {
        //Empty
        if (root == null)
            return (int.MaxValue, int.MinValue, true);

        //Leaf Node
        if (root.right == null && root.left == null)
            return (root.val, root.val, true);

        var left = IsValidBSTWithMinMax(root.left);
        var right = IsValidBSTWithMinMax(root.right);

        // left subtree must be valid and its max < root
        if (!left.found || left.max >= root.val)
            return (0, 0, false);

        if (!right.found || right.min <= root.val)
            return (0, 0, false);

        //we came to here means valid bst
        int min = Math.Min(left.min, root.val);
        int max = Math.Max(right.max, root.val);

        return (min, max, true);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - bubble up (min, max, valid)
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  result   the tuple returned for the root; only .found is read
  left     (min, max, found) summary of the entire left subtree
  right    (min, max, found) summary of the entire right subtree
  min      smallest value in this subtree = Math.Min(left.min, root.val)
  max      largest value in this subtree = Math.Max(right.max, root.val)
WHY THIS PATTERN
  "Valid BST" is not a local rule about a node and its two children; every value
  in the left subtree must be smaller than root.val, not just the child. So a
  node cannot be judged until its subtrees report something about themselves.
  Post-order fits: each call returns the subtree's min and max plus found, and
  the parent tests left.max >= root.val and right.min <= root.val in O(1).
BRUTE FORCE
  The first thing most people write is: for each node, walk the whole left
  subtree to find its maximum and the whole right subtree to find its minimum,
  then compare. That is correct but re-walks the same nodes once per ancestor,
  so it costs O(n^2) on a skewed tree. This file walks each node exactly once
  because the min and max travel upward with the return value instead of being
  recomputed.
INVARIANT
  After IsValidBSTWithMinMax(x) returns with found == true, min and max really
  are the smallest and largest values in the subtree rooted at x, and that
  subtree is a valid BST. The parent only trusts those numbers after checking
  left.found and right.found, so a false never gets treated as a real range.
  Because the parent compares against the extreme value of the whole subtree,
  not just the child, the BST rule holds for every ancestor-descendant pair,
  which is exactly the definition.
THE NULL SENTINELS
  An empty child returns (int.MaxValue, int.MinValue, true), and both of its
  jobs are to be neutral. In the guard, right.min = int.MaxValue can never be <=
  root.val, and left.max = int.MinValue can never be >= root.val, so a missing
  child never rejects the tree. In the combine step, Math.Min(left.min,
  root.val) collapses to root.val and Math.Max(right.max, root.val) does the
  same, so the returned range is still exact.
WATCH OUT
  The header comment is right and the code truly depends on it: a node holding
  int.MaxValue with no right child makes right.min <= root.val true (MaxValue <=
  MaxValue) and a valid tree is rejected. The failure return (0, 0, false)
  carries meaningless min and max; it is safe only because every caller reads
  .found first, so do not "optimize" by reading left.max before checking
  left.found. Note the asymmetry - min only consults left.min and max only
  consults right.max; that is correct here only because the validity checks
  already passed, so copying those two lines into another problem is risky. The
  leaf-node special case is dead weight: with both children null the general
  path already returns (root.val, root.val, true).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this top-down instead of bottom-up?
     Pass an allowed range (low, high) down as long or nullable int and check
     low < root.val < high at each node. Same O(n), but it fails fast on the
     first bad node and sidesteps the int.MaxValue sentinel problem entirely;
     the cost is an extra parameter instead of a returned tuple.
  2. The tree is one long chain of a million nodes. What breaks?
     The recursion depth equals the height, so the call stack can overflow.
     Switch to an iterative inorder traversal with an explicit Stack<TreeNode>
     and one prev variable, asserting prev < current.val; same time, and the
     stack lives on the heap instead of the thread stack.
  3. What if equal values are allowed in the left subtree?
     Change left.max >= root.val to left.max > root.val. The strict >= and <=
     here are what enforce the "no duplicates" version of the BST rule.
  4. How would you return the size of the largest BST subtree instead of a
  yes/no?
     Add a size field to the tuple and stop returning early on failure - an
     invalid subtree still returns found = false but the parent keeps scanning,
     and you track a running best across all nodes. Same single post-order pass.
TRIGGER
  A tree rule that talks about a whole subtree ("all values below", "every
  descendant") rather than just the immediate children - return a small summary
  tuple from each node in post-order.
C# NOTE
  Naming the tuple fields as (int min, int max, bool found) is what makes
  left.max readable instead of left.Item2, and ValueTuple is a struct, so the
  per-node return is a plain value with no class to define. If you later add
  more fields, a small readonly record struct keeps the same cost with a real
  type name.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
