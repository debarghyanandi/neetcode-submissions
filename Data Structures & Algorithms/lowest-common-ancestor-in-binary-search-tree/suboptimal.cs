// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  recursive BST binary search on value range   [bst-property-navigate]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  recurses one direction per call using BST ordering; worst-case skewed
// #  tree gives O(n) depth and O(n) call stack
// ##########################################################################

public class Solution {
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        //My solution
        int min = Math.Min(p.val, q.val);
        int max = Math.Max(p.val, q.val);

        if (max >= root.val && min <= root.val)
            return root;

        if (min > root.val)
        {
            return LowestCommonAncestor(root.right, p, q);
        }
        else
            return LowestCommonAncestor(root.left, p, q);
    }
}

/*
================================================================================
 PATTERN : BST descent - first node that splits p and q
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The BST ordering property is the whole solution: everything in root.left is
  below root.val, everything in root.right is above it. So the lowest common
  ancestor is simply the first node encountered from the root whose value falls
  inside the closed interval [min, max]. There is no need to search both
  subtrees and merge results, and no need to know anything about the tree's
  shape - a single root-to-node walk is enough.
INVARIANT
  On entry to every call, both p and q are located somewhere in the subtree
  rooted at root. The recursion preserves it: if min > root.val then both values
  are strictly greater than root.val, so by the BST property both nodes must be
  in root.right; symmetrically, if max < root.val both must be in root.left. The
  first call satisfies the invariant by the problem's guarantee that p and q are
  in the tree. The moment root.val separates them, the invariant plus the split
  means neither subtree alone contains both, so root is the lowest node that
  does.
WHY THE TEST IS EXHAUSTIVE
  The guard is max >= root.val && min <= root.val. Its negation is max <
  root.val || min > root.val, and because min <= max by construction those two
  disjuncts are mutually exclusive - so after the guard fails, testing only min
  > root.val is sufficient and the bare else is exactly the max < root.val case.
  That is the follow-up an interviewer asks: why is a two-way branch safe when
  there were three logical outcomes.

  The inclusive comparisons also silently handle the case a lot of people miss:
  when root is itself p or q. If root.val == min, then min <= root.val and max
  >= root.val both hold, so root is returned - a node is its own ancestor. The
  same holds for p == q, which returns that node.
WHAT THE OPTIMAL VERSION CHANGES
  This descends in one direction and never combines results from two branches -
  it is tail recursion, so the call stack carries no information. Convert it to
  a loop and the auxiliary space drops to a constant:

    int min = Math.Min(p.val, q.val), max = Math.Max(p.val, q.val);
    while (root.val < min) ... else if (root.val > max) root = root.left; else
    return root;

  Two things improve. The stack frames disappear, which matters because a BST
  built from sorted inserts degenerates into a chain and the depth of that chain
  is the depth of the recursion. And min/max are computed once instead of being
  recomputed by Math.Min and Math.Max in every single frame, even though p and q
  are identical arguments at every level - that recomputation is pure waste that
  the loop form makes structurally impossible.
WATCH OUT
  There is no null check on root. This is only safe because the problem
  guarantees both p and q exist in the tree, which makes the invariant hold and
  guarantees the split node is found before the walk runs off the bottom. Hand
  this the same code with a p that is not present and the descent falls past a
  leaf and dereferences null at root.val. If an interviewer relaxes that
  guarantee, the fix is a while (root != null) loop that returns null on exit.

  The comparison is on values, not references. That matches the standard problem
  statement (all node values unique), but if duplicate values were allowed,
  matching p by val would be ambiguous and the descent could commit to the wrong
  subtree.
TRIGGER
  Two target nodes plus a search tree, and the question is where their paths
  diverge. Reach for the interval test on [min, max]. If the tree is a plain
  binary tree with no ordering, this collapses - you lose the ability to pick a
  direction and have to fall back to the postorder recursion that searches both
  subtrees and returns the node where two non-null results meet.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
