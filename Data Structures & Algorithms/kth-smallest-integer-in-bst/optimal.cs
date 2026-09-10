// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  in-order traversal with early return propagation
// -  [inorder-traversal-early-stop]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  recurses left-root-right, propagating a found value up through return
// -  codes to stop early, but worst-case recursion depth and node visits
// -  are O(n) for a skewed tree or large k
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
 PATTERN : In-order DFS - shared counter, -1 sentinel early exit
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A BST's in-order walk (left, node, right) emits values in ascending order. So
  "k-th smallest" reduces to "k-th node touched by an in-order walk" - no
  sorting, no extra list. The only state needed is a count of how many nodes
  have already been emitted, which is what visitedCount holds.
INVARIANT
  At the moment the line visitedCount++ executes for a given node, every node
  with a smaller value has already been counted, and no node with a larger value
  has. That is exactly what makes the equality test visitedCount == k correct
  rather than approximate: the counter is a rank, not just a tally. It holds
  because the left recursive call has fully returned before the increment, and
  the right call has not yet started.
HOW THE ANSWER PROPAGATES
  Read the method as returning either a found value or the sentinel -1 for "not
  found in this subtree".
  1. Empty subtree: return -1 immediately, nothing counted.
  2. Recurse left. If it came back non-sentinel, the answer was already located
  deeper; return it untouched. Crucially this skips visitedCount++, so no node
  is counted twice on the way up the stack.
  3. Otherwise this node is the next in sorted order: increment, and if the rank
  now equals k, this node's val is the answer.
  4. Otherwise the answer lies to the right; tail into the right subtree and
  pass its result up verbatim.
  Once a match is found, every remaining frame is a pure pass-through, so the
  traversal stops doing real work.
THE TRAP - THE SENTINEL IS A VALUE
  -1 does double duty: it means "empty/not found" and it is also a legal int
  that root.val could hold. If any node stores -1 and it happens to be the k-th
  smallest, step 3 returns -1, and the caller's check left != -1 reads that as
  "not found" and keeps walking - wrong answer, silently. This works here only
  because the problem constrains node values to be non-negative. If an
  interviewer removes that constraint, the fix is to return a nullable int
  (int?) or to return a bool found plus an out int result, so "found" is carried
  out of band from the value.
THE OTHER TRAP - MUTABLE INSTANCE STATE
  visitedCount is a field on Solution, not a local or a ref parameter. It is
  initialized to 0 exactly once, when the object is constructed. Calling
  KthSmallest twice on the same Solution instance gives a wrong result the
  second time, because the counter starts where the first call left it. A judge
  harness that news up a fresh Solution per test case hides this; a caller that
  reuses the object, or two threads sharing it, does not. Say this out loud in
  an interview before being asked - it is the standard follow-up to any solution
  that keeps traversal state in a field.
FOLLOW-UP - REPEATED QUERIES
  If asked "what if the tree is modified often and KthSmallest is called many
  times?", the counter approach is the wrong shape: each query re-walks up to k
  nodes from scratch. The answer is to augment every node with the size of its
  subtree. Then a query descends once: compare k against left subtree size to
  decide go left, stop here, or go right with k reduced - one root-to-node path
  per query, and insert/delete updates the sizes along the path they already
  touch.

  A second common follow-up is "do it without recursion": push left spine onto
  an explicit stack, pop, decrement k, push the popped node's right spine,
  repeat. Same order, same early stop, but it removes both problems above - the
  counter is a local, and there is no sentinel because the loop returns directly
  from the pop.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
