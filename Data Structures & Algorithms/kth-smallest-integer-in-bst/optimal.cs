// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  recursive in-order DFS with early-stop sentinel
// -  [inorder-traversal-early-stop]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  recurses left-root-right, propagating a found value up through return
// -  values, but worst case (skewed tree or large k) still visits and
// -  recurses through all n nodes
// --------------------------------------------------------------------------

public class Solution
{
    private int visitedCount = 0;

    public int KthSmallest(TreeNode root, int k)
    {
        if (root == null)
            return -1;

        int left = KthSmallest(root.left, k);
        if (left != -1)
            return left;

        visitedCount++;

        if (visitedCount == k)
        {
            return root.val;
        }

        return KthSmallest(root.right, k);
    }
}

/*
================================================================================
 PATTERN : In-order DFS with a visit counter and -1 sentinel
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A BST's in-order walk (left, node, right) emits values in ascending order.
  That turns "kth smallest" into "the kth node the walk touches" - no sorting,
  no heap, no extra container. The whole solution is an in-order traversal that
  happens to count as it goes, and stops talking the moment the count reaches k.
ALGORITHM
  1. A null subtree returns -1, meaning "the answer is not inside me."
  2. Recurse left first. If the left call returned anything other than -1, the
  answer was already found deeper; return it straight up without touching
  visitedCount and without looking right.
  3. Reaching this line means the entire left subtree was counted, so root is
  the next node in ascending order: visitedCount++.
  4. If visitedCount == k, root.val is the answer.
  5. Otherwise recurse right and return whatever it reports.
INVARIANT
  At the instant visitedCount++ executes for a node, visitedCount equals that
  node's 1-based rank among all values in the tree. It holds because the
  in-order order is exactly ascending order, and a node is reached only after
  every node that precedes it in that order has already run its own increment.
  The single place the walk skips work is step 2, and that skip only happens
  after the answer is already in hand - so no node is ever skipped before its
  rank is needed. Hence visitedCount == k identifies the kth smallest and
  nothing else can.
THE -1 SENTINEL IS THE TRAP
  -1 is doing two jobs: "not found" and "a node value." If the tree legitimately
  contained -1 and that node were the answer, the parent's `left != -1` test
  would read the correct answer as "not found," fall through, increment
  visitedCount for itself, and keep walking right - silently wrong. This file is
  only correct because the problem constrains values to 0 <= val <= 10^4. Say
  that out loud if asked; do not defend -1 as a general design. A nullable int,
  an out-parameter, or the iterative form removes the ambiguity entirely.
THE MUTABLE FIELD IS THE OTHER TRAP
  visitedCount is an instance field initialized once at construction, never
  reset inside KthSmallest. A second call on the same Solution object resumes
  from the previous count and returns garbage. It passes only because the judge
  builds a fresh Solution per test case. In real code this method is not
  reentrant and not thread-safe. Fixes: reset visitedCount at the public entry
  point and recurse into a private helper, thread the counter as `ref int`, or
  drop the field.
INTERVIEWER FOLLOW-UP
  Iterative version: push the left spine onto a Stack<TreeNode>, pop, decrement
  k, and if k hits 0 return that node's val, else move to node.right and push
  its left spine. No sentinel, no shared field, and the exit is immediate rather
  than propagated up the call chain.

  "What if the tree is modified often and kth smallest is queried often?" -
  augment each node with the size of its subtree. Then each query descends once:
  compare k against leftSize + 1 and go left, stop, or go right with k reduced
  by leftSize + 1. Insert and delete adjust the counts along the path they
  already walk.
TRIGGER
  Any phrasing about order statistics on a BST - kth smallest, kth largest, the
  rank of a value, the median - should pull up in-order traversal before it
  pulls up a heap or a sort. For kth largest, mirror the recursion: right, node,
  left, and the same counter works unchanged.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
