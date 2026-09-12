// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  post-order DFS returning height and balance together
// -  [bottom-up-height-balance]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  single post-order traversal computes height and balance
// -  simultaneously; recursion stack depth is O(n) worst case for a skewed
// -  tree
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
 PATTERN : Post-order DFS returning (height, balanced) together
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Balance at a node needs the heights of both children, and height is itself a
  post-order quantity. The naive split - a Height helper called from an
  IsBalanced walker - recomputes the same subtree heights once per ancestor.
  Folding both answers into one return value from CheckBalance means every node
  is visited exactly once and its height is computed exactly once, from the
  heights its children already handed up.
BRUTE FORCE
  bool IsBalanced(node) => node == null || (Abs(Height(node.left) -
  Height(node.right)) <= 1 && IsBalanced(node.left) && IsBalanced(node.right)),
  with a separate Height that walks the whole subtree. Correct, but Height(node)
  re-walks everything under node, and it is called again at every descendant. On
  a left-skewed chain of n nodes that is 1+2+...+n work, quadratic. The
  information Height already found is thrown away between calls; the tuple
  return is what keeps it.
INVARIANT
  CheckBalance(node) returns exactly two facts about the subtree rooted at node:
  height is its true height, and balanced is true if and only if EVERY node
  inside that subtree satisfies the height-difference condition - not just node
  itself. The base case (true, 0) gives an empty subtree height 0, so a leaf
  returns (true, 1) and its null children differ by 0. Because height is
  returned unconditionally and correctly even when balanced is false, a parent
  can never be misled about geometry by an unbalanced child.
ALGORITHM
  1. Null node returns (true, 0).
  2. Recurse left, then right, capturing both tuples in left and right.
  3. balanced = left.balanced && right.balanced && Math.Abs(left.height -
  right.height) <= 1. All three conjuncts are required.
  4. height = 1 + Math.Max(left.height, right.height), computed regardless of
  balance.
  5. IsBalanced returns only the .balanced field of the root's tuple; the root
  height is discarded.
WATCH OUT
  The two left.balanced && right.balanced conjuncts are the part people drop.
  Without them you would only check the root: a tree whose left and right
  subtrees both have height 3 passes the Math.Abs test at the root even if the
  left subtree is internally a skewed chain. Balance is defined over all nodes,
  and only the recursive conjunction carries that up.

  The && short-circuits, but nothing is saved by it - both CheckBalance calls
  have already run on the lines above. An early return after left when
  !left.balanced would prune real work; it does not change the worst case, since
  a fully balanced tree never triggers it.

  The comment above CheckBalance is stale: it describes returning 1 or 0 in a
  2-element int array. The method returns a named ValueTuple, so left.height and
  left.balanced are the accessors, not left[0].
FOLLOW-UP
  The classic compression of this: return int alone, using -1 as a sentinel for
  "unbalanced somewhere below", and propagate -1 upward on sight. Same
  traversal, one less field, but it conflates a height with an error code - the
  tuple here is the more honest version and is what you should be able to
  defend.

  The other likely probe is depth. This recurses to the height of the tree, so a
  degenerate n-node chain puts n frames on the call stack and can overflow
  before the algorithm's own cost matters. The answer is an explicit stack with
  a post-order iterative traversal, carrying computed heights in a dictionary or
  on the stack itself.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
