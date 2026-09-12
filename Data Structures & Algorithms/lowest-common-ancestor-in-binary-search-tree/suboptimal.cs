// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  recursive BST descent using ordering property
// #  [bst-property-navigate]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  same single-direction descent as optimal.cs but via tail recursion, so
// #  a skewed tree grows the call stack to depth n
// ##########################################################################

public class Solution
{
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
 PATTERN : BST Descent - stop at the first node that splits the pair
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
CORE IDEA
  In a BST everything in root.left is below root.val and everything in
  root.right is above it. So p and q sit on opposite sides of a node exactly
  when that node's value lies between them, and the highest such node is the
  answer. After the first two lines the code never compares p to q again - it
  only ever compares the pair's bounds, min and max, against root.val. Walk down
  while both bounds fall on the same side; stop the moment they straddle.
WHY MIN AND MAX
  Normalizing p.val and q.val into min and max erases a case split. Without it
  you would handle p.val < root.val < q.val and q.val < root.val < p.val
  separately. With it, the single test min <= root.val <= max covers both
  orders, and it also covers root being p or q itself: if root.val equals min,
  then min <= root.val holds and max >= root.val holds too (max is at least
  min), so the node returns itself. That matches the standard definition in
  which a node counts as its own descendant.
INVARIANT
  On entry to any call, both p and q live somewhere in root's subtree. True at
  the top by the problem's guarantee, and preserved by each step: the right
  branch is taken only when min > root.val, which puts both values strictly
  above root, and by BST ordering every such value in this subtree is in
  root.right. The else branch is reached only when max < root.val - min >
  root.val was consumed by the first if, and the guard above already rejected
  max >= root.val combined with min <= root.val - so both values are strictly
  below and live in root.left. This invariant is the reason there is no null
  check on root: the recursion cannot step into an empty child while two real
  nodes are still supposed to be inside it.
WHY THIS LOSES
  The descent never backtracks, and the value returned by the recursive call is
  passed straight up untouched. That makes it a loop in disguise. Hoist min and
  max out, then: while true, if min > root.val set root = root.right; else if
  max < root.val set root = root.left; else return root. Identical comparisons,
  identical path, but nothing is stacked and the extra space becomes constant.
  As written, each level costs a frame, and on a degenerate BST - values
  inserted in sorted order, every node with a single child - the depth is the
  node count, not log n. Recomputing Math.Min and Math.Max at every level is the
  tell that state is being rebuilt on the way down instead of carried.
TRAPS
  1. The bounds are inclusive on both sides on purpose. Tightening max >=
  root.val to max > root.val breaks the case where root is q (or p) and the code
  descends past the answer into a subtree that holds only one of them.
  2. Ordering of the two ifs is load-bearing. The first branch tests min >
  root.val, not min >= root.val, because equality was already consumed by the
  guard that returns root. Reorder the tests and the equality case falls through
  into a wrong turn.
  3. There is no recovery from a wrong turn - no second child is ever examined.
  Correctness rests entirely on the BST property actually holding on the input:
  a violated ordering, or a duplicate value placed on the unexpected side, sends
  the descent into a subtree that does not contain both nodes, and the invariant
  above breaks with it.
FOLLOW-UP TO EXPECT
  "Now it is not a BST." Ordering gives you nothing and this collapses; you
  switch to the general post-order LCA - recurse into both children, return root
  if both sides came back non-null, otherwise return whichever side did - and
  you pay a full traversal because you have to look at every node.

  "What if p or q might not be in the tree?" This descent would return a node
  that straddles two values it never confirmed exist. The fix is to search for
  each value and verify it before (or while) descending, which is why the
  guarantee that both nodes are present is worth stating out loud when you
  present this.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
