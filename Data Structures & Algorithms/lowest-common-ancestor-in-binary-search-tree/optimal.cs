// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  iterative BST binary search on value range   [bst-property-navigate]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  same BST-ordering navigation as submission-0 but iterative, so no call
// -  stack growth
// --------------------------------------------------------------------------

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        TreeNode node = root;

        while (node != null)
        {
            if (p.val > node.val && q.val > node.val)
            {
                node = node.right;
            }
            else if (p.val < node.val && q.val < node.val)
            {
                node = node.left;
            }
            else
            {
                return node;
            }
        }
        return null;
    }
}

/*
================================================================================
 PATTERN : BST Descent - stop at the first split point
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Nothing here searches. The only fact used is the BST ordering property:
  everything in node.left is smaller than node.val, everything in node.right is
  larger. That single fact turns "find the lowest common ancestor" into "walk
  down one path and notice when p and q stop agreeing on which way to go." No
  parent pointers, no path lists, no second traversal, no comparing subtree
  results on the way back up.
INVARIANT
  At the top of every loop iteration, node is an ancestor of both p and q (root
  satisfies this trivially). The body only moves node in a direction that both
  p.val and q.val agree on, so the invariant is preserved: if both values are
  greater than node.val, both targets live in the right subtree, so node.right
  is still a common ancestor. Same mirrored for left. Therefore the first node
  we do NOT move past is a common ancestor, and since every node above it was
  strictly higher on the same root path, it is the lowest one.
ALGORITHM
  1. node = root.
  2. If p.val > node.val AND q.val > node.val, both targets are strictly right:
  node = node.right.
  3. Else if p.val < node.val AND q.val < node.val, both are strictly left: node
  = node.left.
  4. Else return node. This else fires in exactly two situations - the split
  (one target on each side) and the hit (node.val equals p.val or q.val). Both
  are correct answers, and they are the same answer for the same reason: a node
  is defined as a descendant of itself, so if node IS p, no descendant of node
  can be an ancestor of p.
WHY THE COMPARISONS MUST BE STRICT
  Change the first condition to p.val >= node.val && q.val >= node.val and the
  code breaks on the ancestor-of-itself case: when node.val == p.val and q sits
  in the right subtree, it would descend into node.right and walk right past the
  real answer, eventually returning q instead of p. Strict > and < are what make
  the else branch absorb equality. This is the detail an interviewer pokes at.
WATCH OUT
  The comparisons are on .val, not reference equality - correct only because the
  problem guarantees unique node values. With duplicates in the BST this logic
  has no way to tell which physical node p refers to.

  The trailing return null is unreachable under the problem's guarantee that
  both p and q exist in the tree; it is there because C# needs every path to
  return. If an interviewer relaxes that guarantee, this method silently returns
  a wrong split node rather than null - detecting an absent target needs an
  explicit membership check, which the loop does not do.
FOLLOW-UP TO EXPECT
  "Now it is a plain binary tree, not a BST." The ordering property is gone, so
  the descent has nothing to steer by. The answer becomes the recursive
  postorder version: recurse into both children, and a node is the LCA if it
  matches p or q, or if both child calls returned non-null. That one must touch
  every node and carries the recursion stack, so it is strictly worse - which is
  exactly why the BST version is worth remembering separately instead of just
  reusing the general one.
TRIGGER
  See a tree question that hands you a BST plus two target nodes and asks about
  a relationship between them (ancestor, distance, the path between them,
  insertion point): reach for the single downward walk driven by comparing both
  targets against node.val. If both comparisons agree, keep descending; the
  moment they disagree, you are standing on the answer.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
