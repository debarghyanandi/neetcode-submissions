// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive post-order DFS, depth = 1 + max(children)
// #  [dfs-recursive-depth]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  each node visited once, recursion depth equals tree height which is
// #  O(n) worst case for a skewed tree
// ##########################################################################

public class Solution
{
    // my solution
    public int MaxDepth(TreeNode root)
    {
        if (root == null)
            return 0;
        return Math.Max(MaxDepth(root.left), MaxDepth(root.right)) + 1;
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - height = 1 + max of child heights
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Depth is defined recursively on the tree's own shape, so the code can mirror
  the definition exactly. The longest root-to-leaf path through root must pass
  through either root.left or root.right, so it equals one more than the longer
  of the two subtree answers. Nothing about a subtree's depth depends on its
  parent or its sibling, so each node needs only the two values returned from
  below - no accumulator parameter, no shared state, no second pass.
THE RECURSIVE CONTRACT
  MaxDepth(x) returns the number of nodes on the longest downward path starting
  at x, counting x itself; 0 when x is null. Every line in the body assumes the
  two recursive calls already satisfy that contract for strictly smaller
  subtrees, and the body's job is only to restore it for x. Write the contract
  down before trusting the one-liner - the whole proof is the claim that
  Math.Max(left, right) + 1 turns two valid child answers into a valid answer
  for x.
CORRECTNESS
  Induction on the height of the subtree. Base: root == null has no nodes, and 0
  is returned. Step: assume MaxDepth(root.left) and MaxDepth(root.right) are
  correct. Any downward path from root either stops at root (length 1) or steps
  into one child and continues along a longest path there. Those cases are
  exactly 1, MaxDepth(root.left) + 1, and MaxDepth(root.right) + 1; the maximum
  of the three is Math.Max(left, right) + 1, because a null child contributes 0
  and so the +1 alone covers the stop-at-root case. A leaf shows this
  concretely: Math.Max(0, 0) + 1 = 1.
WATCH OUT
  1. The + 1 lives outside Math.Max, not inside either call.
  Math.Max(MaxDepth(root.left) + 1, MaxDepth(root.right)) silently drops a level
  on right-heavy trees.
  2. The null base must return 0, not 1. Return 1 and a leaf reports 2 and an
  empty tree reports 1 - off by one everywhere, and the leaf case is the one a
  quick test misses if you only try a two-level tree.
  3. There is deliberately no null check on root.left or root.right before
  recursing. The base case absorbs them. Adding child-level guards forces you to
  hand-handle the one-child-null case, which is where the classic bug (returning
  the non-null child's depth without the +1, or treating a one-child node as a
  leaf) creeps in.
  4. Both branches always execute - Math.Max is an ordinary method call, so
  there is no short-circuit that lets you skip a subtree.
TRIGGER
  Reach for this shape whenever the answer at a node is a pure function of the
  answers at its children and nothing flows downward. Height, node count, sum,
  "does this subtree contain x", min/max value - all the same three lines with
  Math.Max swapped out. If information has to travel down (a path sum target, a
  valid BST range), you need a parameter instead, and this template is the wrong
  one.
FOLLOW-UPS AN INTERVIEWER ASKS
  1. "What breaks on a degenerate tree?" A tree that is one long chain of right
  children makes the recursion nest once per node, and deep enough input blows
  the call stack - a hard crash, not a slow answer. The fix is to stop using the
  call stack: BFS with a queue counting levels, or DFS with an explicit Stack of
  (node, depth) pairs.
  2. "Now check whether the tree is balanced." Do not call MaxDepth at every
  node - that re-walks subtrees. Reuse this exact post-order shape and return -1
  as a sentinel meaning "already unbalanced below me", propagating it up as soon
  as the two child heights differ by more than 1.
  3. "Now find the diameter." Same walk: keep returning the height, but on the
  way up also record left + right against a running best. The value you return
  and the value you track are allowed to differ - that is the trick worth
  remembering from this file.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
