// ##########################################################################
// #  optimal-variant.cs    O(n) time / O(n) space
// #  recursive DFS computing max depth   [dfs-recursive]
// #  ties with optimal.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-2)
// #
// #  visits each node once; recursion stack depth equals tree height,
// #  worst-case O(n) for a skewed tree
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
 PATTERN : Post-order DFS - height = 1 + max(child heights)
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Depth is defined recursively, so the code is the definition. Each node needs
  exactly two numbers from below and nothing from above, and there is no
  information that has to cross between the left and right subtrees. That is why
  there is no accumulator parameter and no shared max field anywhere in this
  file - the answer rides up on the return value alone.
INVARIANT
  MaxDepth(x) returns the number of nodes on the longest path from x down to a
  leaf, counting x itself, and 0 for an empty subtree. That single contract is
  what makes it legal to call MaxDepth(root.left) and MaxDepth(root.right) with
  no extra state. Note it counts nodes, not edges - a one-node tree answers 1,
  not 0.
CORRECTNESS
  Induction on subtree height. Base: root == null returns 0, which is the
  contract for empty. Step: assume both child calls satisfy the contract. The
  longest downward path through root is root itself plus the longest path in
  whichever child subtree is deeper, which is exactly
  Math.Max(MaxDepth(root.left), MaxDepth(root.right)) + 1. A leaf falls out of
  the same line: both children are null, so max(0, 0) + 1 = 1. Every node is
  entered exactly once, because a node is reachable only through its unique
  parent link.
THE TRAP - BASE CASE ON NULL, NOT ON LEAF
  The tempting rewrite is if (root.left == null && root.right == null) return 1.
  It breaks on a node with exactly one child: the missing side still owes the
  recursion a 0, so you end up special-casing each side, and the top-level call
  with root == null now dereferences null. Recursing into null and returning 0
  handles the empty tree, the one-child spine, and the leaf with one branch.
WATCH OUT - STACK DEPTH
  The extra space here is the call stack: one frame per node on the current
  root-to-leaf path. A degenerate chain where every node has only a left child
  pushes that to n frames; a balanced tree keeps it near log n. Neither
  recursive call sits in tail position - Math.Max consumes both results and adds
  1 after they return - so this cannot be flattened into a loop without
  introducing an explicit stack. Blowing the stack on a deep skewed input is the
  reason to switch routes, not the running time.
INTERVIEWER FOLLOW-UP
  The other route to the same bound is iterative: BFS with a queue, draining one
  full level per outer iteration and incrementing a counter, or DFS with an
  explicit stack of (node, depth) pairs. Both move the frames onto the heap -
  BFS peaks at the widest level rather than the deepest path, which is the
  better trade on a broad shallow tree and the worse one on a complete tree's
  bottom row. Expect the two extensions that reuse this exact skeleton: diameter
  (at each node also consider left + right and update a field, but still return
  max + 1) and balanced-check (return -1 as a sentinel so an unbalanced subtree
  aborts every caller above it).
TRIGGER
  Reach for return-the-height post-order whenever a node's answer is a pure
  function of its children's answers. If the recursion instead needs something
  about its ancestors - remaining path sum, depth so far, a valid-BST range -
  that value goes down as a parameter and the shape flips to pre-order with a
  void or bool return.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
