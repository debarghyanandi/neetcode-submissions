// ##########################################################################
// #  optimal.cs            O(m * n) time / O(n) space
// #  recursive tree match at every node   [tree-compare-each-node]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  for each of m nodes in root, IsSameTree does an O(n) comparison
// #  against subRoot in the worst case; recursion stack depth is O(m) for a
// #  skewed tree
// ##########################################################################

public class Solution
{
    //My solution 
    public bool IsSubtree(TreeNode root, TreeNode subRoot)
    {
        if (root == null && subRoot != null)
            return false;

        if (IsSameTree(root, subRoot))
            return true;

        else
        {
            return IsSubtree(root.left, subRoot) || IsSubtree(root.right, subRoot);
        }
    }

    public bool IsSameTree(TreeNode first, TreeNode second)
    {
        if (first == null && second == null)
            return true;

        if (first != null && second != null && first.val == second.val)
            return IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right);
        return false;
    }
}

/*
================================================================================
 PATTERN : DFS anchor scan + full structural equality check
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  "Subtree" means an exact match rooted at some node, carrying every descendant
  with it - not a partial overlap, not a value-set containment. That definition
  gives you a finite candidate list: one anchor per node of root. So the problem
  splits cleanly into two recursions. IsSubtree picks the anchor; IsSameTree
  judges it. One traversal cannot do both, because a failed match at an anchor
  tells you nothing about whether its children match.
THE NULL ARGUMENT
  The else branch dereferences root.left and root.right with no null check of
  its own. It is safe only because of the two statements above it, and that is
  the fragile spot in this file - the guard lives in a different statement from
  the dereference.

  Walk it: if root is null and subRoot is not, the first if returns false. If
  root is null and subRoot is null too, IsSameTree(null, null) hits its first
  line and returns true, so IsSubtree returns true. Either way control never
  reaches the recursion with a null root. Delete that first if and a null root
  with a non-null subRoot falls straight through to root.left and throws.

  Equivalent and clearer: if (root == null) return subRoot == null;
WHAT ISSAMETREE ENFORCES
  Shape and value at once. The only path to true is null paired with null. Any
  (non-null, null) pairing falls past the second if and returns false - that
  clause is what rejects a candidate holding the right values but one extra
  child hanging off it, which is the whole difference between "subtree" and "the
  values appear below here".

  It recurses only when both nodes are non-null AND first.val == second.val, so
  a value mismatch stops at that node rather than descending. Both child calls
  are joined with &&, so the first structural disagreement collapses the whole
  comparison.
WATCH OUT
  1. After IsSameTree(root, subRoot) returns false you must keep searching both
  children. A failure at this anchor says nothing about deeper anchors. The ||
  in the else does this and short-circuits once the left side finds a match.

  2. Do not be tempted to skip anchors by testing root.val == subRoot.val first
  and only calling IsSameTree there. Values may repeat, and a node that matches
  on value can still be a false anchor while the real match sits deeper. Notice
  IsSubtree never inspects val at all - the anchor loop stays dumb on purpose.

  3. Anchors shallower than subRoot are still tried in full. There is no height
  precheck; IsSameTree rejects them through the (non-null, null) case. Correct,
  just not free.

  4. Repeated work is real: a deep anchor gets compared against subRoot once for
  itself, and its ancestors each ran their own comparison over overlapping
  nodes.
FOLLOW-UP AN INTERVIEWER WILL ASK
  "Can you beat the nested traversals?" Yes - serialize both trees with an
  explicit null sentinel and a delimiter before every value (for example ^3 for
  a node and # for null), then run KMP to find subRoot's string inside root's
  string. Linear.

  The delimiters are the point of the question. Without a marker before each
  value, 2 matches inside 12; without null sentinels, two different shapes
  serialize identically and you report a match that is not one.

  Alternative: Merkle-hash every node as h(val, h(left), h(right)), put all of
  root's hashes in a set, look up subRoot's hash. Expected linear, but a hash
  collision gives a wrong answer unless you verify the hit with an IsSameTree
  call anyway.
TRIGGER
  "Is X contained in Y" over trees, where containment means an exact match
  including all descendants. The tell is two nested recursions - an outer one
  that chooses a starting point and an inner one that verifies it - and it is
  the same skeleton as naive substring search: a loop over start positions plus
  a compare. That structural kinship is exactly why the KMP follow-up exists.
COMPLEXITY
  Time  : O(m * n)
  Space : O(n)
================================================================================
*/
