// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  iterative DFS with explicit stack, pairwise compare
// -  [dfs-pairwise-compare]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node pair pushed/popped once from an explicit stack, moving the
// -  same pairwise DFS comparison off the call stack
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
 PATTERN : Iterative DFS - explicit stack of (node1, node2) pairs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The recursive version of this comparison is four lines, so the only reason to
  reach for an explicit stack is to move the traversal state off the call stack.
  On a degenerate tree - one long right spine - recursion needs a live call
  frame per level, and that is the input that overflows it. Here the pending
  work lives in a heap-allocated Stack instead.

  Pairs are the other half of the idea. A single Stack<(TreeNode, TreeNode)>
  keeps a position in tree 1 glued to the matching position in tree 2, so you
  never run two traversals and never have to ask which node in root2 corresponds
  to the node you are holding from root1.
INVARIANT
  Every tuple sitting on the stack is a pair of positions already known to be
  aligned: their ancestors matched in value and in shape all the way back to
  (root1, root2), and both were reached by the same sequence of left/right
  steps.

  That is what lets the loop body decide a purely local question - are these two
  positions compatible - and still get a global answer. The method returns true
  only after the stack drains, which is exactly the statement that every
  reachable aligned pair was compared and none of them answered no.
ORDER OF THE THREE CHECKS
  The both-null test must come first and must continue, not return. Two nulls
  mean two absent subtrees, which agree locally; returning true there would
  declare the whole trees equal off one matching missing child.

  Once that continue is past, the second if reads as a single guard. node1 ==
  null || node2 == null catches exactly-one-null, which is a shape mismatch.
  Only when both disjuncts are false does short-circuit evaluation let node1.val
  != node2.val run - so by construction neither dereference can hit a null.
  Reorder those two ifs, or split the value test into its own statement above
  them, and the first leaf throws NullReferenceException.
WHY NULLS GET PUSHED
  node1.right and node1.left are pushed unconditionally, so null children do go
  on the stack and are popped and discarded by the first check.

  The alternative - test the children before pushing - shrinks the stack but
  moves the same case analysis to the push site, where it has to inspect both
  trees' children together anyway. Pushing blind keeps exactly one place in the
  method where nullness is reasoned about, and that is why the body stays this
  short.
STRUCTURE NOT JUST VALUES
  The node1 == null || node2 == null line is the one people drop, and it is the
  one carrying shape. Without it you are comparing sequences of values: a root 2
  with a left child 1 would compare equal to a root 2 with a right child 1,
  since both yield 2 then 1.

  Position is recorded by nothing except the pairing plus that null test. Same
  reason a naive preorder-list comparison needs null sentinels to be correct.
TRAVERSAL ORDER IS FREE
  right is pushed before left, so left pops first and the walk is preorder, left
  to right. Correctness does not depend on it. Swap the two pushes, or use a
  Queue and dequeue from the front for BFS, and the answer is identical - true
  requires visiting every aligned pair regardless of order, and a mismatch
  anywhere returns false whenever that pair is reached.

  Order only decides which mismatch you find first. Worth knowing if the
  follow-up asks you to report where the trees diverge rather than just whether.
WATCH OUT
  1. return false abandons a non-empty stack mid-traversal. That is the intended
  early exit; the remaining tuples are just dropped.

  2. root1 and root2 both null needs no special case at the top. One pair goes
  in, the first check continues, the loop ends, true comes back.

  3. This shape does not generalize to "is root2 a subtree of root1" by itself -
  the pair here is anchored at the roots, and subtree search means restarting
  this comparison from every candidate node in tree 1.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
