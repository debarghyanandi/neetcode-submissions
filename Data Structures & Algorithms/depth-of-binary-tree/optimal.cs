// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive post-order DFS computing max depth   [dfs-recursive]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (was optimal-variant.cs)
// #
// #  visits each node once; recursion call stack depth equals tree height,
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
 PATTERN : DFS post-order fold - depth = 1 + max(child depths)
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Depth is defined recursively, so the code can be the definition. The answer at
  root needs exactly two numbers from below - the depth of root.left and the
  depth of root.right - and nothing about the left subtree changes what the
  right subtree returns. That independence is why there is no accumulator
  parameter, no depth argument threaded down, no visited set: the value flows up
  only, on the return path. Any tree quantity with that shape collapses to this
  three-line body.
CORRECTNESS
  Induction on the height of the subtree.
  1. Base: root == null returns 0. An empty subtree contributes no nodes to any
  root-to-leaf path, so 0 is the identity the parent's +1 builds on.
  2. Step: assume MaxDepth(root.left) and MaxDepth(root.right) are correct for
  the strictly shorter subtrees. Every root-to-leaf path from root starts with
  root and then lies entirely inside one child subtree - it cannot straddle
  both. So the longest such path has length 1 + max over the two children, which
  is exactly Math.Max(...) + 1.
  3. Termination: each call recurses only on strict descendants, and the null
  check is reached at the bottom of every branch.
INVARIANT
  MaxDepth(x) returns the number of nodes on the longest path from x down to a
  leaf in x's subtree, and reads nothing outside that subtree. The function is
  pure - no field writes, no shared counter - which is why the evaluation order
  of the two calls is irrelevant and why the same routine can be called on any
  node, not just the real root. There is no leaf check because a leaf is just a
  node whose two calls both hit the null base and return 0, giving Math.Max(0,
  0) + 1 = 1.
WATCH OUT
  The +1 sits outside Math.Max, applied once to the winner. The classic
  corruption is attaching it to one operand - Math.Max(MaxDepth(root.left),
  MaxDepth(root.right) + 1) - which silently biases every comparison toward the
  right child and reports a wrong depth on unbalanced trees.
  Also pin down the convention before writing the base case: returning 0 for
  null counts NODES, so a single-node tree answers 1. If a variant of the
  problem counts EDGES, a single node must answer 0 and the base case has to
  become -1 for null (or a leaf check returning 0). Same skeleton, different
  constant - and this is the first thing to re-derive rather than recall.
THE SCALE TRAP
  This is not tail recursive: Math.Max and the +1 both run after the two calls
  return, so every ancestor's frame stays live while the deepest node is being
  evaluated. The call chain is as long as the tree is tall, and a degenerate
  chain of nodes (a sorted-insert BST, a linked-list-shaped tree) makes that the
  node count. On tens of thousands of nodes that is a real
  StackOverflowException, which in .NET cannot be caught. If asked to harden it:
  BFS with a Queue<TreeNode>, counting levels as you drain each level's Count,
  or an explicit Stack of (node, depth) pairs. Both move the frames to the heap
  and are otherwise the same traversal.
TRIGGER
  Reach for this shape whenever the parent's answer is a pure function of its
  children's answers and nothing else. Only the combine line changes: max + 1
  here, Math.Abs(l - r) <= 1 plus the two subtrees for balanced-tree, l + r + 1
  with a side channel for diameter, l + r + node.val for path sums. If you catch
  yourself wanting to pass information DOWN the tree (a running prefix, a valid
  range for BST validation), this template no longer fits - add a parameter or a
  helper, because the return value alone cannot carry it.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
