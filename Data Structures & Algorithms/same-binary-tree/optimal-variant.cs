// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  iterative DFS with explicit stack, pairwise compare   [dfs-compare]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Each node pair is pushed/popped once; the stack can hold O(n) pairs in
// -  the worst case (skewed tree).
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsSameTree(TreeNode root1, TreeNode root2)
    {
        var stack = new Stack<(TreeNode, TreeNode)>();
        stack.Push((root1, root2));

        while (stack.Count > 0)
        {
            var (node1, node2) = stack.Pop();

            if (node1 == null && node2 == null) continue;
            if (node1 == null || node2 == null || node1.val != node2.val)
            {
                return false;
            }
            stack.Push((node1.right, node2.right));
            stack.Push((node1.left, node2.left));
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Iterative DFS - explicit stack of node pairs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The question is not about one tree, it is about two trees walked in lockstep.
  So the unit of work is a PAIR of positions, not a node: the stack is
  Stack<(TreeNode, TreeNode)> and it is seeded with the single pair (root1,
  root2). Every push keeps the two halves at the same coordinate in their
  respective trees - left with left, right with right - which is what makes a
  single traversal enough to decide the answer.
INVARIANT
  Every pair (node1, node2) still on the stack denotes two subtree roots
  occupying identical positions, and every pair already popped has been
  verified: either both were null, or both were non-null with node1.val ==
  node2.val. Nothing is ever pushed that is not a same-position pair, and
  nothing is popped without being checked. Therefore draining the stack to empty
  means every reachable position in both trees agreed, and the final return true
  is earned rather than assumed.
ALGORITHM
  1. Push (root1, root2). Note this handles the both-null-roots case with no
  special branch.
  2. Pop a pair into node1, node2.
  3. Both null: this position is absent from both trees - agreement, continue.
  4. Exactly one null, or values differ: a structural or value mismatch exists,
  return false immediately; the remaining stack contents are irrelevant.
  5. Otherwise both are non-null and equal, so push (node1.right, node2.right)
  then (node1.left, node2.left) and loop.
  6. Stack empties without a false: trees are identical.
WHY THE CHECK ORDER IS FORCED
  The both-null continue must come first. If the one-null test ran first it
  would fire on (null, null) and wrongly return false. Then, inside the second
  if, the || short-circuits left to right: by the time node1.val != node2.val is
  evaluated, node1 == null and node2 == null have both been ruled out, so
  neither dereference can throw. Reordering that condition to put the value
  comparison first is the classic way to turn this into a null reference
  exception on any leaf's child.
WATCH OUT
  Null-ness is data here, not an edge case - it is what encodes shape. A version
  that skipped the one-null branch and compared only values would call a tree
  and its left-spine-only cousin equal. Also be honest about the (null, null)
  pushes: every leaf pushes two pairs that exist only to be popped and discarded
  by the continue. You could guard the pushes instead, at the cost of
  duplicating the null logic in two places; this file chose the single check
  point. Finally, the explicit stack lives on the heap, so a degenerate
  chain-shaped input cannot blow the call stack the way the recursive version
  can.
INTERVIEW FOLLOW-UP
  Traversal order does not matter for the verdict. Right is pushed before left
  only so left pops first, giving a left-to-right pre-order walk; swapping them,
  or replacing Stack with Queue for a level-order sweep, still returns the same
  boolean because the invariant says nothing about visit order. Order only
  changes WHICH mismatch you report first when several exist. The natural
  follow-ups from here are same-shape-different-values variants such as
  subtree-of-another-tree, which reuses this exact pair comparison as its inner
  test.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
