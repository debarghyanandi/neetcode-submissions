// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  bottom-up DFS returning height and balanced flag
// -  [bottom-up-height-balance]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  single post-order traversal computes height and balance
// -  simultaneously, avoiding recomputation; recursion stack depth is O(n)
// -  worst case for a skewed tree
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
        if (node == null) return (true, 0);
        var left = CheckBalance(node.left);
        var right = CheckBalance(node.right);
        bool balanced = left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1;
        int height = 1 + Math.Max(left.height, right.height);
        return (balanced, height);
    }
}

/*
================================================================================
 PATTERN : Bottom-up DFS returning height and balance in one pass
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
BRUTE FORCE, AND WHY THIS BEATS IT
  The obvious version walks every node and calls a separate Height(node) helper
  on its two children. Height is itself a full subtree walk, so a node at depth
  d is re-measured once per ancestor: O(n^2) on a left-leaning chain, since the
  top node measures n nodes, the next n-1, and so on.

  CheckBalance kills the repetition by having the traversal that checks balance
  also carry the height back up. Each node is visited exactly once, and its
  height is computed exactly once, from the two values its children already
  returned.
THE CONTRACT
  CheckBalance(node) returns a pair with a fixed meaning that must hold at every
  node, not just the root:

  1. balanced - true if EVERY subtree rooted inside node's subtree satisfies the
  height rule, not merely node itself.
  2. height - number of nodes on the longest downward path from node, always the
  true height regardless of what balanced says.

  IsBalanced just reads .balanced off the root pair and throws the height away.
HEIGHT CONVENTION
  null returns (true, 0), so a leaf returns 1 + Math.Max(0, 0) = 1. This counts
  nodes, not edges; the textbook edge-height of a leaf is 0, not 1.

  The off-by-one never matters here because every height in the file comes from
  the same recursion. Math.Abs(left.height - right.height) is a difference of
  two heights, so the constant cancels, and Math.Max preserves the shift as it
  moves up. Just do not mix this helper with an edge-counting one.
WHY IT IS CORRECT
  Induction on subtree size. Base: an empty tree is balanced and has height 0,
  which (true, 0) states.

  Step: assume left and right are correct for the children. Then balanced =
  left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1
  is a literal transcription of the definition - both sides internally balanced,
  plus the local difference test - so it is correct for node. And height = 1 +
  Math.Max(left.height, right.height) is the longest child path plus node
  itself.

  The order matters: both recursive calls complete before either field is
  computed. That is what post-order buys you - children are already solved facts
  by the time the parent runs.
THE TRAP: A POISONED SENTINEL
  The common rewrite drops the tuple and returns int, using -1 to mean
  "unbalanced somewhere below." That version is only correct if every caller
  checks for -1 BEFORE using the value. Forget the guard and -1 flows into
  Math.Abs(left - right), where an unbalanced child of a null sibling gives
  Math.Abs(-1 - 0) = 1 <= 1 and the failure is silently reported as balanced.

  Carrying the bool separately means height is never a lie, so no call site
  needs a guard. That is the tradeoff being made - an allocation-free ValueTuple
  in exchange for not having to reason about sentinel propagation.
WATCH OUT
  The comment above CheckBalance - "returns balanced (1 or 0) and height as 2
  element int array" - describes an earlier int[] draft, not this code. Nothing
  returns an array or a 1/0 here. Fix or delete it before this file is read as
  reference.

  Also note there is no early exit. Once left.balanced is false the answer is
  already decided, but CheckBalance(node.right) on the line above has already
  run. The && short-circuits on the booleans, not on the recursion that produced
  them. Inserting an early return after the left call is a legitimate
  constant-factor win and changes nothing about the result.
INTERVIEWER FOLLOW-UP
  Expect "what breaks on a 100000-node linked-list-shaped tree?" - the recursion
  depth equals the height, so this stack-overflows on a fully skewed input long
  before it runs slowly. The answer is an explicit post-order stack keeping a
  map from node to computed height, or a parent-pointer iterative walk; the
  recurrence is unchanged, only the stack moves to the heap.

  Second likely probe: "can you report the deepest unbalanced node?" Replace the
  bool with the offending node reference or an int depth and keep the same merge
  rule - the shape of the recursion does not change.
TRIGGER
  Reach for this whenever a question asks for a property that must hold at every
  subtree, and checking it at one node needs an aggregate over that node's
  subtree. Diameter of a binary tree, max path sum, count of univalue subtrees,
  and validating a BST by passing ranges are the same skeleton: one post-order
  pass, return the aggregate the parent needs alongside the answer the problem
  asked for.

  The tell for the O(n^2) version you are replacing: a helper being called from
  inside another traversal over the same nodes.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
