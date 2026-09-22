// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Post-order DFS with single-pass height computation
// -  [recursive-balance-single-pass]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited exactly once, height computed and returned alongside
// -  balance status; worst-case call-stack depth is n for skewed trees.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsBalanced(TreeNode root)
    {
        return CheckBalance(root).balanced;
    }

    // returns balanced (1 or 0) and height as 2 element int array
    private (bool balanced, int height) CheckBalance(TreeNode node)
    {
        if (node == null)
            return (true, 0);
        var left = CheckBalance(node.left);
        var right = CheckBalance(node.right);
        bool balanced = left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1;
        int height = 1 + Math.Max(left.height, right.height);
        return (balanced, height);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - return height and balance together
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left     (balanced, height) result for node.left
  right    (balanced, height) result for node.right
  balanced true when both subtrees are balanced AND their heights differ by at most 1
  height   1 + the taller of the two child heights
WHY THIS PATTERN
  The question asks about every node at once: for each node the two subtree
  heights must differ by at most 1. Height is a bottom-up fact - a node's height
  depends on its children, not its parent - so post-order DFS is the natural
  order. CheckBalance computes left and right first, then folds both answers
  into one tuple, so each node learns its own height and its own balance verdict
  in the same visit.
BRUTE FORCE
  The first version most people write is a Height(node) helper plus an
  IsBalanced that, at every node, calls Height on both children and then
  recurses. That recomputes heights over and over: O(n log n) on a balanced tree
  and O(n^2) on a skewed one. It loses because height is discarded after each
  check instead of being passed up.
INVARIANT
  When CheckBalance(node) returns, height is the exact number of nodes on the
  longest path from node down to a leaf, and balanced is true exactly when the
  whole subtree rooted at node satisfies the rule. Both hold for the null base
  case, (true, 0). The AND chain carries the verdict of every descendant upward
  unchanged, so by induction the value at the root is the answer for the entire
  tree.
WATCH OUT
  The comment says the method "returns balanced (1 or 0) and height as 2 element
  int array" - it does not; it returns a (bool, int) tuple. That comment is
  stale and would confuse a reader looking for int[] indexing. The real risk
  here is depth: recursion is not cut short, so a long skewed tree can overflow
  the call stack. Also note balanced is computed with && but left and right are
  both already evaluated above, so no subtree is ever skipped - the
  short-circuit saves no work.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you stop early once you find an unbalanced node?
     Yes - return a sentinel height such as -1 the moment left.height or
     right.height is -1 or the gap exceeds 1, and check for -1 at the top. The
     asymptotic cost is the same, but it avoids walking the rest of a huge tree
     after failure.
  2. Deep skewed tree blows the stack. Fix it?
     Convert to an explicit iterative post-order with a Stack<TreeNode> plus a
     dictionary or a per-node height map, so depth costs heap memory instead of
     stack frames. The code gets noticeably longer and you must track whether a
     node's children have already been processed.
  3. What if the rule loosened to "heights differ by at most k"?
     Only the constant changes: Math.Abs(left.height - right.height) <= k.
     Nothing about the traversal or the height computation needs to move.
  4. How would you also return the first offending node, not just a bool?
     Widen the tuple to (bool balanced, int height, TreeNode culprit) and set
     culprit to node when the local check fails, preferring a child's culprit if
     one already exists, so the deepest failure is reported.
TRIGGER
  When a tree question asks a property of every node that depends on subtree
  size, height or sum, return that measurement upward with the verdict in one
  post-order pass.
C# NOTE
  The named value tuple (bool balanced, int height) is a struct, so each call
  returns by value with no heap allocation, and the field names make left.height
  readable without an extra class - much cleaner than the int[] the comment
  describes.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
