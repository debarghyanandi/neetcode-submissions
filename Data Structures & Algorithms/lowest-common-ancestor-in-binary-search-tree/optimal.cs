// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  iterative BST descent using ordering property
// -  [bst-property-navigate]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  walks down one path using node.val comparisons, stopping at the
// -  split/hit node, worst-case depth n on a skewed tree but no extra
// -  memory
// --------------------------------------------------------------------------

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        TreeNode curr = root;

        while (curr != null)
        {
            if (p.val > curr.val && q.val > curr.val)
            {
                curr = curr.right;
            }
            else if (p.val < curr.val && q.val < curr.val)
            {
                curr = curr.left;
            }
            else
            {
                return curr;
            }
        }
        return null;
    }
}

/*
================================================================================
 PATTERN : BST descent - first split point is the LCA
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The ordering is information the generic LCA algorithm throws away. Everything
  in curr.left is below curr.val and everything in curr.right is above it, so
  one comparison of p.val and q.val against curr.val already says which subtree
  can still hold both nodes. That turns a search into a walk: one node per
  level, no branching, no backtracking, which is why a single curr pointer is
  the entire state.
INVARIANT
  At the top of every iteration, the subtree rooted at curr contains both p and
  q. True initially since curr = root. Preserved because curr only moves right
  when p.val and q.val both exceed curr.val (both nodes must then live in the
  right subtree) and only moves left when both are below. curr strictly descends
  each pass and the invariant guarantees the chosen child is non-null, so the
  loop cannot spin.
WHY THE SPLIT NODE IS THE ANSWER
  The else branch fires exactly when p and q disagree on a direction: p.val <=
  curr.val <= q.val, or the mirror. By the invariant curr is a common ancestor.
  Neither child can be one, because whichever child you descend into excludes
  the other node. So curr is the lowest such node, and it is unique - there is
  exactly one place where the two search paths diverge.
WHY NO EQUALITY CHECK IS NEEDED
  The else also absorbs the case p.val == curr.val (or q.val == curr.val):
  neither strict comparison holds, so it returns curr. That is correct, since
  this problem counts a node as a descendant of itself - when curr is p, p is
  the LCA. An explicit guard like 'if curr == p or curr == q return curr' would
  be dead weight, and interviewers often expect you to justify leaving it out
  rather than add it.
WATCH OUT
  Comparisons are on .val, not reference identity, so the code assumes BST
  values are distinct. It also assumes p and q are both actually in the tree; if
  one were missing the descent could run off a leaf, which is the only way to
  reach the final return null - unreachable under the stated guarantees, present
  only to satisfy the compiler. Note the code never assumes p.val < q.val: the
  two mirrored conditions handle either order.
TRIGGER
  Lowest or first common ancestor asked over a search tree, or any phrasing of
  'where do two search paths diverge'. The tell is that you can compare a target
  against the current node and rule out a whole subtree - the moment that holds,
  prefer the iterative descent over any traversal that visits both children.
FOLLOW-UP
  Plain binary tree with no ordering: post-order recursion that returns a found
  node upward, and the first node receiving a non-null result from both sides is
  the LCA. Nodes with parent pointers: walk both upward and intersect the
  chains. Asked to handle p or q possibly absent: this loop cannot detect it,
  since it never confirms it reached either node - you need a separate existence
  search for each before trusting the result.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
